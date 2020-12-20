namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMemberships : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserProfileReading",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        UserMembershipId = c.Int(nullable: false),
                        DateOfBirth = c.DateTime(nullable: false),
                        Gender = c.Int(nullable: false),
                        Name = c.String(maxLength: 128),
                        IsSystemStandard = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 255),
                        CreatedOn = c.DateTime(),
                        LastUpdatedBy = c.String(maxLength: 255),
                        LastUpdatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId)
                .ForeignKey("dbo.UserMembership", t => t.UserMembershipId)
                .Index(t => t.UserId)
                .Index(t => t.UserMembershipId)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.UserRelationshipCompatibilityReading",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        UserMembershipId = c.Int(nullable: false),
                        FirstDateOfBirth = c.DateTime(nullable: false),
                        FirstGender = c.Int(nullable: false),
                        SecondDateOfBirth = c.DateTime(nullable: false),
                        SecondGender = c.Int(nullable: false),
                        Name = c.String(maxLength: 128),
                        IsSystemStandard = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 255),
                        CreatedOn = c.DateTime(),
                        LastUpdatedBy = c.String(maxLength: 255),
                        LastUpdatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId)
                .ForeignKey("dbo.UserMembership", t => t.UserMembershipId)
                .Index(t => t.UserId)
                .Index(t => t.UserMembershipId)
                .Index(t => t.Name, unique: true);
            
            AddColumn("dbo.MembershipOption", "MaxNumberOfProfileReadings", c => c.Int(nullable: false));
            AddColumn("dbo.MembershipOption", "MaxNumberOfCompatibilityReadings", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserRelationshipCompatibilityReading", "UserMembershipId", "dbo.UserMembership");
            DropForeignKey("dbo.UserRelationshipCompatibilityReading", "UserId", "dbo.User");
            DropForeignKey("dbo.UserProfileReading", "UserMembershipId", "dbo.UserMembership");
            DropForeignKey("dbo.UserProfileReading", "UserId", "dbo.User");
            DropIndex("dbo.UserRelationshipCompatibilityReading", new[] { "Name" });
            DropIndex("dbo.UserRelationshipCompatibilityReading", new[] { "UserMembershipId" });
            DropIndex("dbo.UserRelationshipCompatibilityReading", new[] { "UserId" });
            DropIndex("dbo.UserProfileReading", new[] { "Name" });
            DropIndex("dbo.UserProfileReading", new[] { "UserMembershipId" });
            DropIndex("dbo.UserProfileReading", new[] { "UserId" });
            DropColumn("dbo.MembershipOption", "MaxNumberOfCompatibilityReadings");
            DropColumn("dbo.MembershipOption", "MaxNumberOfProfileReadings");
            DropTable("dbo.UserRelationshipCompatibilityReading");
            DropTable("dbo.UserProfileReading");
        }
    }
}
