using System.Collections.Generic;
using System.Web.Mvc;
using K9.DataAccess.Models;
using K9.SharedLibrary.Extensions;
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

	}
}
