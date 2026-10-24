using System;
using System.IO;
using System.Linq;

namespace K9.WebApplication.Helpers
{
    public static class DeploymentTargetHelper
    {
        public static string Environment(string environment)
        {
            if (environment == null) return "prod";
            switch (environment.Trim().ToLowerInvariant())
            {
                case "prod": return "prod";
                case "int": return "int";
                default: throw new ArgumentException("Unknown deployment environment.");
            }
        }

        public static string Vault(string environment, string production, string integration)
        {
            var value = Environment(environment) == "int" ? integration : production;
            if (string.IsNullOrWhiteSpace(value) || !Path.IsPathRooted(value))
                throw new ArgumentException("The deployment vault must be configured as an absolute path.");
            var result = Path.GetFullPath(value).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (Environment(environment) == "int" && string.Equals(result,
                Path.GetFullPath(production).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Integration requires its own vault.");
            return result;
        }

        public static string UploadPath(string vault, string name, string category)
        {
            if (string.IsNullOrWhiteSpace(name) || Path.IsPathRooted(name) || name.Contains(":"))
                throw new ArgumentException("Invalid upload path.");
            var parts = name.Replace('\\', '/').Split('/');
            if (parts.Any(p => string.IsNullOrWhiteSpace(p) || p == "." || p == ".."
                || p.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0))
                throw new ArgumentException("Invalid upload path.");
            var extension = Path.GetExtension(name).ToLowerInvariant();
            if (string.IsNullOrEmpty(category))
            {
                switch (extension)
                {
                    case ".dll": case ".pdb": category = "bin"; break;
                    case ".json": category = "config"; break;
                    case ".cshtml": category = "views"; break;
                    case ".css": category = "css"; break;
                    case ".js": category = "scripts"; break;
                    case ".png": case ".jpg": case ".jpeg": case ".gif": category = "images"; break;
                    case ".mp4": case ".webm": category = "videos"; break;
                    case ".config":
                        category = name.Equals("views.config", StringComparison.OrdinalIgnoreCase) ? "views" : "root";
                        if (category == "views") name = "web.config";
                        break;
                    default: throw new ArgumentException("Unsupported file type.");
                }
            }
            string[] allowed;
            switch (category)
            {
                case "bin": allowed = new[] { ".dll", ".pdb" }; break;
                case "config": allowed = new[] { ".json" }; break;
                case "views": allowed = new[] { ".cshtml", ".config" }; break;
                case "css": allowed = new[] { ".css" }; break;
                case "scripts": allowed = new[] { ".js" }; break;
                case "images": allowed = new[] { ".png", ".jpg", ".jpeg", ".gif" }; break;
                case "videos": allowed = new[] { ".mp4", ".webm" }; break;
                case "root": allowed = new[] { ".config" }; break;
                default: throw new ArgumentException("Unsupported upload category.");
            }
            if (!allowed.Contains(extension)) throw new ArgumentException("File type does not match its category.");
            var root = Path.GetFullPath(category == "root" ? vault : Path.Combine(vault, category));
            var result = Path.GetFullPath(Path.Combine(root, name.Replace('/', Path.DirectorySeparatorChar)));
            if (!result.StartsWith(root.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar,
                StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("Invalid upload path.");
            return result;
        }
    }
}
