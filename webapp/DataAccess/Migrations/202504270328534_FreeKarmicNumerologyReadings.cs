namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FreeKarmicNumerologyReadings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserMembership", "ComplementaryKarmicReadingCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserMembership", "ComplementaryKarmicReadingCount");
        }
    }
}
