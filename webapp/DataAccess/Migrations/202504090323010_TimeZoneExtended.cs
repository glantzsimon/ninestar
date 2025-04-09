namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TimeZoneExtended : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TimeZone", "WindowsTimeZoneId", c => c.String());
            AddColumn("dbo.TimeZone", "FriendlyName", c => c.String());
            AddColumn("dbo.TimeZone", "DisplayName", c => c.String());
            AddColumn("dbo.TimeZone", "Abbreviation", c => c.String());
            AddColumn("dbo.TimeZone", "UTCOffset", c => c.String());
            AddColumn("dbo.TimeZone", "DisplayOrder", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TimeZone", "DisplayOrder");
            DropColumn("dbo.TimeZone", "UTCOffset");
            DropColumn("dbo.TimeZone", "Abbreviation");
            DropColumn("dbo.TimeZone", "DisplayName");
            DropColumn("dbo.TimeZone", "FriendlyName");
            DropColumn("dbo.TimeZone", "WindowsTimeZoneId");
        }
    }
}
