using System.Text.RegularExpressions;
using K9.SharedLibrary.Extensions;

namespace K9.SharedLibrary.Helpers
{
	public class TemplateProcessor
	{

		#region Methods

		private static string GetPlaceHolder(string fieldName)
		{
			return string.Format("{{{0}}}", fieldName);
		}

		public static string PopulateTemplate(string template, object data)
		{
			foreach (var prop in data.GetType().GetProperties())
			{
				var placeHolder = GetPlaceHolder(prop.Name);
				var value = data.GetProperty(prop.Name).ToString();

				template = Regex.Replace(template, placeHolder, value);
			}
			return template;
		}

		#endregion

	}
}
