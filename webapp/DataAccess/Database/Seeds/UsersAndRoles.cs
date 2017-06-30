using System;
using System.Data.Entity;
using System.Linq;
using K9.DataAccess.Config;
using K9.DataAccess.Helpers;
using K9.DataAccess.Models;
using K9.DataAccess.Respositories;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using WebMatrix.WebData;

namespace K9.DataAccess.Database.Seeds
{
	public static class UsersAndRolesSeeder
	{
		public static void SeedUsersAndRoles(DbContext context)
		{
			var roles = new Helpers.Roles(
				context,
				new BaseRepository<Role>(context),
				new BaseRepository<Permission>(context),
				new BaseRepository<UserRole>(context),
				new BaseRepository<RolePermission>(context),
				new Users(context, new BaseRepository<User>(context)));

			SeedSystemUser();
			SeedRoles(roles);
			SeedPermissions(roles);
		}

		private static void SeedSystemUser()
		{
			if (WebSecurity.Initialized)
			{
				if (!WebSecurity.UserExists(SystemUser.System))
				{
					WebSecurity.CreateUserAndAccount(SystemUser.System, AppConfig.SystemUserPassword, new
					{
						FirstName = "System",
						LastName = "User",
						Name = "System User",
						EmailAddress = "simon@glantzconsulting.co.uk",
						BirthDate = DateTime.Now,
						IsSystemStandrad = true,
						CreatedBy = SystemUser.System,
						CreatedOn = DateTime.Now,
						LastUpdatedBy = SystemUser.System,
						LastUpdatedOn = DateTime.Now
					});
				}
			}
		}

		private static void SeedRoles(Roles roles)
		{
			roles.CreateRole(RoleNames.Administrators, true);
			roles.CreateRole(RoleNames.PowerUsers, true);
			roles.AddUserToRole(SystemUser.System, RoleNames.Administrators);
		}

		private static void SeedPermissions(Roles roles)
		{
			foreach (var item in typeof(ObjectBase).Assembly.GetTypes().Where(t => typeof(ObjectBase).IsAssignableFrom(t)))
			{
				var instance = Activator.CreateInstance(item) as IPermissable;
				roles.CreatePermission(instance.CreatePermissionName);
				roles.AddPermissionsToRole(instance.CreatePermissionName, RoleNames.PowerUsers);
				
				roles.CreatePermission(instance.EditPermissionName);
				roles.AddPermissionsToRole(instance.EditPermissionName, RoleNames.PowerUsers);

				roles.CreatePermission(instance.DeletePermissionName);

				roles.CreatePermission(instance.ViewPermissionName);
				roles.AddPermissionsToRole(instance.ViewPermissionName, RoleNames.PowerUsers);
			}
		}

	}
}
