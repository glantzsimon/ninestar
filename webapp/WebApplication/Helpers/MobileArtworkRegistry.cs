using K9.WebApplication.ViewModels.MobileApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace K9.WebApplication.Helpers
{
    // Only checked-in public artwork. No filesystem, database, identity or network access.
    public static partial class MobileArtworkRegistry
    {
        private sealed class ArtworkSource
        {
            public ArtworkSource(string id, string group, string variant, string path, string revision,
                string contentType, int width, int height)
            {
                Id = id; Group = group; Variant = variant; Path = path; Revision = revision;
                ContentType = contentType; Width = width; Height = height;
            }
            public string Id { get; }
            public string Group { get; }
            public string Variant { get; }
            public string Path { get; }
            public string Revision { get; }
            public string ContentType { get; }
            public int Width { get; }
            public int Height { get; }
        }

        // Initialise through a nested holder so partial-file field order cannot affect lookup creation.
        private static class Lookup
        {
            internal static readonly Dictionary<string, string> Ids = Sources.ToDictionary(
                e => e.Path, e => e.Id, StringComparer.Ordinal);
            internal static readonly Dictionary<string, string> PathsByRevision = Sources
                .GroupBy(e => e.Revision, StringComparer.Ordinal)
                .ToDictionary(e => e.Key, e => e.First().Path, StringComparer.Ordinal);
        }

        public static string IdForPath(string path)
        {
            string id;
            return path != null && Lookup.Ids.TryGetValue(path, out id) ? id : null;
        }

        public static MobileArtworkResponse Create(string baseImagesPath)
        {
            Uri baseUri;
            if (!Uri.TryCreate(baseImagesPath, UriKind.Absolute, out baseUri)
                || (baseUri.Scheme != Uri.UriSchemeHttps && baseUri.Scheme != Uri.UriSchemeHttp)
                || !string.IsNullOrEmpty(baseUri.Query) || !string.IsNullOrEmpty(baseUri.Fragment)
                || !string.IsNullOrEmpty(baseUri.UserInfo))
                throw new ArgumentException("An absolute HTTP media base is required.", nameof(baseImagesPath));

            var imageBase = baseUri.AbsoluteUri.TrimEnd('/') + "/";
            var assets = new Dictionary<string, MobileArtwork>(StringComparer.Ordinal);
            var versionInput = new StringBuilder("mobile-artwork-v1\n").Append(imageBase).Append('\n');
            foreach (var source in Sources)
            {
                MobileArtwork artwork;
                if (!assets.TryGetValue(source.Id, out artwork))
                {
                    artwork = new MobileArtwork { Group = source.Group,
                        Variants = new Dictionary<string, MobileArtworkVariant>(StringComparer.Ordinal) };
                    assets.Add(source.Id, artwork);
                }
                var url = new Uri(new Uri(imageBase), Lookup.PathsByRevision[source.Revision]).AbsoluteUri + "?v=" + source.Revision;
                artwork.Variants.Add(source.Variant, new MobileArtworkVariant
                {
                    Url = url, Revision = source.Revision, ContentType = source.ContentType,
                    Width = source.Width, Height = source.Height
                });
                versionInput.Append(source.Id).Append('|').Append(source.Group).Append('|')
                    .Append(source.Variant).Append('|').Append(url).Append('|')
                    .Append(source.ContentType).Append('|').Append(source.Width.ToString(CultureInfo.InvariantCulture)).Append('|').Append(source.Height.ToString(CultureInfo.InvariantCulture)).Append('\n');
            }
            return new MobileArtworkResponse { Version = Hash(versionInput.ToString()), Assets = assets };
        }

        private static string Hash(string value)
        {
            using (var sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(value)))
                    .Replace("-", string.Empty).ToLowerInvariant();
        }

        public static bool MatchesETag(string ifNoneMatch, string version)
        {
            if (string.IsNullOrWhiteSpace(ifNoneMatch)) return false;
            var expected = "\"" + version + "\"";
            return ifNoneMatch.Split(',').Any(item =>
            {
                var tag = item.Trim();
                if (tag == "*") return true;
                if (tag.StartsWith("W/", StringComparison.Ordinal)) tag = tag.Substring(2);
                return tag == expected;
            });
        }
    }
}
