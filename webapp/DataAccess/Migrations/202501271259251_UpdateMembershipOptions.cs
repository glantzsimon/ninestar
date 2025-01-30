namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMembershipOptions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MembershipOption", "NumberOfProfileReadings", c => c.Int(nullable: false));
            AddColumn("dbo.MembershipOption", "NumberOfCompatibilityReadings", c => c.Int(nullable: false));
            DropColumn("dbo.MembershipOption", "MaxNumberOfProfileReadings");
            DropColumn("dbo.MembershipOption", "MaxNumberOfCompatibilityReadings");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MembershipOption", "MaxNumberOfCompatibilityReadings", c => c.Int(nullable: false));
            AddColumn("dbo.MembershipOption", "MaxNumberOfProfileReadings", c => c.Int(nullable: false));
            DropColumn("dbo.MembershipOption", "NumberOfCompatibilityReadings");
            DropColumn("dbo.MembershipOption", "NumberOfProfileReadings");
        }
    }
}
