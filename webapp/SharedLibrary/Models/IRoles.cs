using System.Collections.Generic;

namespace K9.SharedLibrary.Models
{
	public interface IRoles
	{

		List<IRole> GetRolesForUser(string username);
		List<IPermission> GetPermissionsForUser(string username);
		IRole GetRole(string roleName);
		bool UserHasPermission(string username, string permissionName);
		bool UserIsInRole(string username, string roleName);
		void CreateRole(string roleName);
		void AddUserToRole(string username, string roleName);
	}
}
