
using System.Collections.Generic;
using K9.DataAccess.Models;

namespace K9.DataAccess.Helpers
{
	public interface IRoles
	{

		List<Role> GetRolesForUser(string username);
		List<Permission> GetPermissionsForUser(string username);
		Role GetRole(string roleName);
		bool UserHasPermission(string username, string permissionName);
		bool UserIsInRole(string username, string roleName);
		void CreateRole(string roleName);
		void AddUserToRole(string username, string roleName);
	}
}
