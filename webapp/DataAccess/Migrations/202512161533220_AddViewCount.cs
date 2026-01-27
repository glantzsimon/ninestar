namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddViewCount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ArticleView",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ArticleId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        ViewedOn = c.DateTime(nullable: false),
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
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.ArticleId)
                .Index(t => t.UserId)
                .Index(t => t.Name, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ArticleView", "UserId", "dbo.User");
            DropForeignKey("dbo.ArticleView", "ArticleId", "dbo.Article");
            DropIndex("dbo.ArticleView", new[] { "Name" });
            DropIndex("dbo.ArticleView", new[] { "UserId" });
            DropIndex("dbo.ArticleView", new[] { "ArticleId" });
            DropTable("dbo.ArticleView");
        }
    }
}
