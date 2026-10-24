"""Quick deployment using configuration from the existing local git-crypt uploader."""
import json
import math
import os
from pathlib import Path
import runpy
import shutil
import subprocess
import sys
import time
from urllib.parse import urlsplit
import requests


class DeploymentError(RuntimeError):
    pass


def parse_args(arguments):
    args = [value.lower() for value in arguments]
    if any(value not in {'full', 'prod', 'int', '--dry-run', '--all-files'} for value in args) or len(args) != len(set(args)):
        raise DeploymentError('Use: deployninestar [full] [prod|int] [--dry-run] [--all-files]')
    if 'prod' in args and 'int' in args:
        raise DeploymentError('Choose one environment.')
    return ('int' if 'int' in args else 'prod', 'full' in args, '--dry-run' in args, '--all-files' in args)


def find_setting(document, name):
    found = []
    def visit(value):
        if isinstance(value, dict):
            for key, child in value.items():
                if key.lower() == name.lower(): found.append(child)
                else: visit(child)
        elif isinstance(value, list):
            for child in value: visit(child)
    visit(document)
    if not found or any(value != found[0] for value in found):
        raise DeploymentError('Missing or ambiguous setting: ' + name)
    return found[0]


def site_url(value):
    if not isinstance(value, str) or not value.strip():
        raise DeploymentError('A site base URL is required.')
    value = value.strip()
    if '://' not in value: value = 'https://' + value
    parsed = urlsplit(value)
    if parsed.scheme != 'https' or not parsed.hostname or parsed.username or parsed.password or parsed.query or parsed.fragment:
        raise DeploymentError('Deployment requires a configured HTTPS site base URL.')
    return value.rstrip('/')


def api_base(settings, environment):
    if environment == 'int':
        try:
            document = json.loads((Path(settings['SRC_CONFIG']) / 'appsettings.json').read_text(encoding='utf-8-sig'))
        except (OSError, ValueError) as error:
            raise DeploymentError('Cannot read local appsettings.json.') from error
        return site_url(find_setting(document, 'SiteIntBaseUrl')) + '/api/deploy'
    endpoint = settings['API_ENDPOINT']
    suffix = '/api/deploy/upload'
    if not endpoint.endswith(suffix): raise DeploymentError('Unexpected Production upload endpoint.')
    return site_url(endpoint[:-len(suffix)]) + '/api/deploy'


def require_success(response, environment):
    if not 200 <= response.status_code < 300:
        raise DeploymentError('Server returned HTTP ' + str(response.status_code) + '; checkpoint retained.')
    try: body = response.json()
    except ValueError as error: raise DeploymentError('Server did not return deployment JSON.') from error
    if not isinstance(body, dict) or body.get('success') is not True:
        raise DeploymentError('Server reported deployment failure; inspect its logs.')
    if body.get('environment') != environment or body.get('protocolVersion') != 2:
        raise DeploymentError('Environment/protocol mismatch. Install the matching backend and server script first.')
    return body


def checkpoint_time(path):
    if not path.exists(): return 0.0
    try:
        value = float(path.read_text().strip())
        if value < 0 or not math.isfinite(value): raise ValueError()
        return value
    except (OSError, ValueError) as error: raise DeploymentError('Invalid deployment checkpoint: ' + str(path)) from error


def save_checkpoint(path, started_at):
    temporary = path.with_suffix(path.suffix + '.tmp')
    temporary.write_text(str(started_at))
    os.replace(str(temporary), str(path))


EXTENSIONS = {'bin': {'.dll', '.pdb'}, 'root': {'.config'}, 'config': {'.json'},
    'views': {'.cshtml', '.config'}, 'css': {'.css'}, 'scripts': {'.js'},
    'images': {'.png', '.jpg', '.jpeg', '.gif'}, 'videos': {'.mp4', '.webm'}}


def asset_plan(settings, repo, full, cutoff):
    excluded = {value.lower() for value in settings.get('EXCLUDED_FOLDERS', ())}
    specs = [('css', Path(settings['SRC_CSS'])), ('scripts', Path(settings['SRC_SCRIPTS'])),
        ('views', Path(settings['SRC_VIEWS'])), ('images', Path(settings['SRC_IMAGES'])),
        ('videos', repo / 'webapp/WebApplication/Videos')]
    if full: specs.append(('config', Path(settings['SRC_CONFIG'])))
    result = []
    for category, source in specs:
        if not source.is_dir():
            if category == 'videos': continue
            raise DeploymentError('Source folder missing: ' + str(source))
        for directory, dirs, files in os.walk(str(source)):
            dirs[:] = [value for value in dirs if value.lower() not in excluded]
            for name in sorted(files):
                path = Path(directory) / name
                if name in settings.get('EXCLUDED_VIEWS', ()) or path.suffix.lower() not in EXTENSIONS[category]: continue
                if path.stat().st_mtime > cutoff:
                    result.append((category, path.relative_to(source).as_posix(), path))
    return result


def pending_files(stage):
    result = []
    for category, extensions in EXTENSIONS.items():
        folder = stage / category
        if folder.is_dir():
            for path in sorted(folder.rglob('*')):
                if path.is_file() and path.suffix.lower() in extensions:
                    result.append((category, path.relative_to(folder).as_posix(), path))
    return result


def deploy(repo, settings, arguments, session=None, clock=time.time, build=subprocess.run):
    environment, full, dry_run, all_files = parse_args(arguments)
    base = api_base(settings, environment)
    checkpoint = repo / 'deploy' / ('last_run.' + environment + '.txt')
    legacy = repo / 'deploy/last_run.txt'
    cutoff = 0.0 if all_files else checkpoint_time(
        legacy if environment == 'prod' and not checkpoint.exists() and legacy.exists() else checkpoint)
    stage = repo / 'vault' / environment
    configuration = 'Integration' if environment == 'int' else 'Production2'
    started_at = clock()
    plan = asset_plan(settings, repo, full, cutoff)
    print('Target: ' + environment + ' (' + base + '); mode: ' + ('full' if full else 'assets'))
    if dry_run:
        print('Dry run: no build, network requests, staging writes, deployment or checkpoint changes.')
        for category, relative, _ in plan: print('Would stage: ' + category + '/' + relative)
        print('Pending staged files: ' + str(len(pending_files(stage))))
        if full: print('Would build ' + configuration + ', then include all bin DLL/PDBs and transformed web.config.')
        return
    key = settings.get('API_KEY')
    if not isinstance(key, str) or not key.strip(): raise DeploymentError('The local deployment API key is missing.')
    session = session or requests.Session()
    auth = {'Authorization': 'ApiKey ' + key}
    params = {'environment': environment}
    # Old servers must fail this read-only check BEFORE they can receive an upload.
    require_success(session.get(base + '/target', params=params, headers=auth,
        timeout=(15, 30), allow_redirects=False), environment)
    lock_path = repo / 'vault/.quick-deploy.lock'
    lock_path.parent.mkdir(parents=True, exist_ok=True)
    try: handle = os.open(str(lock_path), os.O_CREAT | os.O_EXCL | os.O_WRONLY)
    except FileExistsError as error: raise DeploymentError('Deployment lock exists; inspect the previous run before removing it.') from error
    os.close(handle)
    try:
        if full:
            build([settings['MSBUILD_PATH'], str(repo / 'webapp/WebApplication/WebApplication.csproj'),
                '/t:Build;TransformWebConfig', '/p:Configuration=' + configuration, '/p:LangVersion=7.3'], check=True)
            transformed = repo / 'webapp/WebApplication/obj' / configuration / 'TransformWebConfig/transformed/web.config'
            if not transformed.is_file(): raise DeploymentError('Transformed ' + configuration + ' web.config is missing.')
            binaries = sorted(path for path in Path(settings['SRC_BIN']).iterdir()
                if path.is_file() and path.suffix.lower() in EXTENSIONS['bin'])
            if not any(path.name == 'K9.WebApplication.dll' for path in binaries): raise DeploymentError('Built application DLL is missing.')
            plan = asset_plan(settings, repo, full, cutoff)
            plan.extend(('bin', path.name, path) for path in binaries)
            plan.append(('root', 'web.config', transformed))
        for category, relative, source in plan:
            destination = stage / category / relative
            destination.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(str(source), str(destination))
        pending = pending_files(stage)
        if not full and any(category in {'bin', 'root', 'config'} for category, _, _ in pending):
            raise DeploymentError('A previous full deployment is pending. Retry with full before an assets-only deployment.')
        for category, relative, source in pending:
            with source.open('rb') as stream:
                response = session.post(base + '/upload', params=params, headers=auth, data={'category': category},
                    files={'file': (relative, stream, 'application/octet-stream')}, timeout=(15, 180), allow_redirects=False)
            require_success(response, environment)
            print('Staged on server: ' + category + '/' + relative)
        require_success(session.post(base + '/run-maintenance', params=params, headers=auth,
            timeout=(15, 95), allow_redirects=False), environment)
        save_checkpoint(checkpoint, started_at)
        for _, _, source in pending: source.unlink()
        print('Deployment acknowledged successfully: ' + environment)
    finally:
        lock_path.unlink()


def main(arguments=None):
    arguments = sys.argv[1:] if arguments is None else arguments
    try:
        parse_args(arguments)
        repo = Path(__file__).resolve().parent.parent
        settings = runpy.run_path(str(repo / 'deploy/quick-deploy.py'), run_name='ninestar_deployment_settings')
        deploy(repo, settings, arguments)
        return 0
    except Exception as error:
        print('Deployment NOT confirmed: ' + str(error), file=sys.stderr)
        return 1


if __name__ == '__main__':
    sys.exit(main())
