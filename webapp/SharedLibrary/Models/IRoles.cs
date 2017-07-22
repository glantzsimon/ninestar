using System.Collections.Generic;

namespace K9.SharedLibrary.Models
{
	public interface IRoles
	{

		List<IRole> GetRolesForUser(string username);
		List<IPermission> GetPermissionsForUser(string username);
		IRole GetRole(string roleName);
		bool CurrentUserIsInRole(string roleName);
		bool CurrentUserHasPermission<T>(string permissionName) where T : IObjectBase;
		List<IPermission> GetPermissionsForCurrentUser();
		bool UserHasPermission(string username, string permissionName);
		bool UserIsInRole(string username, string roleName);
		void CreateRole(string roleName, bool isSystemStandard = false);
		void CreatePermission(string permissionName, bool isSystemStandard = false);
		void AddUserToRole(string username, string roleName);
	}
}
