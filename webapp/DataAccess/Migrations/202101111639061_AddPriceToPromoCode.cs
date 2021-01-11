namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPriceToPromoCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PromoCode", "TotalPrice", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PromoCode", "TotalPrice");
        }
    }
}
