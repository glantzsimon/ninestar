namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUserReadingTables : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserProfileReading", "UserId", "dbo.User");
            DropForeignKey("dbo.UserCreditPack", "UserId", "dbo.User");
            DropForeignKey("dbo.UserProfileReading", "UserCreditPackId", "dbo.UserCreditPack");
            DropForeignKey("dbo.UserProfileReading", "UserMembershipId", "dbo.UserMembership");
            DropForeignKey("dbo.UserRelationshipCompatibilityReading", "UserId", "dbo.User");
            DropForeignKey("dbo.UserRelationshipCompatibilityReading", "UserCreditPackId", "dbo.UserCreditPack");
            DropForeignKey("dbo.UserRelationshipCompatibilityReading", "UserMembershipId", "dbo.UserMembership");
            DropIndex("dbo.UserProfileReading", new[] { "UserId" });
            DropIndex("dbo.UserProfileReading", new[] { "UserMembershipId" });
            DropIndex("dbo.UserProfileReading", new[] { "UserCreditPackId" });
            DropIndex("dbo.UserProfileReading", new[] { "Name" });
            DropIndex("dbo.UserCreditPack", new[] { "UserId" });
            DropIndex("dbo.UserCreditPack", new[] { "Name" });
            DropIndex("dbo.UserRelationshipCompatibilityReading", new[] { "UserId" });
            DropIndex("dbo.UserRelationshipCompatibilityReading", new[] { "UserMembershipId" });
            DropIndex("dbo.UserRelationshipCompatibilityReading", new[] { "UserCreditPackId" });
            DropIndex("dbo.UserRelationshipCompatibilityReading", new[] { "Name" });
            DropTable("dbo.UserProfileReading");
            DropTable("dbo.UserCreditPack");
            DropTable("dbo.UserRelationshipCompatibilityReading");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserRelationshipCompatibilityReading",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        UserMembershipId = c.Int(nullable: false),
                        UserCreditPackId = c.Int(),
                        FirstName = c.String(),
                        SecondName = c.String(),
                        FirstDateOfBirth = c.DateTime(nullable: false),
                        FirstGender = c.Int(nullable: false),
                        SecondDateOfBirth = c.DateTime(nullable: false),
                        SecondGender = c.Int(nullable: false),
                        IsHideSexuality = c.Boolean(nullable: false),
                        Name = c.String(maxLength: 128),
                        IsSystemStandard = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 255),
                        CreatedOn = c.DateTime(),
                        LastUpdatedBy = c.String(maxLength: 255),
                        LastUpdatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserCreditPack",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NumberOfCredits = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        TotalPrice = c.Double(nullable: false),
                        Name = c.String(maxLength: 128),
                        IsSystemStandard = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 255),
                        CreatedOn = c.DateTime(),
                        LastUpdatedBy = c.String(maxLength: 255),
                        LastUpdatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserProfileReading",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        UserMembershipId = c.Int(nullable: false),
                        UserCreditPackId = c.Int(),
                        FullName = c.String(),
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
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.UserRelationshipCompatibilityReading", "Name", unique: true);
            CreateIndex("dbo.UserRelationshipCompatibilityReading", "UserCreditPackId");
            CreateIndex("dbo.UserRelationshipCompatibilityReading", "UserMembershipId");
            CreateIndex("dbo.UserRelationshipCompatibilityReading", "UserId");
            CreateIndex("dbo.UserCreditPack", "Name", unique: true);
            CreateIndex("dbo.UserCreditPack", "UserId");
            CreateIndex("dbo.UserProfileReading", "Name", unique: true);
            CreateIndex("dbo.UserProfileReading", "UserCreditPackId");
            CreateIndex("dbo.UserProfileReading", "UserMembershipId");
            CreateIndex("dbo.UserProfileReading", "UserId");
            AddForeignKey("dbo.UserRelationshipCompatibilityReading", "UserMembershipId", "dbo.UserMembership", "Id");
            AddForeignKey("dbo.UserRelationshipCompatibilityReading", "UserCreditPackId", "dbo.UserCreditPack", "Id");
            AddForeignKey("dbo.UserRelationshipCompatibilityReading", "UserId", "dbo.User", "Id");
            AddForeignKey("dbo.UserProfileReading", "UserMembershipId", "dbo.UserMembership", "Id");
            AddForeignKey("dbo.UserProfileReading", "UserCreditPackId", "dbo.UserCreditPack", "Id");
            AddForeignKey("dbo.UserCreditPack", "UserId", "dbo.User", "Id");
            AddForeignKey("dbo.UserProfileReading", "UserId", "dbo.User", "Id");
        }
    }
}
