namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserOTP : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserOTP",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UniqueIdentifier = c.Guid(nullable: false),
                        UserId = c.Int(nullable: false),
                        SixDigitCode = c.Int(nullable: false),
                        VerifiedOn = c.DateTime(),
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserOTP", "UserId", "dbo.User");
            DropIndex("dbo.UserOTP", new[] { "Name" });
            DropIndex("dbo.UserOTP", new[] { "UserId" });
            DropTable("dbo.UserOTP");
        }
    }
}
