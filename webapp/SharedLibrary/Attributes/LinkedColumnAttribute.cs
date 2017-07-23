using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9.SharedLibrary.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class LinkedColumnAttribute : NotMappedAttribute
	{
		public string LinkedTableName { get; set; }
		public string LinkedColumnName { get; set; }
		public string ForeignKey { get; set; }

	}
}
