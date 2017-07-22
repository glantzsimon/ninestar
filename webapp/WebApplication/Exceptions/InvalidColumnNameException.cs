
using System;

namespace K9.WebApplication.Exceptions
{
	public class InvalidColumnNameException : ApplicationException
	{
		public InvalidColumnNameException(string columnNames) : base(string.Format("Invalid column name(s) '{0}'", columnNames))
		{
		}
	}
}