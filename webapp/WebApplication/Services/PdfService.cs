using K9.WebApplication.Packages;
using System;
using System.Diagnostics;
using System.IO;
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
            var tempDir = Path.Combine(Path.GetTempPath(), "wkhtmltopdf");
            Directory.CreateDirectory(tempDir);

            var pdfPath = Path.Combine(tempDir, Guid.NewGuid().ToString("N") + ".pdf");

            try
            {
                var args =
                    "--quiet " +
                    "--print-media-type " +
                    "--margin-top 12mm --margin-bottom 12mm --margin-left 12mm --margin-right 12mm " +
                    $"\"{url}\" \"{pdfPath}\"";

                var psi = new ProcessStartInfo
                {
                    FileName = My.DefaultValuesConfiguration.WkHtmlToPdfExePath,
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

                    var stderr = p.StandardError.ReadToEnd();
                    var stdout = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();

                    if (p.ExitCode != 0)
                        throw new Exception("wkhtmltopdf failed. " + stderr + " " + stdout);
                }

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