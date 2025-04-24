using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;

namespace K9.SharedLibrary.Helpers
{
    public class ConfigHelper
	{

		public static IOptions<T> GetConfiguration<T>(string json) 
			where T : class 
		{
			var jsonObject = JObject.Parse(json);
			var configSection = jsonObject.GetValue(typeof(T).Name);
			var configuration = Activator.CreateInstance<T>();
			
			foreach (var propertyInfo in configuration.GetProperties())
			{
			    try
			    {
			        configuration.SetProperty(propertyInfo.Name, configSection[propertyInfo.Name]);
			    }
			    catch (Exception e)
			    {
			        var typeName = typeof(T).Name;
			        throw new Exception($"Error udpateing {typeName} configuration class. Make sure the appsettings.json file is up-to-date. Details: {e.GetFullErrorMessage()}");
			    }
			}

			return new Options<T>(configuration);
		}

	    public static IOptions<T> GetConfiguration<T>(NameValueCollection appsettings)
	        where T : class
	    {
	        var configuration = Activator.CreateInstance<T>();

	        foreach (var propertyInfo in configuration.GetProperties())
	        {
	            configuration.SetProperty(propertyInfo.Name, appsettings[propertyInfo.Name]);
	        }

	        return new Options<T>(configuration);
	    }

    }
}
