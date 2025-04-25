namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddComplementaryReadings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserMembership", "ComplementaryPersonalChartReadingCount", c => c.Int(nullable: false));
            AddColumn("dbo.UserMembership", "ComplementaryPredictionsReadingCount", c => c.Int(nullable: false));
            AddColumn("dbo.UserMembership", "ComplementaryCompatibilityReadingCount", c => c.Int(nullable: false));
            DropColumn("dbo.MembershipOption", "NumberOfProfileReadings");
            DropColumn("dbo.MembershipOption", "NumberOfCompatibilityReadings");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MembershipOption", "NumberOfCompatibilityReadings", c => c.Int(nullable: false));
            AddColumn("dbo.MembershipOption", "NumberOfProfileReadings", c => c.Int(nullable: false));
            DropColumn("dbo.UserMembership", "ComplementaryCompatibilityReadingCount");
            DropColumn("dbo.UserMembership", "ComplementaryPredictionsReadingCount");
            DropColumn("dbo.UserMembership", "ComplementaryPersonalChartReadingCount");
        }
    }
}
