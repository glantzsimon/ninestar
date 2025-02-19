using System;
using System.Collections.Generic;
using System.Web.Mvc;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Tests.Models;
using Xunit;

namespace K9.SharedLibrary.Tests.Unit
{
	public class ExtensionTests
	{
		[Fact]
		public void ViewDataDictionary_AddCssClass_ShouldAddCssClassesWithSpace()
		{
			var viewDataDictionary = new ViewDataDictionary(new { });

			viewDataDictionary.MergeAttribute("class", "test2");
			viewDataDictionary.MergeAttribute("class", "test3");

			Assert.Equal("test2 test3", viewDataDictionary["class"]);
		}

		[Fact]
		public void DelimitedString_ShouldReturnCorrectString()
		{
			var delimitedString = new List<string>
			{
				"Wolf",
				"Back",
				"Meow"
			}.ToDelimitedString();

			var delimitedStringCustom = new List<string>
			{
				"Wolf",
				"Back",
				"Meow"
			}.ToDelimitedString(" |");

			Assert.Equal("Wolf, Back, Meow", delimitedString);
			Assert.Equal("Wolf | Back | Meow", delimitedStringCustom);
		}

		[Fact]
		public void ImplementsIUserData_ShouldReturnTrue()
		{
			Assert.True(typeof(Message).ImplementsIUserData());
		}

	    [Fact]
	    public void JoinUrl_CorrectJoinsUrls()
	    {
	        var baseUrl = new Uri("https://test.com/test/mystuff/");
	        var url = "Images/mycontent.jpg";

	        var result = new Uri(baseUrl.GetLeftPart(UriPartial.Authority)).CombineWith(url);
	        var result2 = baseUrl.CombineWith(url);

            Assert.Equal("https://test.com/Images/mycontent.jpg", result);
	        Assert.Equal("https://test.com/test/mystuff/Images/mycontent.jpg", result2);
        }

	}
}
