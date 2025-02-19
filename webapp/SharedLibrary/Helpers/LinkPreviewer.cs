using K9.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using WebGrease.Css.Extensions;

namespace K9.SharedLibrary.Helpers
{
    public class LinkPreviewer : ILinkPreviewer
    {
        private const int MinimumImageDimension = 50;
        private List<string> _imageUrls;
        private string _url;

        public ILinkPreviewResult GetPreview(string url, int imageMinimumWidth = 400, int imageMinimumHeight = 300)
        {
            url = url.ToLower();
            url = url.StartsWith("http") ? url : $"http://{url}";
            _url = url.Trim();

            if (url.StartsWith("https"))
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            }

            var imageUrl = "";
            var title = "";
            var description = "";
            var html = GetHtml(url);
            var metadata = GetWebPageMetaData(html);
            _imageUrls = new List<string>();

            if (metadata != null)
            {
                foreach (Match item in metadata)
                {
                    for (int i = 0; i <= item.Groups.Count; i++)
                    {
                        if (item.Groups[i].Value.ToLower().Contains("description"))
                        {
                            description = item.Groups[i + 1].Value;
                            break;
                        }

                        if (item.Groups[i].Value.ToLower().Contains("og:title"))
                        {
                            title = item.Groups[i + 1].Value;
                            break;
                        }

                        if (!string.IsNullOrEmpty(imageUrl) && item.Groups[i].Value.ToLower().Contains("image") && item.Groups[i].Value.ToLower().Contains("itemprop"))
                        {
                            AddImages(item.Groups[i + 1].Value);
                            break;
                        }

                        if (item.Groups[i].Value.ToLower().Contains("og:image"))
                        {
                            AddImages(item.Groups[i + 1].Value);
                            break;
                        }
                    }
                }
            }

            AddImages(GetOtherImages(html).ToArray());

            while (string.IsNullOrEmpty(imageUrl) && imageMinimumWidth >= MinimumImageDimension && imageMinimumHeight >= MinimumImageDimension)
            {
                foreach (var imageSrc in _imageUrls)
                {
                    if (TestImage(imageSrc, imageMinimumWidth, imageMinimumHeight))
                    {
                        imageUrl = imageSrc;
                    }
                }
                imageMinimumWidth -= 50;
                imageMinimumHeight -= 50;
            }

            return new LinkPreviewResult(url, title, description, imageUrl);
        }

        private void AddImages(params string[] imageUrls)
        {
            imageUrls.ForEach(imageUrl =>
            {
                _imageUrls.Insert(0, GetFullImageUrl(imageUrl));
            });
        }

        private string GetFullImageUrl(string imageUrl)
        {
            var url = "";
            if (!imageUrl.StartsWith("http"))
            {
                imageUrl = imageUrl.StartsWith("//") ? imageUrl.Substring(1) : imageUrl;
                imageUrl = imageUrl.StartsWith("/") ? imageUrl.Substring(1) : imageUrl;
                var uri = new Uri(_url);
                var host = uri.Host.EndsWith("//") ? uri.Host.Substring(0, uri.Host.Length - 1) : uri.Host;
                host = uri.Host.EndsWith("/") ? uri.Host : $"{uri.Host}/";
                url = $"{uri.Scheme}://{host}{imageUrl}";
            }
            else
            {
                url = imageUrl;
            }

            return HttpUtility.HtmlDecode(HttpUtility.UrlDecode(url));
        }

        private static bool TestImage(string imageUrl, int width, int height)
        {
            if (!TestUrl(imageUrl, "image"))
            {
                return false;
            }

            var imageInfo = GetImageInfo(imageUrl);
            if (imageInfo.Width < width || imageInfo.Height < height)
            {
                return false;
            }

            return true;
        }

        private static ImageInfo GetImageInfo(string imageUrl)
        {
            byte[] imageData = new WebClient().DownloadData(imageUrl);
            MemoryStream imgStream = new MemoryStream(imageData);

            if (imageUrl.EndsWith("svg"))
            {
                return new ImageInfo();
            }

            Image img = Image.FromStream(imgStream);
            var result = new ImageInfo
            {
                Width = img.Width,
                Height = img.Height,
                Format = img.RawFormat
            };
            img.Dispose();
            return result;
        }

        private static bool TestUrl(string url, string contentType = "")
        {
            try
            {
                var response = GetHttpWebResponse(url);
                if (!string.IsNullOrEmpty(contentType))
                {
                    if (!response.ContentType.ToLower().Contains(contentType))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private static string GetHtml(string url)
        {
            GetHttpWebResponse(url);
            return GetHtmlPage(url);
        }

        private static HttpWebResponse GetHttpWebResponse(string url)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            var response = request.GetResponse() as HttpWebResponse;
            var statusCode = response.StatusCode.ToString().ToLower();
            if (statusCode != "ok")
            {
                int.TryParse(statusCode, out var responseStatusCode);
                throw new HttpException(responseStatusCode, response.StatusDescription);
            }
            return response;
        }

        private static string GetHtmlPage(string url)
        {
            var result = "";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
            var response = request.GetResponse();

            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
                streamReader.Close();
            }
            return result;
        }

        private static MatchCollection GetWebPageMetaData(string html)
        {
            Regex regex = new Regex("<meta[\\s]+[^>]*?content[\\s]?=[\\s\"\']+(.*?)[\"\']+.*?>");
            var matches = regex.Matches(html);
            return matches;
        }

        public static List<string> GetOtherImages(string html)
        {
            var otherImages = new List<string>();
            const string pattern = @"<img\b[^\<\>]+?\bsrc\s*=\s*[""'](?<L>.+?)[""'][^\<\>]*?\>";
            foreach (Match match in Regex.Matches(html, pattern, RegexOptions.IgnoreCase))
            {
                var imageLink = match.Groups["L"].Value;
                otherImages.Add(imageLink);
            }

            const string backgroundPattern = @"url\((?!['""]?(?:data|http):)['""]?([^'""\)]*)['""]?\)";
            foreach (Match match in Regex.Matches(html, backgroundPattern, RegexOptions.IgnoreCase))
            {
                var imageLink = match.Groups[1].Value;
                otherImages.Add(imageLink);
            }

            return otherImages;
        }
    }
}
