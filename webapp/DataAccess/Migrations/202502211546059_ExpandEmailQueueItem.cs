namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExpandEmailQueueItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailQueueItem", "Type", c => c.Int(nullable: false));
            AddColumn("dbo.EmailQueueItem", "ContactId", c => c.Int());
            AddColumn("dbo.EmailQueueItem", "UserId", c => c.Int());
            AddColumn("dbo.EmailQueueItem", "ScheduleOn", c => c.DateTime());
            CreateIndex("dbo.EmailQueueItem", "ContactId");
            CreateIndex("dbo.EmailQueueItem", "UserId");
            AddForeignKey("dbo.EmailQueueItem", "ContactId", "dbo.Contact", "Id");
            AddForeignKey("dbo.EmailQueueItem", "UserId", "dbo.User", "Id");
            DropColumn("dbo.EmailQueueItem", "RecipientEmailAddress");
            DropColumn("dbo.EmailQueueItem", "RecipientName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EmailQueueItem", "RecipientName", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.EmailQueueItem", "RecipientEmailAddress", c => c.String(nullable: false, maxLength: 255));
            DropForeignKey("dbo.EmailQueueItem", "UserId", "dbo.User");
            DropForeignKey("dbo.EmailQueueItem", "ContactId", "dbo.Contact");
            DropIndex("dbo.EmailQueueItem", new[] { "UserId" });
            DropIndex("dbo.EmailQueueItem", new[] { "ContactId" });
            DropColumn("dbo.EmailQueueItem", "ScheduleOn");
            DropColumn("dbo.EmailQueueItem", "UserId");
            DropColumn("dbo.EmailQueueItem", "ContactId");
            DropColumn("dbo.EmailQueueItem", "Type");
        }
    }
}
