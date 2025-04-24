namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RefactorLikes : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ArticleCommentLike", newName: "Like");
            DropIndex("dbo.Like", new[] { "ArticleId" });
            DropIndex("dbo.Like", new[] { "ArticleCommentId" });
            AlterColumn("dbo.Like", "ArticleId", c => c.Int());
            AlterColumn("dbo.Like", "ArticleCommentId", c => c.Int());
            CreateIndex("dbo.Like", "ArticleId");
            CreateIndex("dbo.Like", "ArticleCommentId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Like", new[] { "ArticleCommentId" });
            DropIndex("dbo.Like", new[] { "ArticleId" });
            AlterColumn("dbo.Like", "ArticleCommentId", c => c.Int(nullable: false));
            AlterColumn("dbo.Like", "ArticleId", c => c.Int(nullable: false));
            CreateIndex("dbo.Like", "ArticleCommentId");
            CreateIndex("dbo.Like", "ArticleId");
            RenameTable(name: "dbo.Like", newName: "ArticleCommentLike");
        }
    }
}
