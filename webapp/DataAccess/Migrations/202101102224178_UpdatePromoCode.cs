namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePromoCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PromoCode", "SentOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.PromoCode", "UsedOn", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PromoCode", "UsedOn");
            DropColumn("dbo.PromoCode", "SentOn");
        }
    }
}
