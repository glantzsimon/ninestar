namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImageUrls : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Article", "ImageUrl", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Article", "ImageUrl");
        }
    }
}
