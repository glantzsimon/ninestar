namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExpandEmailTemplates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailQueueItem", "EmailTemplateId", c => c.Int(nullable: false));
            AddColumn("dbo.EmailTemplate", "MembershipOptionId", c => c.Int(nullable: false));
            AddColumn("dbo.EmailTemplate", "Discount", c => c.Int());
            CreateIndex("dbo.EmailQueueItem", "EmailTemplateId");
            CreateIndex("dbo.EmailTemplate", "MembershipOptionId");
            AddForeignKey("dbo.EmailTemplate", "MembershipOptionId", "dbo.MembershipOption", "Id");
            AddForeignKey("dbo.EmailQueueItem", "EmailTemplateId", "dbo.EmailTemplate", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmailQueueItem", "EmailTemplateId", "dbo.EmailTemplate");
            DropForeignKey("dbo.EmailTemplate", "MembershipOptionId", "dbo.MembershipOption");
            DropIndex("dbo.EmailTemplate", new[] { "MembershipOptionId" });
            DropIndex("dbo.EmailQueueItem", new[] { "EmailTemplateId" });
            DropColumn("dbo.EmailTemplate", "Discount");
            DropColumn("dbo.EmailTemplate", "MembershipOptionId");
            DropColumn("dbo.EmailQueueItem", "EmailTemplateId");
        }
    }
}
