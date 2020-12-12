namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20201212_2117 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ArchiveCategory", newName: "ArchiveItemCategory");
            CreateTable(
                "dbo.ArchiveItemType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 128),
                        IsSystemStandard = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 255),
                        CreatedOn = c.DateTime(),
                        LastUpdatedBy = c.String(maxLength: 255),
                        LastUpdatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            AddColumn("dbo.ArchiveItemCategory", "IsSubscriptionOnly", c => c.Boolean(nullable: false));
            AddColumn("dbo.ArchiveItem", "TypeId", c => c.Int(nullable: false));
            AddColumn("dbo.ArchiveItem", "ImageUrl", c => c.String(maxLength: 512));
            AddColumn("dbo.ArchiveItem", "IsHideMetaData", c => c.Boolean(nullable: false));
            AddColumn("dbo.ArchiveItem", "IsShowLocalOnly", c => c.Boolean(nullable: false));
            AddColumn("dbo.ArchiveItem", "AdditionalCssClasses", c => c.String());
            AddColumn("dbo.ArchiveItem", "SeoFriendlyId", c => c.String());
            AddColumn("dbo.NewsItem", "ImageUrl", c => c.String(maxLength: 512));
            AddColumn("dbo.NewsItem", "IsShowLocalOnly", c => c.Boolean(nullable: false));
            AddColumn("dbo.NewsItem", "AdditionalCssClasses", c => c.String());
            AddColumn("dbo.NewsItem", "SeoFriendlyId", c => c.String());
            CreateIndex("dbo.ArchiveItem", "TypeId");
            AddForeignKey("dbo.ArchiveItem", "TypeId", "dbo.ArchiveItemType", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ArchiveItem", "TypeId", "dbo.ArchiveItemType");
            DropIndex("dbo.ArchiveItemType", new[] { "Name" });
            DropIndex("dbo.ArchiveItem", new[] { "TypeId" });
            DropColumn("dbo.NewsItem", "SeoFriendlyId");
            DropColumn("dbo.NewsItem", "AdditionalCssClasses");
            DropColumn("dbo.NewsItem", "IsShowLocalOnly");
            DropColumn("dbo.NewsItem", "ImageUrl");
            DropColumn("dbo.ArchiveItem", "SeoFriendlyId");
            DropColumn("dbo.ArchiveItem", "AdditionalCssClasses");
            DropColumn("dbo.ArchiveItem", "IsShowLocalOnly");
            DropColumn("dbo.ArchiveItem", "IsHideMetaData");
            DropColumn("dbo.ArchiveItem", "ImageUrl");
            DropColumn("dbo.ArchiveItem", "TypeId");
            DropColumn("dbo.ArchiveItemCategory", "IsSubscriptionOnly");
            DropTable("dbo.ArchiveItemType");
            RenameTable(name: "dbo.ArchiveItemCategory", newName: "ArchiveCategory");
        }
    }
}
