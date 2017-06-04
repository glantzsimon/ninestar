using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Security;
using K9.DataAccess.Config;
using K9.SharedLibrary.Authentication;
using WebMatrix.WebData;

namespace K9.DataAccess.Database.Seeds
{
	public static class UsersAndRolesSeeder
	{
		public static void SeedUsersAndRoles(DbContext context)
		{
			SeedSystemUser();
			SeedRoles();
			AssignRoles();
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

		private static void SeedRoles()
		{
			if (WebSecurity.Initialized)
			{
				if (!Roles.RoleExists(UserRoles.Administrators))
				{
					Roles.CreateRole(UserRoles.Administrators);
				}

				if (!Roles.RoleExists(UserRoles.PowerUsers))
				{
					Roles.CreateRole(UserRoles.PowerUsers);
				}
			}
		}

		private static void AssignRoles()
		{
			if (WebSecurity.Initialized)
			{
				if (!Roles.GetRolesForUser(SystemUser.System).Contains(UserRoles.Administrators))
				{
					Roles.AddUsersToRoles(new[] {SystemUser.System}, new[] {UserRoles.Administrators});
				}
			}
		}

	}
}
