using System.Collections.Generic;

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
					"Id",
					"CreatedBy",
					"CreatedOn",
					"LastUpdatedBy",
					"LastUpdatedOn"
				};
			}
		}
	}
}
