using K9.WebApplication.Helpers;
using K9.SharedLibrary.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public partial class ApiController
    {
        private const string DeploymentProtocol = "NINESTAR_DEPLOY_V2";
        private static readonly object ProductionDeploymentLock = new object();
        private static readonly object IntegrationDeploymentLock = new object();

        private JsonResult DeploymentResponse(bool success, string environment, string error = null, int status = 200)
        {
            Response.StatusCode = status;
            Response.SuppressFormsAuthenticationRedirect = true;
            Response.TrySkipIisCustomErrors = true;
            Response.Cache.SetNoStore();
            return Json(new { success, environment, protocolVersion = 2, error }, JsonRequestBehavior.AllowGet);
        }

        private JsonResult ValidateDeployment(string environment, Func<string, JsonResult> action)
        {
            var header = Request.Headers[authRequestHeader];
            if (header == null || !header.StartsWith("ApiKey ", StringComparison.OrdinalIgnoreCase)
                || !IsValidApiKey(header)) return DeploymentResponse(false, null, "Invalid ApiKey", 401);
            string target;
            try { target = DeploymentTargetHelper.Environment(environment); }
            catch (ArgumentException) { return DeploymentResponse(false, null, "Unknown deployment environment", 400); }
            lock (target == "int" ? IntegrationDeploymentLock : ProductionDeploymentLock)
            {
                try
                {
                    var intBase = My.DefaultValuesConfiguration.SiteIntBaseUrl;
                    Uri intSite;
                    if (!string.IsNullOrWhiteSpace(intBase)
                        && Uri.TryCreate(intBase.Contains("://") ? intBase : "https://" + intBase, UriKind.Absolute, out intSite)
                        && string.Equals(Request.Url?.Host, intSite.Host, StringComparison.OrdinalIgnoreCase)
                        && target != "int")
                        return DeploymentResponse(false, target, "Integration host cannot deploy Production", 400);
                    return action(target);
                }
                catch (ArgumentException ex)
                {
                    My.Logger.Error("Deployment validation failed (" + target + "): " + ex.Message);
                    return DeploymentResponse(false, target, "Invalid deployment configuration or upload path", 400);
                }
                catch (Exception ex)
                {
                    My.Logger.Error("Deployment failed (" + target + "): " + ex.GetFullErrorMessage());
                    return DeploymentResponse(false, target, "Deployment operation failed; inspect server logs", 500);
                }
            }
        }

        private string DeploymentVault(string target) => DeploymentTargetHelper.Vault(target,
            My.DefaultValuesConfiguration.VaultPath, My.DefaultValuesConfiguration.VaultIntPath);

        private string DeploymentScript()
        {
            var root = DeploymentVault("prod");
            var name = My.DefaultValuesConfiguration.VaultDeployScript;
            if (string.IsNullOrWhiteSpace(name) || name != Path.GetFileName(name))
                throw new ArgumentException("Invalid deployment script name.");
            var path = Path.Combine(root, name);
            if (!System.IO.File.Exists(path) || !System.IO.File.ReadAllText(path).Contains(DeploymentProtocol))
                throw new ArgumentException("Install the matching deployment script before using this protocol.");
            return path;
        }

        [HttpGet, Route("deploy/target")]
        public JsonResult DeploymentTarget(string environment = "prod") => ValidateDeployment(environment, target =>
        {
            DeploymentVault(target);
            DeploymentScript();
            return DeploymentResponse(true, target);
        });

        [HttpPost, Route("deploy/upload")]
        public JsonResult UploadFile(HttpPostedFileBase file, string environment = "prod", string category = null)
        {
            return ValidateDeployment(environment, target =>
            {
                if (file == null || file.ContentLength < 0)
                    return DeploymentResponse(false, target, "No file uploaded", 400);
                var destination = DeploymentTargetHelper.UploadPath(DeploymentVault(target), file.FileName, category);
                // Empty text assets can be intentional; empty binaries/config/media cannot.
                if (file.ContentLength == 0 && !new[] { ".css", ".js", ".cshtml" }
                    .Contains(Path.GetExtension(file.FileName), StringComparer.OrdinalIgnoreCase))
                    return DeploymentResponse(false, target, "Empty binary, configuration or media upload", 400);
                Directory.CreateDirectory(Path.GetDirectoryName(destination));
                var temporary = destination + "." + Guid.NewGuid().ToString("N") + ".upload";
                try
                {
                    file.SaveAs(temporary);
                    if (System.IO.File.Exists(destination)) System.IO.File.Replace(temporary, destination, null);
                    else System.IO.File.Move(temporary, destination);
                }
                finally { if (System.IO.File.Exists(temporary)) System.IO.File.Delete(temporary); }
                return DeploymentResponse(true, target);
            });
        }

        [HttpPost, Route("deploy/run-maintenance")]
        public JsonResult RunMaintenanceScript(string environment = "prod") => ValidateDeployment(environment, target =>
        {
            var script = DeploymentScript();
            var vault = DeploymentVault(target);
            var output = new StringBuilder();
            var errors = new StringBuilder();
            using (var process = new Process())
            {
                // Paths come from administrator-owned configuration, never request strings.
                if (script.Contains("\"") || vault.Contains("\"")) throw new ArgumentException("Invalid configured path.");
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-NoProfile -NonInteractive -ExecutionPolicy Bypass -File \"" + script
                        + "\" -Environment " + target + " -SourceRoot \"" + vault + "\"",
                    UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true
                };
                process.OutputDataReceived += (sender, args) => { if (args.Data != null) output.AppendLine(args.Data); };
                process.ErrorDataReceived += (sender, args) => { if (args.Data != null) errors.AppendLine(args.Data); };
                process.Start(); process.BeginOutputReadLine(); process.BeginErrorReadLine();
                if (!process.WaitForExit(60000))
                {
                    process.Kill(); process.WaitForExit();
                    My.Logger.Error("Deployment script timed out; server state requires inspection.");
                    return DeploymentResponse(false, target, "Deployment timed out; inspect server state before retrying", 504);
                }
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    My.Logger.Error("Deployment script failed: " + errors);
                    return DeploymentResponse(false, target, "Server copy or restart failed; inspect server logs", 500);
                }
            }
            return DeploymentResponse(true, target);
        });
    }
}
