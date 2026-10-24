import contextlib
import importlib.util
import io
import json
import os
from pathlib import Path
import subprocess
import tempfile
import unittest

script = Path(os.environ.get('NINESTAR_QUICK_DEPLOY_TEST_SOURCE',
    str(Path(__file__).resolve().parents[2] / 'deploy/quick-deploy.py')))
spec = importlib.util.spec_from_file_location('quick_deploy', script)
q = importlib.util.module_from_spec(spec)
spec.loader.exec_module(q)


class Response:
    def __init__(self, body=None, status=200):
        self.status_code = status
        self.body = body
    def json(self):
        if isinstance(self.body, Exception): raise self.body
        return self.body


class Session:
    def __init__(self, environment='int', fail_upload=None, fail_maintenance=False, preflight=None):
        self.environment = environment
        self.fail_upload = fail_upload
        self.fail_maintenance = fail_maintenance
        self.preflight = preflight
        self.uploads = []
        self.maintenance = 0
    def success(self):
        return Response({'success': True, 'environment': self.environment, 'protocolVersion': 2})
    def get(self, url, **kwargs):
        self.get_url = url
        return self.preflight if self.preflight is not None else self.success()
    def post(self, url, **kwargs):
        if url.endswith('/upload'):
            relative, stream, _ = kwargs['files']['file']
            self.uploads.append((kwargs['data']['category'], relative, stream.read(), kwargs['params']['environment']))
            if self.fail_upload == len(self.uploads): return Response({'success': False}, 200)
        else:
            self.maintenance += 1
            if self.fail_maintenance: return Response({'success': False}, 200)
        return self.success()


class QuickDeployTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.repo = Path(self.temp.name)
        (self.repo / 'deploy').mkdir()
        self.settings = {'API_ENDPOINT': 'https://prod.example/api/deploy/upload', 'API_KEY': 'test-only-not-a-secret',
            'MSBUILD_PATH': 'msbuild', 'EXCLUDED_FOLDERS': ('less',), 'EXCLUDED_VIEWS': ('_ViewStart.cshtml',)}
        for key, folder in [('SRC_CSS', 'Content'), ('SRC_SCRIPTS', 'Scripts'), ('SRC_VIEWS', 'Views'),
            ('SRC_IMAGES', 'Images'), ('SRC_CONFIG', 'Config'), ('SRC_BIN', 'bin')]:
            path = self.repo / 'webapp/WebApplication' / folder
            path.mkdir(parents=True)
            self.settings[key] = str(path)
        Path(self.settings['SRC_CONFIG'], 'appsettings.json').write_text(json.dumps({'DefaultValues': {'SiteIntBaseUrl': 'int.example'}}))
        Path(self.settings['SRC_CSS'], 'site.css').write_text('css')
    def run_deploy(self, args=('int',), session=None, **kwargs):
        with contextlib.redirect_stdout(io.StringIO()):
            return q.deploy(self.repo, self.settings, list(args), session=session or Session(), clock=lambda: 1234, **kwargs)
    def test_arguments_are_order_independent_and_reject_unknown_targets(self):
        self.assertEqual(q.parse_args(['full', 'int']), q.parse_args(['int', 'full']))
        self.assertEqual(q.parse_args([]), ('prod', False, False, False))
        for args in [['inte'], ['prod', 'int'], ['full', 'full']]:
            with self.assertRaises(q.DeploymentError): q.parse_args(args)
    def test_url_normalization_and_environment_selection(self):
        self.assertEqual(q.api_base(self.settings, 'int'), 'https://int.example/api/deploy')
        self.assertEqual(q.api_base(self.settings, 'prod'), 'https://prod.example/api/deploy')
        for value in ['http://int.example', 'https://user:pass@int.example', 'https://int.example?secret=1']:
            with self.assertRaises(q.DeploymentError): q.site_url(value)
    def test_http_200_false_and_malformed_success_are_rejected(self):
        for response in [Response({'success': False}), Response({'success': 'true'}), Response(ValueError()),
            Response({'success': True, 'environment': 'prod', 'protocolVersion': 2}), Response({}, 500), Response({}, 302),
            Response({'success': True, 'environment': 'int'})]:
            with self.assertRaises(q.DeploymentError): q.require_success(response, 'int')
    def test_old_server_is_rejected_before_upload(self):
        session = Session(preflight=Response({}, 404))
        with self.assertRaises(q.DeploymentError): self.run_deploy(session=session)
        self.assertEqual(session.uploads, [])
        self.assertEqual(session.maintenance, 0)
        self.assertFalse((self.repo / 'vault').exists())
    def test_upload_failure_retains_staging_and_checkpoint_and_skips_maintenance(self):
        checkpoint = self.repo / 'deploy/last_run.int.txt'
        checkpoint.write_text('1')
        session = Session(fail_upload=1)
        with self.assertRaises(q.DeploymentError): self.run_deploy(session=session)
        self.assertEqual(checkpoint.read_text(), '1')
        self.assertEqual(session.maintenance, 0)
        self.assertTrue((self.repo / 'vault/int/css/site.css').exists())
    def test_maintenance_failure_does_not_claim_completion(self):
        session = Session(fail_maintenance=True)
        with self.assertRaises(q.DeploymentError): self.run_deploy(session=session)
        self.assertTrue((self.repo / 'vault/int/css/site.css').exists())
        self.assertFalse((self.repo / 'deploy/last_run.int.txt').exists())
    def test_success_cleans_staging_and_sets_start_time_only_for_selected_environment(self):
        prod = self.repo / 'deploy/last_run.prod.txt'
        prod.write_text('12')
        session = Session()
        self.run_deploy(session=session)
        self.assertEqual((self.repo / 'deploy/last_run.int.txt').read_text(), '1234')
        self.assertEqual(prod.read_text(), '12')
        self.assertFalse((self.repo / 'vault/int/css/site.css').exists())
        self.assertEqual(session.maintenance, 1)
    def test_retry_resends_retained_files(self):
        with self.assertRaises(q.DeploymentError): self.run_deploy(session=Session(fail_maintenance=True))
        session = Session()
        self.run_deploy(session=session)
        self.assertEqual(len(session.uploads), 1)
    def test_dry_run_has_no_network_build_or_writes(self):
        class Forbidden:
            def get(self, *args, **kwargs): raise AssertionError('network')
        self.run_deploy(args=('full', 'int', '--dry-run'), session=Forbidden(), build=lambda *a, **k: self.fail('build'))
        self.assertFalse((self.repo / 'vault').exists())
        self.assertFalse((self.repo / 'deploy/last_run.int.txt').exists())
    def test_failed_build_stops_before_upload_and_config_copy(self):
        session = Session()
        def failed(*args, **kwargs): raise subprocess.CalledProcessError(1, 'msbuild')
        with self.assertRaises(subprocess.CalledProcessError): self.run_deploy(args=('full', 'int'), session=session, build=failed)
        self.assertEqual(session.uploads, [])
        self.assertEqual(session.maintenance, 0)
        self.assertFalse((self.repo / 'deploy/last_run.int.txt').exists())
    def test_full_build_selects_integration_and_uploads_nested_configs_and_dependencies(self):
        nested = Path(self.settings['SRC_CONFIG']) / 'nested'
        nested.mkdir()
        (nested / 'extra.json').write_text('{}')
        def build(args, **kwargs):
            self.assertIn('/p:Configuration=Integration', args)
            self.assertTrue(kwargs['check'])
            transformed = self.repo / 'webapp/WebApplication/obj/Integration/TransformWebConfig/transformed/web.config'
            transformed.parent.mkdir(parents=True)
            transformed.write_text('integration-only')
            for name in ['K9.WebApplication.dll', 'Dependency.dll']:
                Path(self.settings['SRC_BIN'], name).write_text(name)
        session = Session()
        self.run_deploy(args=('full', 'int'), session=session, build=build)
        self.assertIn(('root', 'web.config', b'integration-only', 'int'), session.uploads)
        self.assertIn(('config', 'nested/extra.json', b'{}', 'int'), session.uploads)
        self.assertTrue(any(item[1] == 'Dependency.dll' for item in session.uploads))
    def test_existing_lock_blocks_upload_and_is_not_removed(self):
        lock = self.repo / 'vault/.quick-deploy.lock'
        lock.parent.mkdir()
        lock.write_text('previous')
        session = Session()
        with self.assertRaises(q.DeploymentError): self.run_deploy(session=session)
        self.assertEqual(lock.read_text(), 'previous')
        self.assertEqual(session.uploads, [])
    def test_videos_use_actual_videos_folder(self):
        folder = self.repo / 'webapp/WebApplication/Videos'
        folder.mkdir()
        (folder / 'test.mp4').write_bytes(b'video')
        session = Session()
        self.run_deploy(session=session)
        self.assertIn(('videos', 'test.mp4', b'video', 'int'), session.uploads)
    def test_production_keeps_legacy_checkpoint_and_never_uses_integration_target(self):
        legacy = self.repo / 'deploy/last_run.txt'
        legacy.write_text('0')
        session = Session(environment='prod')
        self.run_deploy(args=(), session=session)
        self.assertEqual(session.get_url, 'https://prod.example/api/deploy/target')
        self.assertTrue(all(item[3] == 'prod' for item in session.uploads))
        self.assertEqual(legacy.read_text(), '0')
        self.assertEqual((self.repo / 'deploy/last_run.prod.txt').read_text(), '1234')
        self.assertFalse((self.repo / 'vault/int').exists())
    def test_network_timeout_retains_staging_without_maintenance(self):
        class TimeoutSession(Session):
            def post(self, *args, **kwargs): raise q.requests.Timeout('timeout')
        session = TimeoutSession()
        with self.assertRaises(q.requests.Timeout): self.run_deploy(session=session)
        self.assertEqual(session.maintenance, 0)
        self.assertTrue((self.repo / 'vault/int/css/site.css').exists())
        self.assertFalse((self.repo / 'deploy/last_run.int.txt').exists())
    def test_full_production_selects_production2_transform(self):
        def build(args, **kwargs):
            self.assertIn('/p:Configuration=Production2', args)
            transformed = self.repo / 'webapp/WebApplication/obj/Production2/TransformWebConfig/transformed/web.config'
            transformed.parent.mkdir(parents=True)
            transformed.write_text('production-only')
            Path(self.settings['SRC_BIN'], 'K9.WebApplication.dll').write_text('binary')
        session = Session(environment='prod')
        self.run_deploy(args=('full',), session=session, build=build)
        self.assertIn(('root', 'web.config', b'production-only', 'prod'), session.uploads)

    def test_ambiguous_or_missing_site_setting_is_rejected(self):
        for settings in [{}, {'A': {'SiteIntBaseUrl': 'a'}, 'B': {'SiteIntBaseUrl': 'b'}}]:
            with self.assertRaises(q.DeploymentError): q.find_setting(settings, 'SiteIntBaseUrl')


if __name__ == '__main__': unittest.main()
