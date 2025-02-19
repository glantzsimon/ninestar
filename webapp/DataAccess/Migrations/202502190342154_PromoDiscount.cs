namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PromoDiscount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PromoCode", "Discount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PromoCode", "Discount");
        }
    }
}
