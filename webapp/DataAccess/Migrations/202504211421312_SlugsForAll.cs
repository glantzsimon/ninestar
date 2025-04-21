namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SlugsForAll : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Article", "Slug", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Article", "Slug");
        }
    }
}
