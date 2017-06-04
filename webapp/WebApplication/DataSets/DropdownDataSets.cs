
using System.Collections.Generic;
using K9.DataAccess.Models;
using K9.SharedLibrary.Models;

namespace K9.WebApplication.DataSets
{

	public class DropdownDataSets : IDropdownDataSets
	{
		public static IDropdownDataSets Instance { set; get; }
		public List<IListItem> CountriesList { get; private set; }
		public List<IListItem> CoursesList { get; private set; }

		public DropdownDataSets(IRepository<Country> countriesRepository, IRepository<Course> coursesRepository)
		{
			CountriesList = countriesRepository.ItemList();
			CoursesList = coursesRepository.ItemList();
		}

	}
}