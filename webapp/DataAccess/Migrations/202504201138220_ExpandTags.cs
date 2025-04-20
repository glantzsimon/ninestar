namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExpandTags : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ArticleTag",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ArticleId = c.Int(nullable: false),
                        TagId = c.Int(nullable: false),
                        Name = c.String(maxLength: 128),
                        IsSystemStandard = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 255),
                        CreatedOn = c.DateTime(),
                        LastUpdatedBy = c.String(maxLength: 255),
                        LastUpdatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Article", t => t.ArticleId)
                .ForeignKey("dbo.Tag", t => t.TagId)
                .Index(t => t.ArticleId)
                .Index(t => t.TagId)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Tag",
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
            
            AddColumn("dbo.Article", "Summary", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ArticleTag", "TagId", "dbo.Tag");
            DropForeignKey("dbo.ArticleTag", "ArticleId", "dbo.Article");
            DropIndex("dbo.Tag", new[] { "Name" });
            DropIndex("dbo.ArticleTag", new[] { "Name" });
            DropIndex("dbo.ArticleTag", new[] { "TagId" });
            DropIndex("dbo.ArticleTag", new[] { "ArticleId" });
            DropColumn("dbo.Article", "Summary");
            DropTable("dbo.Tag");
            DropTable("dbo.ArticleTag");
        }
    }
}
