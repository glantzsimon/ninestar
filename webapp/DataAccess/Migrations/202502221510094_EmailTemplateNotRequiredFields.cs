namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmailTemplateNotRequiredFields : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.EmailTemplate", new[] { "MembershipOptionId" });
            DropIndex("dbo.EmailTemplate", new[] { "PromotionId" });
            AlterColumn("dbo.EmailTemplate", "MembershipOptionId", c => c.Int());
            AlterColumn("dbo.EmailTemplate", "PromotionId", c => c.Int());
            CreateIndex("dbo.EmailTemplate", "MembershipOptionId");
            CreateIndex("dbo.EmailTemplate", "PromotionId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.EmailTemplate", new[] { "PromotionId" });
            DropIndex("dbo.EmailTemplate", new[] { "MembershipOptionId" });
            AlterColumn("dbo.EmailTemplate", "PromotionId", c => c.Int(nullable: false));
            AlterColumn("dbo.EmailTemplate", "MembershipOptionId", c => c.Int(nullable: false));
            CreateIndex("dbo.EmailTemplate", "PromotionId");
            CreateIndex("dbo.EmailTemplate", "MembershipOptionId");
        }
    }
}
