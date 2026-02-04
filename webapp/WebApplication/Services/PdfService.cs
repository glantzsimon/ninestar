using K9.WebApplication.Packages;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Text;

namespace K9.WebApplication.Services
{
    public class PdfService : BaseService, IPdfService
    {
        public PdfService(INineStarKiBasePackage my)
            : base(my)
        {
        }

        public byte[] HtmlToPdf(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                throw new ArgumentException("HTML is required.", nameof(html));

            if (string.IsNullOrWhiteSpace(My.DefaultValuesConfiguration.WkHtmlToPdfExePath) || !File.Exists(My.DefaultValuesConfiguration.WkHtmlToPdfExePath))
                throw new FileNotFoundException("wkhtmltopdf.exe not found.", My.DefaultValuesConfiguration.WkHtmlToPdfExePath);

            // Temp files
            var tempDir = Path.Combine(Path.GetTempPath(), "wkhtmltopdf");
            Directory.CreateDirectory(tempDir);

            var htmlPath = Path.Combine(tempDir, Guid.NewGuid().ToString("N") + ".html");
            var pdfPath = Path.Combine(tempDir, Guid.NewGuid().ToString("N") + ".pdf");

            try
            {
                // Write UTF-8 HTML
                File.WriteAllText(htmlPath, html, new UTF8Encoding(false));

                // Important flags:
                // --encoding utf-8: consistent character handling
                // --print-media-type: uses print CSS if present (harmless if not)
                // --disable-smart-shrinking: often improves layout stability for tables
                // --enable-local-file-access: only needed if you reference local file paths (you said public images, so optional)
                var args =
                    "--quiet " +
                    "--encoding utf-8 " +
                    "--print-media-type " +
                    "--disable-smart-shrinking " +
                    "--margin-top 12mm --margin-bottom 12mm --margin-left 12mm --margin-right 12mm " +
                    $"\"{htmlPath}\" \"{pdfPath}\"";

                var psi = new ProcessStartInfo
                {
                    FileName = My.DefaultValuesConfiguration.WkHtmlToPdfExePath,
                    WorkingDirectory = Path.GetDirectoryName(My.DefaultValuesConfiguration.WkHtmlToPdfExePath),
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };

                using (var p = new Process { StartInfo = psi })
                {
                    try
                    {
                        p.Start();
                    }
                    catch (System.ComponentModel.Win32Exception ex)
                    {
                        throw new Exception(
                            "wkhtmltopdf failed to start." + Environment.NewLine +
                            "Exe: " + psi.FileName + Environment.NewLine +
                            "WorkingDir: " + psi.WorkingDirectory + Environment.NewLine +
                            "Args: " + psi.Arguments + Environment.NewLine +
                            "Identity: " + System.Security.Principal.WindowsIdentity.GetCurrent().Name + Environment.NewLine +
                            "NativeErrorCode: " + ex.NativeErrorCode + Environment.NewLine +
                            "Message: " + ex.Message,
                            ex);
                    }


                    // Read streams to avoid deadlocks in edge cases
                    var stderr = p.StandardError.ReadToEnd();
                    var stdout = p.StandardOutput.ReadToEnd();

                    p.WaitForExit();

                    if (p.ExitCode != 0)
                    {
                        throw new Exception(
                            "wkhtmltopdf failed. ExitCode=" + p.ExitCode + System.Environment.NewLine +
                            "STDERR: " + stderr + System.Environment.NewLine +
                            "STDOUT: " + stdout
                        );
                    }
                }

                if (!File.Exists(pdfPath))
                    throw new Exception("wkhtmltopdf did not produce a PDF file.");

                return File.ReadAllBytes(pdfPath);
            }
            finally
            {
                SafeDelete(htmlPath);
                SafeDelete(pdfPath);
            }
        }

        public byte[] UrlToPdf(string url)
        {
            // Folder for temp PDFs
            var tempDir = Path.Combine(Path.GetTempPath(), "html2pdf");
            Directory.CreateDirectory(tempDir);

            var pdfPath = Path.Combine(tempDir, Guid.NewGuid().ToString("N") + ".pdf");
            var scriptPath = System.Web.Hosting.HostingEnvironment.MapPath(
                "~/Scripts/node/htmlToPdf.js"
            );

            if (string.IsNullOrEmpty(scriptPath) || !File.Exists(scriptPath))
            {
                throw new FileNotFoundException("htmlToPdf.js not found", scriptPath);
            }

            try
            {
                // Args: "<script>" "<url>" "<outputPdf>"
                // Use quotes to protect spaces
                var args = $"\"{scriptPath}\" \"{url}\" \"{pdfPath}\"";

                var psi = new ProcessStartInfo
                {
                    FileName = "node",
                    WorkingDirectory = Path.GetDirectoryName(scriptPath) ?? Environment.CurrentDirectory,
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };

                using (var p = new Process { StartInfo = psi })
                {
                    try
                    {
                        p.Start();
                    }
                    catch (Win32Exception ex)
                    {
                        throw new Exception(
                            "Node/Puppeteer PDF process failed to start." + Environment.NewLine +
                            "Exe: " + psi.FileName + Environment.NewLine +
                            "WorkingDir: " + psi.WorkingDirectory + Environment.NewLine +
                            "Args: " + psi.Arguments + Environment.NewLine +
                            "Identity: " + WindowsIdentity.GetCurrent().Name + Environment.NewLine +
                            "NativeErrorCode: " + ex.NativeErrorCode + Environment.NewLine +
                            "Message: " + ex.Message,
                            ex);
                    }

                    var stdout = p.StandardOutput.ReadToEnd();
                    var stderr = p.StandardError.ReadToEnd();
                    p.WaitForExit();

                    if (p.ExitCode != 0)
                    {
                        throw new Exception(
                            "Node/Puppeteer PDF process failed." + Environment.NewLine +
                            "ExitCode: " + p.ExitCode + Environment.NewLine +
                            "STDERR: " + stderr + Environment.NewLine +
                            "STDOUT: " + stdout);
                    }
                }

                if (!File.Exists(pdfPath))
                    throw new Exception("Node/Puppeteer reported success but the PDF file was not created: " + pdfPath);

                return File.ReadAllBytes(pdfPath);
            }
            finally
            {
                try { if (File.Exists(pdfPath)) File.Delete(pdfPath); } catch { }
            }
        }

        private static void SafeDelete(string path)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                    File.Delete(path);
            }
            catch
            {
                // swallow cleanup errors
            }
        }
    }
}