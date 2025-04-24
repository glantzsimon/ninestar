namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsRejectedToComment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ArticleComment", "IsRejected", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ArticleComment", "IsRejected");
        }
    }
}
