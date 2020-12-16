namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbUpdate2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Contact",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(),
                        StripeCustomerId = c.String(),
                        FullName = c.String(nullable: false, maxLength: 128),
                        EmailAddress = c.String(nullable: false, maxLength: 255),
                        PhoneNumber = c.String(maxLength: 255),
                        CompanyName = c.String(maxLength: 255),
                        IsUnsubscribed = c.Boolean(nullable: false),
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
            
            CreateTable(
                "dbo.MembershipOption",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SubscriptionType = c.Int(nullable: false),
                        SubscriptionDetails = c.String(nullable: false),
                        Price = c.Double(nullable: false),
                        Name = c.String(maxLength: 128),
                        IsSystemStandard = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 255),
                        CreatedOn = c.DateTime(),
                        LastUpdatedBy = c.String(maxLength: 255),
                        LastUpdatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.UserMembership",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        MembershipOptionId = c.Int(nullable: false),
                        StartsOn = c.DateTime(nullable: false),
                        EndsOn = c.DateTime(nullable: false),
                        IsAutoRenew = c.Boolean(nullable: false),
                        Name = c.String(maxLength: 128),
                        IsSystemStandard = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 255),
                        CreatedOn = c.DateTime(),
                        LastUpdatedBy = c.String(maxLength: 255),
                        LastUpdatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MembershipOption", t => t.MembershipOptionId)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.MembershipOptionId)
                .Index(t => t.Name, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserMembership", "UserId", "dbo.User");
            DropForeignKey("dbo.UserMembership", "MembershipOptionId", "dbo.MembershipOption");
            DropForeignKey("dbo.Contact", "UserId", "dbo.User");
            DropIndex("dbo.UserMembership", new[] { "Name" });
            DropIndex("dbo.UserMembership", new[] { "MembershipOptionId" });
            DropIndex("dbo.UserMembership", new[] { "UserId" });
            DropIndex("dbo.MembershipOption", new[] { "Name" });
            DropIndex("dbo.Contact", new[] { "Name" });
            DropIndex("dbo.Contact", new[] { "UserId" });
            DropTable("dbo.UserMembership");
            DropTable("dbo.MembershipOption");
            DropTable("dbo.Contact");
        }
    }
}
