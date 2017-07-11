
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
					string.Format("SELECT * FROM [Role] WHERE [Id] IN (SELECT [RoleId] FROM [UserRole] WHERE [Userid] = {0})", user.Id))
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
					string.Format(
						"SELECT * FROM [Permission] WHERE [Id] IN (SELECT [PermissionId] FROM [RolePermission] JOIN [UserRole] ON [UserRole].[RoleId] = [RolePermission].[RoleId] AND [UserRole].[Userid] = {0})", user.Id))
					.ToList();
			var list = new List<IPermission>();
			list.AddRange(permissions);
			return list;
		}

		public IRole GetRole(string roleName)
		{
			var role = _roleRepository.GetQuery(string.Format("SELECT TOP 1 * FROM [Role] WHERE [Name] = '{0}'", roleName)).FirstOrDefault();
			if (role == null)
			{
				throw new RoleNotFoundException(roleName);
			}
			return role;
		}

		public bool CurrentUserHasPermission<T>(string permissionName) where T : IObjectBase
		{
			var fullyQualifiedPermissionName = string.Format("{0}{1}", permissionName, typeof(T).Name);
			return UserHasPermission(WebSecurity.CurrentUserName, fullyQualifiedPermissionName);
		}

		public IPermission GetPermission(string permissionName)
		{
			var permission = _permissionRepository.GetQuery(string.Format("SELECT TOP 1 * FROM [Permission] WHERE [Name] = '{0}'", permissionName)).FirstOrDefault();
			if (permission == null)
			{
				throw new PermissionNotFoundException(permissionName);
			}
			return permission;
		}

		public bool UserHasPermission(string username, string permissionName)
		{
			return GetPermissionsForUser(username).Exists(p => p.Name == permissionName);
		}

		public bool UserIsInRole(string username, string roleName)
		{
			var user = _users.GetUser(username);
			var roles =
				_roleRepository.GetQuery(
					string.Format("SELECT * FROM [Role] WHERE [Name] = '{0}' AND [Id] IN (SELECT [RoleId] FROM [UserRole] WHERE [Userid] = {1})", roleName, user.Id))
					.ToList();
			return roles.Any();
		}

		public bool PermissionIsInRole(string permissionName, string roleName)
		{
			var permission = GetPermission(permissionName);
			var roles =
				_roleRepository.GetQuery(
					string.Format("SELECT * FROM [Role] WHERE [Name] = '{0}' AND [Id] IN (SELECT [RoleId] FROM [RolePermission] WHERE [PermissionId] = {1})", roleName, permission.Id))
					.ToList();
			return roles.Any();
		}

		public void CreateRole(string roleName, bool isSystemStandard = false)
		{
			if (!_roleRepository.Exists(string.Format("SELECT * FROM [Role] WHERE Name = '{0}'", roleName)))
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
			if (!_permissionRepository.Exists(string.Format("SELECT * FROM [Permission] WHERE Name = '{0}'", permissionName)))
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
			if (!UserIsInRole(username, roleName))
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
