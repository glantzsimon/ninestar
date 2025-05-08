namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserPreferences : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserPreference",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Key = c.String(nullable: false),
                        Value = c.String(),
                        ValueType = c.String(nullable: false),
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
            DropForeignKey("dbo.UserPreference", "UserId", "dbo.User");
            DropIndex("dbo.UserPreference", new[] { "Name" });
            DropIndex("dbo.UserPreference", new[] { "UserId" });
            DropTable("dbo.UserPreference");
        }
    }
}
