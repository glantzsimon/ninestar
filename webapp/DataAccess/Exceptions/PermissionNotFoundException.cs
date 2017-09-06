using System;

namespace K9.DataAccess.Exceptions
{
	public class PermissionNotFoundException : ApplicationException
	{

		public PermissionNotFoundException(string permissionName)
			: base($"The Permission '{permissionName}' was not found.") { }

	}
}
