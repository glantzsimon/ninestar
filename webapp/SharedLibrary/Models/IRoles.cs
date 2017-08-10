using System.Collections.Generic;

namespace K9.SharedLibrary.Models
{
	public interface IRoles
	{

		List<IRole> GetRolesForUser(string username);
		List<IPermission> GetPermissionsForUser(string username);
		IRole GetRole(string roleName);
		bool CurrentUserIsInRoles(params string[] roleNames);
		bool CurrentUserHasPermissions<T>(params string[] permissionNames) where T : IObjectBase;
		List<IPermission> GetPermissionsForCurrentUser();
		List<IRole> GetRolesForCurrentUser();
		bool UserHasPermissions(string username, params string[] permissionNames);
		bool UserIsInRoles(string username, params string[] roleNames);
		void CreateRole(string roleName, bool isSystemStandard = false);
		void CreatePermission(string permissionName, bool isSystemStandard = false);
		void AddUserToRole(string username, string roleName);
	}
}
