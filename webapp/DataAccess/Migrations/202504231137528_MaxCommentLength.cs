namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MaxCommentLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ArticleComment", "Comment", c => c.String(nullable: false, maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ArticleComment", "Comment", c => c.String(nullable: false));
        }
    }
}
