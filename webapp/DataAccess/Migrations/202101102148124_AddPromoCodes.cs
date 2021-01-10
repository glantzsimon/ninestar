namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPromoCodes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PromoCode",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 10),
                        Credits = c.Int(nullable: false),
                        SubscriptionType = c.Int(nullable: false),
                        Name = c.String(maxLength: 128),
                        IsSystemStandard = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 255),
                        CreatedOn = c.DateTime(),
                        LastUpdatedBy = c.String(maxLength: 255),
                        LastUpdatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.UserPromoCode",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PromoCodeId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Name = c.String(maxLength: 128),
                        IsSystemStandard = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 255),
                        CreatedOn = c.DateTime(),
                        LastUpdatedBy = c.String(maxLength: 255),
                        LastUpdatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PromoCode", t => t.PromoCodeId)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.PromoCodeId)
                .Index(t => t.UserId)
                .Index(t => t.Name, unique: true);
            
            CreateIndex("dbo.Contact", "EmailAddress", unique: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserPromoCode", "UserId", "dbo.User");
            DropForeignKey("dbo.UserPromoCode", "PromoCodeId", "dbo.PromoCode");
            DropIndex("dbo.UserPromoCode", new[] { "Name" });
            DropIndex("dbo.UserPromoCode", new[] { "UserId" });
            DropIndex("dbo.UserPromoCode", new[] { "PromoCodeId" });
            DropIndex("dbo.PromoCode", new[] { "Name" });
            DropIndex("dbo.PromoCode", new[] { "Code" });
            DropIndex("dbo.Contact", new[] { "EmailAddress" });
            DropTable("dbo.UserPromoCode");
            DropTable("dbo.PromoCode");
        }
    }
}
