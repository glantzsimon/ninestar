import contextlib
import importlib.util
import inspect
import io
import json
import os
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch

script = Path(os.environ.get('NINESTAR_QUICK_DEPLOY_TEST_SOURCE',
    str(Path(__file__).resolve().parents[2] / 'deploy/quick-deploy.py')))


class Response:
    def __init__(self, body=None, status=200):
        self.status_code = status
        self.body = body
    def json(self):
        if isinstance(self.body, Exception):
            raise self.body
        return self.body


class Session:
    def __init__(self, environment='prod', fail_upload=False, fail_maintenance=False, preflight=None):
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
        self.get_params = kwargs['params']
        return self.preflight if self.preflight is not None else self.success()
    def post(self, url, **kwargs):
        if url.endswith('/upload'):
            self.uploads.append((kwargs['files']['file'][0], kwargs['files']['file'][1], kwargs['params']))
            if self.fail_upload:
                return Response({'success': False})
        else:
            self.maintenance += 1
            if self.fail_maintenance:
                return Response({'success': False})
        return self.success()


class QuickDeployTests(unittest.TestCase):
    def setUp(self):
        spec = importlib.util.spec_from_file_location('quick_deploy_test_source', script)
        self.q = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(self.q)
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        (self.root / 'deploy').mkdir()
        self.q.PROJECT_ROOT = self.root
        self.q.API_KEY = 'test-only-not-a-secret'
        self.q.API_ENDPOINT = 'https://prod.example/api/deploy/upload'
        self.q.MAINTENANCE_ENDPOINT = 'https://prod.example/api/deploy/run-maintenance'
        self.q.LAST_RUN_FILE = str(self.root / 'deploy/last_run.txt')
        for key, folder in [('SRC_CSS', 'Content'), ('SRC_SCRIPTS', 'Scripts'), ('SRC_VIEWS', 'Views'),
                            ('SRC_IMAGES', 'Images'), ('SRC_VIDEOS', 'Videos'), ('SRC_CONFIG', 'Config'), ('SRC_BIN', 'bin')]:
            path = self.root / 'webapp/WebApplication' / folder
            path.mkdir(parents=True)
            setattr(self.q, key, str(path))
        for key, folder in [('VAULT_BIN', 'bin'), ('VAULT_CONFIG', 'config'), ('VAULT_VIEWS', 'views'),
                            ('VAULT_CSS', 'css'), ('VAULT_SCRIPTS', 'scripts'), ('VAULT_IMAGES', 'images'), ('VAULT_VIDEOS', 'videos')]:
            setattr(self.q, key, str(self.root / 'vault' / folder))
        self.q.VAULT_WEBCONFIG = str(self.root / 'vault/web.config')
        Path(self.q.SRC_CONFIG, 'appsettings.json').write_text(json.dumps({'DefaultValues': {'SiteIntBaseUrl': 'int.example'}}))
        Path(self.q.SRC_CSS, 'site.css').write_text('css')
        self.q.FILES_TO_COPY = ['App.dll', 'App.pdb']
    def run_main(self, args=(), session=None, result=0):
        session = session or Session()
        with contextlib.redirect_stdout(io.StringIO()), patch.object(self.q.sys, 'argv', ['quick-deploy.py'] + list(args)), \
             patch.object(self.q.requests, 'Session', return_value=session), \
             patch.object(self.q.subprocess, 'call', return_value=result) as transform, \
             patch.object(self.q.time, 'time', return_value=1234):
            self.q.main()
        return session, transform
    def prepare_full(self, environment):
        configuration = 'Integration' if environment == 'int' else 'Production2'
        path = self.root / 'webapp/WebApplication/obj' / configuration / 'TransformWebConfig/transformed/web.config'
        path.parent.mkdir(parents=True)
        path.write_text(configuration)
        for name in self.q.FILES_TO_COPY:
            Path(self.q.SRC_BIN, name).write_bytes(b'binary')
    def test_production_defaults_are_preserved(self):
        endpoint = self.q.API_ENDPOINT
        vault = self.q.VAULT_BIN
        checkpoint = self.q.LAST_RUN_FILE
        self.assertEqual(self.q.configure_environment([]), (False, False))
        self.assertEqual(self.q.API_ENDPOINT, endpoint)
        self.assertEqual(self.q.VAULT_BIN, vault)
        self.assertEqual(self.q.LAST_RUN_FILE, checkpoint)
    def test_integration_selects_separate_paths_and_normalizes_url(self):
        self.assertEqual(self.q.configure_environment(['full', 'int']), (True, False))
        self.assertEqual(self.q.API_ENDPOINT, 'https://int.example/api/deploy/upload')
        self.assertEqual(Path(self.q.VAULT_BIN), self.root / 'vault/int/bin')
        self.assertEqual(Path(self.q.LAST_RUN_FILE), self.root / 'deploy/last_run.int.txt')
        self.assertIn('Integration', self.q.SRC_WEBCONFIG)
    def test_unknown_or_conflicting_arguments_are_rejected(self):
        for arguments in [['inte'], ['prod', 'int'], ['full', 'full']]:
            with self.assertRaises(RuntimeError):
                self.q.configure_environment(arguments)
    def test_missing_integration_url_does_not_fall_back_to_production(self):
        Path(self.q.SRC_CONFIG, 'appsettings.json').write_text('{}')
        with self.assertRaises(RuntimeError):
            self.q.configure_environment(['int'])
    def test_ambiguous_or_insecure_integration_url_is_rejected(self):
        for document in [{'A': {'SiteIntBaseUrl': 'a'}, 'B': {'SiteIntBaseUrl': 'b'}},
                         {'SiteIntBaseUrl': 'http://int.example'}, {'SiteIntBaseUrl': 'https://user:pass@int.example'}]:
            Path(self.q.SRC_CONFIG, 'appsettings.json').write_text(json.dumps(document))
            with self.assertRaises(RuntimeError):
                self.q.configure_environment(['int'])
    def test_unsuccessful_and_malformed_acknowledgements_fail(self):
        for response in [Response({'success': False}), Response({'success': 'true'}), Response(ValueError()),
                         Response({'success': True}), Response([], 200), Response({}, 500),
                         Response({'success': True, 'environment': 'int', 'protocolVersion': 2})]:
            with self.assertRaises(RuntimeError):
                self.q.require_deployment_success(response)
    def test_successful_upload_retains_local_file_until_maintenance(self):
        path = Path(self.q.SRC_CSS) / 'site.css'
        with contextlib.redirect_stdout(io.StringIO()):
            self.q.upload_file(str(path), self.q.SRC_CSS, Session())
        self.assertTrue(path.exists())
        self.assertEqual(self.q.UPLOADED_FILES, [str(path)])
    def test_old_server_preflight_stops_before_any_upload(self):
        session = Session(preflight=Response({}, 404))
        with self.assertRaises(RuntimeError):
            self.run_main(session=session)
        self.assertEqual(session.uploads, [])
        self.assertFalse((self.root / 'vault').exists())
    def test_upload_failure_preserves_staging_and_checkpoint(self):
        session = Session(fail_upload=True)
        with self.assertRaises(RuntimeError):
            self.run_main(session=session)
        self.assertEqual(session.maintenance, 0)
        self.assertTrue((self.root / 'vault/css/site.css').exists())
        self.assertFalse(Path(self.q.LAST_RUN_FILE).exists())
    def test_maintenance_failure_preserves_staging_and_checkpoint(self):
        with self.assertRaises(RuntimeError):
            self.run_main(session=Session(fail_maintenance=True))
        self.assertTrue((self.root / 'vault/css/site.css').exists())
        self.assertFalse(Path(self.q.LAST_RUN_FILE).exists())
    def test_successful_run_updates_start_time_and_consumes_acknowledged_staging(self):
        session, _ = self.run_main()
        self.assertEqual(Path(self.q.LAST_RUN_FILE).read_text(), '1234')
        self.assertEqual(session.maintenance, 1)
        self.assertFalse((self.root / 'vault/css/site.css').exists())
    def test_integration_does_not_advance_production_checkpoint(self):
        production = Path(self.q.LAST_RUN_FILE)
        production.write_text('10')
        session, _ = self.run_main(args=['int'], session=Session('int'))
        self.assertEqual(production.read_text(), '10')
        self.assertEqual(Path(self.q.LAST_RUN_FILE).read_text(), '1234')
        self.assertEqual(session.get_params, {'environment': 'int'})
        self.assertTrue(all(item[2] == {'environment': 'int'} for item in session.uploads))
    def test_dry_run_has_no_network_or_file_writes(self):
        with patch.object(self.q.requests, 'Session', side_effect=AssertionError('network')):
            with contextlib.redirect_stdout(io.StringIO()), patch.object(self.q.sys, 'argv', ['quick-deploy.py', 'full', 'int', '--dry-run']):
                self.q.main()
        self.assertFalse((self.root / 'vault').exists())
        self.assertFalse(Path(self.q.LAST_RUN_FILE).exists())
    def test_failed_transform_stops_before_upload(self):
        session = Session('int')
        with self.assertRaises(RuntimeError):
            self.run_main(args=['full', 'int'], session=session, result=1)
        self.assertEqual(session.uploads, [])
        self.assertEqual(session.maintenance, 0)
        self.assertFalse(Path(self.q.LAST_RUN_FILE).exists())
    def test_missing_full_binary_stops_before_upload(self):
        self.prepare_full('int')
        Path(self.q.SRC_BIN, 'App.dll').unlink()
        session = Session('int')
        with self.assertRaises(RuntimeError):
            self.run_main(args=['full', 'int'], session=session)
        self.assertEqual(session.uploads, [])
    def test_full_retains_transform_only_behavior_and_existing_binary_list(self):
        self.prepare_full('int')
        Path(self.q.SRC_BIN, 'Unrequested.dll').write_bytes(b'unused')
        session, transform = self.run_main(args=['full', 'int'], session=Session('int'))
        arguments = transform.call_args[0][0]
        self.assertIn('/t:TransformWebConfig', arguments)
        self.assertIn('/p:Configuration=Integration', arguments)
        names = [item[0] for item in session.uploads]
        self.assertIn('App.dll', names)
        self.assertNotIn('Unrequested.dll', names)
        self.assertIn('web.config', names)
    def test_production_full_uses_production_transform(self):
        self.prepare_full('prod')
        _, transform = self.run_main(args=['full'])
        self.assertIn('/p:Configuration=Production2', transform.call_args[0][0])
    def test_views_config_alias_is_retained(self):
        Path(self.q.SRC_VIEWS, 'web.config').write_text('views config')
        session, _ = self.run_main()
        self.assertIn('views.config', [item[0] for item in session.uploads])
    def test_storj_helper_and_dormant_call_are_retained(self):
        self.assertTrue(callable(self.q.upload_media))
        self.assertIn('#                        upload_media(path)', inspect.getsource(self.q.main))
    def test_maintenance_timeout_is_not_reported_as_success(self):
        session = Session()
        def fail(*args, **kwargs):
            raise self.q.requests.exceptions.Timeout('timeout')
        session.post = fail
        with contextlib.redirect_stdout(io.StringIO()), self.assertRaises(self.q.requests.exceptions.Timeout):
            self.q.run_maintenance(session)
    def test_assets_mode_cannot_consume_pending_full_deployment(self):
        path = Path(self.q.VAULT_BIN)
        path.mkdir(parents=True)
        (path / 'Pending.dll').write_bytes(b'pending')
        session = Session()
        with self.assertRaises(RuntimeError):
            self.run_main(session=session)
        self.assertEqual(session.uploads, [])
        self.assertTrue((path / 'Pending.dll').exists())


if __name__ == '__main__':
    unittest.main()
