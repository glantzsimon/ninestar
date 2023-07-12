using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Helpers;
using K9.Base.DataAccessLayer.Models;
using K9.Base.DataAccessLayer.Respositories;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using System;
using System.Data.Entity;
using System.Linq;

namespace K9.DataAccessLayer.Database.Seeds
{
    public static class PermissionsSeeder
    {
        public static void Seed(DbContext context)
        {
            return;

            var roles = new Roles(
                context,
                new BaseRepository<Role>(context),
                new BaseRepository<Permission>(context),
                new BaseRepository<UserRole>(context),
                new BaseRepository<RolePermission>(context),
                new Users(context, new BaseRepository<User>(context)));

            SeedPermissions(roles);
        }

        private static void SeedPermissions(Roles roles)
        {
            return;

            foreach (var item in typeof(PermissionsSeeder).Assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(ObjectBase))))
            {
                var instance = Activator.CreateInstance(item) as IPermissable;
                var defaultPermissionsAttribute = item.GetAttribute<DefaultPermissionsAttribute>();
                var isDefault = defaultPermissionsAttribute?.Role == RoleNames.DefaultUsers;
                var isAdmin = defaultPermissionsAttribute?.Role == RoleNames.Administrators;
                var isPowerUser = defaultPermissionsAttribute?.Role == RoleNames.PowerUsers;

                if (!isAdmin)
                {
                    roles.CreatePermission(instance.CreatePermissionName, true);
                    roles.AddPermissionsToRole(instance.CreatePermissionName, RoleNames.Administrators, true);
                    roles.AddPermissionsToRole(instance.CreatePermissionName, RoleNames.PowerUsers);
                    if (isDefault)
                    {
                        roles.AddPermissionsToRole(instance.CreatePermissionName, RoleNames.DefaultUsers);
                    }

                    roles.CreatePermission(instance.EditPermissionName, true);
                    roles.AddPermissionsToRole(instance.EditPermissionName, RoleNames.Administrators, true);
                    roles.AddPermissionsToRole(instance.EditPermissionName, RoleNames.PowerUsers);
                    if (isDefault)
                    {
                        roles.AddPermissionsToRole(instance.EditPermissionName, RoleNames.DefaultUsers);
                    }

                    roles.CreatePermission(instance.DeletePermissionName, true);
                    roles.AddPermissionsToRole(instance.DeletePermissionName, RoleNames.Administrators, true);
                    if (isDefault)
                    {
                        roles.AddPermissionsToRole(instance.DeletePermissionName, RoleNames.DefaultUsers);
                        roles.AddPermissionsToRole(instance.DeletePermissionName, RoleNames.PowerUsers);
                    }
                    else if (isPowerUser)
                    {
                        roles.AddPermissionsToRole(instance.DeletePermissionName, RoleNames.PowerUsers);
                    }

                    roles.CreatePermission(instance.ViewPermissionName, true);
                    roles.AddPermissionsToRole(instance.ViewPermissionName, RoleNames.Administrators, true);
                    roles.AddPermissionsToRole(instance.ViewPermissionName, RoleNames.PowerUsers);
                    if (isDefault)
                    {
                        roles.AddPermissionsToRole(instance.ViewPermissionName, RoleNames.DefaultUsers);
                    }
                }
            }
        }

    }
}
