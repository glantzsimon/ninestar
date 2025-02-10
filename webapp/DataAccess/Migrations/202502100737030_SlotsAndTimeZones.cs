namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SlotsAndTimeZones : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Consultation", "SlotId", c => c.Int());
            AlterColumn("dbo.Consultation", "ScheduledOn", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.Slot", "StartsOn", c => c.DateTimeOffset(nullable: false, precision: 7));
            CreateIndex("dbo.Consultation", "SlotId");
            AddForeignKey("dbo.Consultation", "SlotId", "dbo.Slot", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Consultation", "SlotId", "dbo.Slot");
            DropIndex("dbo.Consultation", new[] { "SlotId" });
            AlterColumn("dbo.Slot", "StartsOn", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Consultation", "ScheduledOn", c => c.DateTime());
            DropColumn("dbo.Consultation", "SlotId");
        }
    }
}
