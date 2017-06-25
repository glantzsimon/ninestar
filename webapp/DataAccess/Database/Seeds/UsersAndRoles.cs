using System;
using System.Data.Entity;
using K9.DataAccess.Config;
using K9.DataAccess.Helpers;
using K9.DataAccess.Models;
using K9.DataAccess.Respositories;
using K9.SharedLibrary.Authentication;
using WebMatrix.WebData;

namespace K9.DataAccess.Database.Seeds
{
	public static class UsersAndRolesSeeder
	{
		public static void SeedUsersAndRoles(DbContext context)
		{
			SeedSystemUser();
			SeedRoles(context);
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
						CreatedBy = SystemUser.System,
						CreatedOn = DateTime.Now,
						LastUpdatedBy = SystemUser.System,
						LastUpdatedOn = DateTime.Now
					});
				}
			}
		}

		private static void SeedRoles(DbContext context)
		{
			var roles = new Helpers.Roles(
				context,
				new BaseRepository<Role>(context),
				new BaseRepository<Permission>(context),
				new BaseRepository<UserRole>(context),
				new Users(context, new BaseRepository<User>(context)) as IUsers);

			roles.CreateRole(RoleNames.Administrators);
			roles.CreateRole(RoleNames.PowerUsers);

			roles.AddUserToRole(SystemUser.System, RoleNames.Administrators);
		}

	}
}
