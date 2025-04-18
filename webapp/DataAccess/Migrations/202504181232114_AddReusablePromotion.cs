namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReusablePromotion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Promotion", "IsReusable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Promotion", "IsReusable");
        }
    }
}
