﻿using K9.DataAccess.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.DataSets;
using K9.WebApplication.Helpers;
using NLog;

namespace K9.WebApplication.Controllers
{
	public class StudentsController : BaseController<Student>
	{
		public StudentsController(IRepository<Student> repository, ILogger logger, IDataTableAjaxHelper<Student> ajaxHelper, IDataSetsHelper dropdownDataSets)
			: base(repository, logger, ajaxHelper, dropdownDataSets)
		{
		}
	}
}