namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePromoCode1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PromoCode", "SentOn", c => c.DateTime());
            AlterColumn("dbo.PromoCode", "UsedOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PromoCode", "UsedOn", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PromoCode", "SentOn", c => c.DateTime(nullable: false));
        }
    }
}
