using System;

namespace K9.DataAccess.Exceptions
{
	public class RoleNotFoundException : ApplicationException
	{

		public RoleNotFoundException(string roleName)
			: base(string.Format("The Role '{0}' was not found.", roleName)) { }

	}
}
