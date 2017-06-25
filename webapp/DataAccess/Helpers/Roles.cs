
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using K9.DataAccess.Exceptions;
using K9.DataAccess.Models;
using K9.SharedLibrary.Models;
using NLog;
using WebMatrix.WebData;

namespace K9.DataAccess.Helpers
{
	public class Roles : IRoles
	{
		private readonly DbContext _db;
		private readonly IRepository<Role> _roleRepository;
		private readonly IRepository<Permission> _permissionRepository;
		private readonly IRepository<UserRole> _userRolesRepository;
		private readonly IUsers _users;

		public Roles(DbContext db, IRepository<Role> roleRepository, IRepository<Permission> permissionRepository, IRepository<UserRole> userRolesRepository, IUsers users)
		{
			_db = db;
			_roleRepository = roleRepository;
			_permissionRepository = permissionRepository;
			_userRolesRepository = userRolesRepository;
			_users = users;
		}

		public List<Role> GetRolesForUser(string username)
		{
			var user = _users.GetUser(username);
			return _roleRepository.GetQuery(string.Format("SELECT * FROM [Role] WHERE [Id] IN (SELECT [RoleId] FROM [UserRole] WHERE [Userid] = {0})", user.Id)).ToList();
		}

		public List<Permission> GetPermissionsForUser(string username)
		{
			var user = _users.GetUser(username);
			return _permissionRepository.GetQuery(string.Format("SELECT * FROM [Permission] WHERE [RoleId] IN (SELECT [RoleId] FROM [UserRole] WHERE [Userid] = {0})", user.Id)).ToList();
		}

		public Role GetRole(string roleName)
		{
			var role = _roleRepository.GetQuery(string.Format("SELECT TOP 1 * FROM [Role] WHERE [Name] = '{0}'", roleName)).FirstOrDefault();
			if (role == null)
			{
				throw new RoleNotFoundException(roleName);
			}
			return role;
		}

		public bool UserHasPermission(string username, string permissionName)
		{
			return GetPermissionsForUser(username).Exists(p => p.Name == permissionName);
		}

		public bool UserIsInRole(string username, string roleName)
		{
			return GetRolesForUser(username).Exists(r => r.Name == roleName);
		}

		public void CreateRole(string roleName)
		{
			if (!_roleRepository.Exists(string.Format("SELECT [Id] FROM [Role] WHERE Name = '{0}'", roleName))) ;
			{
				_roleRepository.Create(new Role
				{
					Name = roleName
				});
			}
		}

		public void AddUserToRole(string username, string roleName)
		{
			if (!UserIsInRole(username, roleName)) ;
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
	}
}
