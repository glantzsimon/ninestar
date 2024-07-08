
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Web;
using System.Web.Helpers;

namespace K9.WebApplication.OAuth
{
	public class FacebookClient : DotNetOpenAuth.AspNet.Clients.OAuth2Client
	{
		private const string AuthorizationEp = "https://www.facebook.com/dialog/oauth";
		private const string TokenEp = "https://graph.facebook.com/oauth/access_token";
		private readonly string _appId;
		private readonly string _appSecret;

		public FacebookClient(string appId, string appSecret)
			: base("Facebook")
		{
			_appId = appId;
			_appSecret = appSecret;
		}

		protected override Uri GetServiceLoginUrl(Uri returnUrl)
		{
			return new Uri(
						AuthorizationEp
						+ "?client_id=" + _appId
						+ "&redirect_uri=" + HttpUtility.UrlEncode(returnUrl.ToString())
						+ "&scope=email,user_about_me"
						+ "&display=page"
					);
		}

		protected override IDictionary<string, string> GetUserData(string accessToken)
		{
			WebClient client = new WebClient();
			string content = client.DownloadString(
				"https://graph.facebook.com/me?access_token=" + accessToken
			);
			dynamic data = Json.Decode(content);
			return new Dictionary<string, string> {
                {
                    "id",
                    data.id
                },
                {
                    "name",
                    data.name
                },
                {
                    "photo",
                    "https://graph.facebook.com/" + data.id + "/picture"
                },
                {
                    "email",
                    data.email
                }
            };
		}

		protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
		{
			WebClient client = new WebClient();
			string content = client.DownloadString(
				TokenEp
				+ "?client_id=" + _appId
				+ "&client_secret=" + _appSecret
				+ "&redirect_uri=" + HttpUtility.UrlEncode(returnUrl.ToString())
				+ "&code=" + authorizationCode
			);

			NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(content);
			if (nameValueCollection != null)
			{
				string result = nameValueCollection["access_token"];
				return result;
			}
			return null;
		}
	}
}