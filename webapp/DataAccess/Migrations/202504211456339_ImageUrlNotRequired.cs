namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImageUrlNotRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Article", "ImageUrl", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Article", "ImageUrl", c => c.String(nullable: false));
        }
    }
}
