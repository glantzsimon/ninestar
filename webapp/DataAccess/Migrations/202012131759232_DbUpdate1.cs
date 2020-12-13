namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbUpdate1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.NineStarKiPersonalProfile", "Summary");
        }
        
        public override void Down()
        {
            AddColumn("dbo.NineStarKiPersonalProfile", "Summary", c => c.String(nullable: false));
        }
    }
}
