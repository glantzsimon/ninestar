using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using K9.DataAccess.Models;
using K9.WebApplication.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace K9.WebApplication.Tests.Unit
{
	[TestClass]
	public class ModelStateTests
	{

		[TestMethod]
		public void ModelState_AddErrorMessageFromException_ShouldRenderCorrectMessage()
		{
			var modelState = new ModelStateDictionary();
			var exception = new Exception("An error occurred while updating the entries. See the inner exception for details. Cannot insert duplicate key row in object 'dbo.Country' with unique index 'IX_TwoLetterCountryCode'. The statement has been terminated.");
			modelState.AddErrorMessageFromException(exception, new Country
			{
					TwoLetterCountryCode = "SDF"
			});

			Assert.AreEqual("A country with the Two Letter Country Code 'SDF' already exists.", modelState["TwoLetterCountryCode"].Errors.FirstOrDefault().ErrorMessage);
		}

		[TestMethod]
		public void ModelState_AddErrorMessageFromException_ShouldRenderCorrectMessage_InFrench()
		{
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr");
			var modelState = new ModelStateDictionary();
			var exception = new Exception("An error occurred while updating the entries. See the inner exception for details. Cannot insert duplicate key row in object 'dbo.Country' with unique index 'IX_TwoLetterCountryCode'. The statement has been terminated.");
			modelState.AddErrorMessageFromException(exception, new Country
			{
				TwoLetterCountryCode = "SDF"
			});

			Assert.AreEqual("Un pays avec le Code Pays à Deux Lettres 'SDF' existe déjà.", modelState["TwoLetterCountryCode"].Errors.FirstOrDefault().ErrorMessage);
		}

	}

}
