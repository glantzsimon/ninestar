namespace K9.DataAccessLayer.Database
{
    using System.Data.Entity.Migrations;

    public partial class ScheduledOn : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.EmailQueueItem", "ScheduleOn", "ScheduledOn");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.EmailQueueItem", "ScheduledOn", "ScheduleOn");
        }
    }
}
