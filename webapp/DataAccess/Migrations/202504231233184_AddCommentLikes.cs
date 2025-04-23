namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCommentLikes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ArticleCommentLike",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ArticleId = c.Int(nullable: false),
                        ArticleCommentId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        LikedOn = c.DateTime(nullable: false),
                        IsApproved = c.Boolean(nullable: false),
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
                .ForeignKey("dbo.ArticleComment", t => t.ArticleCommentId)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.ArticleId)
                .Index(t => t.ArticleCommentId)
                .Index(t => t.UserId)
                .Index(t => t.Name, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ArticleCommentLike", "UserId", "dbo.User");
            DropForeignKey("dbo.ArticleCommentLike", "ArticleCommentId", "dbo.ArticleComment");
            DropForeignKey("dbo.ArticleCommentLike", "ArticleId", "dbo.Article");
            DropIndex("dbo.ArticleCommentLike", new[] { "Name" });
            DropIndex("dbo.ArticleCommentLike", new[] { "UserId" });
            DropIndex("dbo.ArticleCommentLike", new[] { "ArticleCommentId" });
            DropIndex("dbo.ArticleCommentLike", new[] { "ArticleId" });
            DropTable("dbo.ArticleCommentLike");
        }
    }
}
