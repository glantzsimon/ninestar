
using System.Collections.Generic;
using K9.SharedLibrary.Models;

namespace K9.WebApplication.DataSets
{
	public interface IDropdownDataSets
	{
		List<IListItem> CountriesList { get; }
		List<IListItem> CoursesList { get; }
	}
}