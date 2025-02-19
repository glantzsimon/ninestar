namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PromoCodeMembershipOption : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PromoCode", "MembershipOptionId", c => c.Int(nullable: false));
            CreateIndex("dbo.PromoCode", "MembershipOptionId");
            AddForeignKey("dbo.PromoCode", "MembershipOptionId", "dbo.MembershipOption", "Id");
            DropColumn("dbo.PromoCode", "SubscriptionType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PromoCode", "SubscriptionType", c => c.Int(nullable: false));
            DropForeignKey("dbo.PromoCode", "MembershipOptionId", "dbo.MembershipOption");
            DropIndex("dbo.PromoCode", new[] { "MembershipOptionId" });
            DropColumn("dbo.PromoCode", "MembershipOptionId");
        }
    }
}
