namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSlugs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tag", "Slug", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tag", "Slug");
        }
    }
}
