using K9.SharedLibrary.Extensions;
using System.Text.RegularExpressions;

namespace K9.SharedLibrary.Helpers
{
    public class TemplateParser
	{

		#region Methods

	    public static string Parse(string template, object data)
		{
			foreach (var prop in data.GetType().GetProperties())
			{
				var placeHolder = GetPlaceHolder(prop.Name);
				var doublePlaceHolder = GetDoublePlaceHolder(prop.Name);
				var value = data.GetProperty(prop.Name)?.ToString();

			    template = Regex.Replace(template, doublePlaceHolder, value ?? string.Empty);
				template = Regex.Replace(template, placeHolder, value ?? string.Empty);
			}
			return template;
		}

	    private static string GetPlaceHolder(string fieldName)
	    {
	        return $"{{{fieldName}}}";
	    }

	    private static string GetDoublePlaceHolder(string fieldName)
	    {
	        return $"{{{{{fieldName}}}}}"; // {{FirstName}}
	    }


	    #endregion

	}
}
