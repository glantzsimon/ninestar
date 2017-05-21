using System.Collections.Generic;
using K9.SharedLibrary.Models;

namespace K9.DataAccess.Models
{
	public class IgnoreColumns : IIgnoreColumns
	{
		public List<string> ColumnsToIgnore
		{
			get
			{
				return new List<string>
				{
					"CreatedBy",
					"CreatedOn",
					"LastUpdatedBy",
					"LastUpdatedOn"
				};
			}
		}
	}
}
