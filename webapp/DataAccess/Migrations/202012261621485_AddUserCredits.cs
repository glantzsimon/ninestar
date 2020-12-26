namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserCredits : DbMigration
    {
        public override void Up()
        {
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.Name, unique: true);
            
            AddColumn("dbo.UserProfileReading", "UserCreditPackId", c => c.Int());
            AddColumn("dbo.UserRelationshipCompatibilityReading", "UserCreditPackId", c => c.Int());
            CreateIndex("dbo.UserProfileReading", "UserCreditPackId");
            CreateIndex("dbo.UserRelationshipCompatibilityReading", "UserCreditPackId");
            AddForeignKey("dbo.UserProfileReading", "UserCreditPackId", "dbo.UserCreditPack", "Id");
            AddForeignKey("dbo.UserRelationshipCompatibilityReading", "UserCreditPackId", "dbo.UserCreditPack", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserRelationshipCompatibilityReading", "UserCreditPackId", "dbo.UserCreditPack");
            DropForeignKey("dbo.UserProfileReading", "UserCreditPackId", "dbo.UserCreditPack");
            DropForeignKey("dbo.UserCreditPack", "UserId", "dbo.User");
            DropIndex("dbo.UserRelationshipCompatibilityReading", new[] { "UserCreditPackId" });
            DropIndex("dbo.UserCreditPack", new[] { "Name" });
            DropIndex("dbo.UserCreditPack", new[] { "UserId" });
            DropIndex("dbo.UserProfileReading", new[] { "UserCreditPackId" });
            DropColumn("dbo.UserRelationshipCompatibilityReading", "UserCreditPackId");
            DropColumn("dbo.UserProfileReading", "UserCreditPackId");
            DropTable("dbo.UserCreditPack");
        }
    }
}
