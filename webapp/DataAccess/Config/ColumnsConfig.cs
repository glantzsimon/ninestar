using System.Collections.Generic;
using K9.SharedLibrary.Models;

namespace K9.DataAccess.Config
{
	public class ColumnsConfig : IColumnsConfig
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
