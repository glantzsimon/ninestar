namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNameToProfileReadings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfileReading", "FullName", c => c.String());
            AddColumn("dbo.UserRelationshipCompatibilityReading", "FirstName", c => c.String());
            AddColumn("dbo.UserRelationshipCompatibilityReading", "SecondName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserRelationshipCompatibilityReading", "SecondName");
            DropColumn("dbo.UserRelationshipCompatibilityReading", "FirstName");
            DropColumn("dbo.UserProfileReading", "FullName");
        }
    }
}
