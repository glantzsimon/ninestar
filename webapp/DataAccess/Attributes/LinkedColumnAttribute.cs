﻿using System.ComponentModel.DataAnnotations.Schema;

namespace K9.DataAccess.Attributes
{
	public class LinkedColumnAttribute : NotMappedAttribute
	{
		public string LinkedTableName { get; set; }
		public string LinkedColumnName { get; set; }

	}
}