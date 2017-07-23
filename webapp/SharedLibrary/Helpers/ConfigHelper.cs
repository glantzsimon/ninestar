using System;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using Newtonsoft.Json.Linq;

namespace K9.SharedLibrary.Helpers
{
	public class ConfigHelper
	{

		public static T GetConfiguration<T>(string json) 
			where T : IConfigurable
		{
			var jsonObject = JObject.Parse(json);
			var configSection = jsonObject.GetValue("SmtpConfiguration");
			var configuration = Activator.CreateInstance<T>();
			
			foreach (var propertyInfo in configuration.GetProperties())
			{
				configuration.SetProperty(propertyInfo.Name, configSection[propertyInfo.Name]);
			}

			return configuration;
		}

	}
}
