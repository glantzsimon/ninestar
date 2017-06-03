using K9.DataAccess.Models;
using K9.WebApplication.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace K9.WebApplication.Tests.Unit
{
	[TestClass]
	public class DataTableOptionsTests
	{
		
		[TestMethod]
		public void GetButtonRenderer_ShouldReturnAppropriateRenderer()
		{
			var options = new DataTableOptions<Country>();

			options.AllowDetail = true;
			Assert.AreEqual("function (data, type, row) { return '<a href=\"/details/\"' + data.Id + ' class=\"btn btn-info\">Details</a>';}", options.GetButtonRenderer());

			options.AllowEdit = true;
			Assert.AreEqual("function (data, type, row) { return '<a href=\"/details/\"' + data.Id + ' class=\"btn btn-info\">Details</a>" +
			                "<a href=\"/edit/\"' + data.Id + ' class=\"btn btn-primary\">Edit</a>';}", options.GetButtonRenderer());

			options.AllowDelete = true;
			Assert.AreEqual("function (data, type, row) { return '<a href=\"/details/\"' + data.Id + ' class=\"btn btn-info\">Details</a>" +
							"<a href=\"/edit/\"' + data.Id + ' class=\"btn btn-primary\">Edit</a>" +
							"<a href=\"/delete/\"' + data.Id + ' class=\"btn btn-danger\">Delete</a>';}", options.GetButtonRenderer());
		}



	
	}

}
