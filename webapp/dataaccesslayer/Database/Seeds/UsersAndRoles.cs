
using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Security;
using K9.DataAccess.Config;
using K9.SharedLibrary.Authentication;
using WebMatrix.WebData;

namespace K9.DataAccess.Database
{
	public partial class DatabaseInitialiser : DbMigrationsConfiguration<Db>
	{

		private void SeedUsersAndRoles(Db context)
		{
			SeedSystemUser();
			SeedRoles();
			AssignRoles();
		}

		private void SeedSystemUser()
		{
			if (!WebSecurity.UserExists(SystemUser.System))
			{
				WebSecurity.CreateUserAndAccount(SystemUser.System, AppConfig.SystemUserPassword, new
				{
					EmailAddress = "system@default.net",
					CreatedBy = SystemUser.System,
					CreatedOn = DateTime.Now,
					LastUpdatedBy = SystemUser.System,
					LastUpdatedOn = DateTime.Now
				});
			}
		}

		private void SeedRoles()
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

		private void AssignRoles()
		{
			if (!Roles.GetRolesForUser(SystemUser.System).Contains(UserRoles.Administrators))
			{
				Roles.AddUsersToRoles(new[] { SystemUser.System }, new[] { UserRoles.Administrators });
			}
		}

	}
}
