
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using K9.DataAccess.Exceptions;
using K9.DataAccess.Models;
using K9.SharedLibrary.Models;
using WebMatrix.WebData;

namespace K9.DataAccess.Helpers
{
	public class Roles : IRoles
	{
		private readonly DbContext _db;
		private readonly IRepository<Role> _roleRepository;
		private readonly IRepository<Permission> _permissionRepository;
		private readonly IRepository<UserRole> _userRolesRepository;
		private readonly IRepository<RolePermission> _rolePermissionsRepository;
		private readonly IUsers _users;

		public Roles(DbContext db, IRepository<Role> roleRepository, IRepository<Permission> permissionRepository, IRepository<UserRole> userRolesRepository, IRepository<RolePermission> rolePermissionsRepository, IUsers users)
		{
			_db = db;
			_roleRepository = roleRepository;
			_permissionRepository = permissionRepository;
			_userRolesRepository = userRolesRepository;
			_rolePermissionsRepository = rolePermissionsRepository;
			_users = users;
		}

		public List<IRole> GetRolesForUser(string username)
		{
			var user = _users.GetUser(username);
			var roles =
				_roleRepository.GetQuery(
				        $"SELECT * FROM [Role] WHERE [Id] IN (SELECT [RoleId] FROM [UserRole] WHERE [Userid] = {user.Id})")
					.ToList();
			var list = new List<IRole>();
			list.AddRange(roles);
			return list;
		}

		public List<IPermission> GetPermissionsForUser(string username)
		{
			var user = _users.GetUser(username);
			var permissions =
				_permissionRepository.GetQuery(
				        $"SELECT * FROM [Permission] WHERE [Id] IN (SELECT [PermissionId] FROM [RolePermission] JOIN [UserRole] ON [UserRole].[RoleId] = [RolePermission].[RoleId] AND [UserRole].[Userid] = {user.Id})")
					.ToList();
			var list = new List<IPermission>();
			list.AddRange(permissions);
			return list;
		}

		public IRole GetRole(string roleName)
		{
			var role = _roleRepository.GetQuery($"SELECT TOP 1 * FROM [Role] WHERE [Name] = '{roleName}'").FirstOrDefault();
			if (role == null)
			{
				throw new RoleNotFoundException(roleName);
			}
			return role;
		}

		public bool CurrentUserIsInRoles(params string[] roleNames)
		{
			return WebSecurity.IsAuthenticated && UserIsInRoles(WebSecurity.CurrentUserName, roleNames);
		}

		public bool CurrentUserHasPermissions<T>(params string[] permissionNames) where T : IObjectBase
		{
			var fullyQualifiedPermissionNames = permissionNames.Select(permissionName => $"{permissionName}{typeof(T).Name}");
			return WebSecurity.IsAuthenticated && UserHasPermissions(WebSecurity.CurrentUserName, fullyQualifiedPermissionNames.ToArray());
		}

		public List<IPermission> GetPermissionsForCurrentUser()
		{
			return WebSecurity.IsAuthenticated ? GetPermissionsForUser(WebSecurity.CurrentUserName) : new List<IPermission>();
		}

		public List<IRole> GetRolesForCurrentUser()
		{
			return WebSecurity.IsAuthenticated ? GetRolesForUser(WebSecurity.CurrentUserName) : new List<IRole>();
		}

		public IPermission GetPermission(string permissionName)
		{
			var permission = _permissionRepository.GetQuery(
			    $"SELECT TOP 1 * FROM [Permission] WHERE [Name] = '{permissionName}'").FirstOrDefault();
			if (permission == null)
			{
				throw new PermissionNotFoundException(permissionName);
			}
			return permission;
		}

		public bool UserHasPermissions(string username, params string[] permissionNames)
		{
			return GetPermissionsForUser(username).Exists(p => permissionNames.Contains(p.Name));
		}

		public bool UserIsInRoles(string username, params string[] roleNames)
		{
			return GetRolesForUser(username).Any(r => roleNames.Contains(r.Name));
		}

		public bool PermissionIsInRole(string permissionName, string roleName)
		{
			var permission = GetPermission(permissionName);
			var roles =
				_roleRepository.GetQuery(
				        $"SELECT * FROM [Role] WHERE [Name] = '{roleName}' AND [Id] IN (SELECT [RoleId] FROM [RolePermission] WHERE [PermissionId] = {permission.Id})")
					.ToList();
			return roles.Any();
		}

		public void CreateRole(string roleName, bool isSystemStandard = false)
		{
			if (!_roleRepository.Exists($"SELECT * FROM [Role] WHERE Name = '{roleName}'"))
			{
				_roleRepository.Create(new Role
				{
					Name = roleName,
					IsSystemStandard = isSystemStandard
				});
			}
		}

		public void CreatePermission(string permissionName, bool isSystemStandard = false)
		{
			if (!_permissionRepository.Exists($"SELECT * FROM [Permission] WHERE Name = '{permissionName}'"))
			{
				_permissionRepository.Create(new Permission
				{
					Name = permissionName,
					IsSystemStandard = isSystemStandard
				});
			}
		}

		public void AddUserToRole(string username, string roleName)
		{
			if (!UserIsInRoles(username, roleName))
			{
				var user = _users.GetUser(username);
				var role = GetRole(roleName);
				_userRolesRepository.Create(new UserRole
				{
					UserId = user.Id,
					RoleId = role.Id
				});
			}
		}

		public void AddPermissionsToRole(string permissionName, string roleName)
		{
			if (!PermissionIsInRole(permissionName, roleName))
			{
				var permission = GetPermission(permissionName);
				var role = GetRole(roleName);
				_rolePermissionsRepository.Create(new RolePermission
				{
					PermissionId = permission.Id,
					RoleId = role.Id
				});
			}
		}
	}
}
