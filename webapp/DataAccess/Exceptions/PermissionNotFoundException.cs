﻿using System;

namespace K9.DataAccess.Exceptions
{
	public class PermissionNotFoundException : ApplicationException
	{

		public PermissionNotFoundException(string permissionName)
			: base(string.Format("The Permission '{0}' was not found.", permissionName)) { }

	}
}