using System;

namespace K9.DataAccess.Exceptions
{
	public class UserNotFoundException : ApplicationException
	{

		public UserNotFoundException(string username)
			: base(string.Format("The user '{0}' was not found.", username)) { }

	}
}
