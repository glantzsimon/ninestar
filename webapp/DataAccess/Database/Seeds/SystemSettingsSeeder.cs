using K9.DataAccessLayer.Models;
using System.Data.Entity;
using System.Linq;

namespace K9.DataAccessLayer.Database.Seeds
{
    public static class SystemSettingsSeeder
    {
        public static void Seed(DbContext context)
        {
            AddOrEditSystemSettings(context, new SystemSetting
            {
                IsPausedEmailJobQueue = false,
                IsSendMembershipUpgradeReminders = false
            });

            context.SaveChanges();
        }

        private static void AddOrEditSystemSettings(DbContext context, SystemSetting setting)
        {
            var entity = context.Set<SystemSetting>().FirstOrDefault();
            
            if (entity == null)
            {
                context.Set<SystemSetting>().Add(setting);
            }
            else
            {
                entity.IsPausedEmailJobQueue = setting.IsPausedEmailJobQueue;
                entity.IsSendMembershipUpgradeReminders = setting.IsSendMembershipUpgradeReminders;

                context.Entry(entity).State = EntityState.Modified;
            }
        }

    }
}
