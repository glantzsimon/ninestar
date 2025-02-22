namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePromoCodes : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.PromoCode", newName: "Promotion");
            DropForeignKey("dbo.UserPromoCode", "PromoCodeId", "dbo.PromoCode");
            DropForeignKey("dbo.UserPromoCode", "UserId", "dbo.User");
            DropIndex("dbo.UserPromoCode", new[] { "PromoCodeId" });
            DropIndex("dbo.UserPromoCode", new[] { "UserId" });
            DropIndex("dbo.UserPromoCode", new[] { "Name" });
            CreateTable(
                "dbo.UserPromotion",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PromotionId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        SentOn = c.DateTime(nullable: false),
                        UsedOn = c.DateTime(),
                        Name = c.String(maxLength: 128),
                        IsSystemStandard = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 255),
                        CreatedOn = c.DateTime(),
                        LastUpdatedBy = c.String(maxLength: 255),
                        LastUpdatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Promotion", t => t.PromotionId)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.PromotionId)
                .Index(t => t.UserId)
                .Index(t => t.Name, unique: true);
            
            AddColumn("dbo.EmailTemplate", "PromotionId", c => c.Int(nullable: false));
            AddColumn("dbo.Promotion", "SpecialPrice", c => c.Double(nullable: false));
            CreateIndex("dbo.EmailTemplate", "PromotionId");
            AddForeignKey("dbo.EmailTemplate", "PromotionId", "dbo.Promotion", "Id");
            DropColumn("dbo.EmailTemplate", "Discount");
            DropColumn("dbo.Promotion", "SentOn");
            DropColumn("dbo.Promotion", "UsedOn");
            DropColumn("dbo.Promotion", "TotalPrice");
            DropTable("dbo.UserPromoCode");
        }
        
        public override void Down()
        {
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
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Promotion", "TotalPrice", c => c.Double(nullable: false));
            AddColumn("dbo.Promotion", "UsedOn", c => c.DateTime());
            AddColumn("dbo.Promotion", "SentOn", c => c.DateTime());
            AddColumn("dbo.EmailTemplate", "Discount", c => c.Int());
            DropForeignKey("dbo.UserPromotion", "UserId", "dbo.User");
            DropForeignKey("dbo.UserPromotion", "PromotionId", "dbo.Promotion");
            DropForeignKey("dbo.EmailTemplate", "PromotionId", "dbo.Promotion");
            DropIndex("dbo.UserPromotion", new[] { "Name" });
            DropIndex("dbo.UserPromotion", new[] { "UserId" });
            DropIndex("dbo.UserPromotion", new[] { "PromotionId" });
            DropIndex("dbo.EmailTemplate", new[] { "PromotionId" });
            DropColumn("dbo.Promotion", "SpecialPrice");
            DropColumn("dbo.EmailTemplate", "PromotionId");
            DropTable("dbo.UserPromotion");
            CreateIndex("dbo.UserPromoCode", "Name", unique: true);
            CreateIndex("dbo.UserPromoCode", "UserId");
            CreateIndex("dbo.UserPromoCode", "PromoCodeId");
            AddForeignKey("dbo.UserPromoCode", "UserId", "dbo.User", "Id");
            AddForeignKey("dbo.UserPromoCode", "PromoCodeId", "dbo.PromoCode", "Id");
            RenameTable(name: "dbo.Promotion", newName: "PromoCode");
        }
    }
}
