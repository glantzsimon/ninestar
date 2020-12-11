namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class July2020 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ArchiveItem", "Url", c => c.String());
            AddColumn("dbo.NewsItem", "Type", c => c.Int(nullable: false));
            AddColumn("dbo.NewsItem", "Url", c => c.String(maxLength: 512));
            DropColumn("dbo.NewsItem", "VideoUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.NewsItem", "VideoUrl", c => c.String(maxLength: 512));
            DropColumn("dbo.NewsItem", "Url");
            DropColumn("dbo.NewsItem", "Type");
            DropColumn("dbo.ArchiveItem", "Url");
        }
    }
}
