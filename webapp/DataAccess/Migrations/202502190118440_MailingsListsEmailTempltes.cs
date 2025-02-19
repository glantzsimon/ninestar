namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MailingsListsEmailTempltes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmailTemplate",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Subject = c.String(nullable: false, maxLength: 256),
                        HtmlBody = c.String(nullable: false),
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
                "dbo.MailingListContact",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MailingListId = c.Int(nullable: false),
                        ContactId = c.Int(nullable: false),
                        Name = c.String(maxLength: 128),
                        IsSystemStandard = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 255),
                        CreatedOn = c.DateTime(),
                        LastUpdatedBy = c.String(maxLength: 255),
                        LastUpdatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contact", t => t.ContactId)
                .ForeignKey("dbo.MailingList", t => t.MailingListId)
                .Index(t => t.MailingListId)
                .Index(t => t.ContactId)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.MailingList",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
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
                "dbo.MailingListUser",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MailingListId = c.Int(nullable: false),
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
                .ForeignKey("dbo.MailingList", t => t.MailingListId)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.MailingListId)
                .Index(t => t.UserId)
                .Index(t => t.Name, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MailingListUser", "UserId", "dbo.User");
            DropForeignKey("dbo.MailingListUser", "MailingListId", "dbo.MailingList");
            DropForeignKey("dbo.MailingListContact", "MailingListId", "dbo.MailingList");
            DropForeignKey("dbo.MailingListContact", "ContactId", "dbo.Contact");
            DropIndex("dbo.MailingListUser", new[] { "Name" });
            DropIndex("dbo.MailingListUser", new[] { "UserId" });
            DropIndex("dbo.MailingListUser", new[] { "MailingListId" });
            DropIndex("dbo.MailingList", new[] { "Name" });
            DropIndex("dbo.MailingListContact", new[] { "Name" });
            DropIndex("dbo.MailingListContact", new[] { "ContactId" });
            DropIndex("dbo.MailingListContact", new[] { "MailingListId" });
            DropIndex("dbo.EmailTemplate", new[] { "Name" });
            DropTable("dbo.MailingListUser");
            DropTable("dbo.MailingList");
            DropTable("dbo.MailingListContact");
            DropTable("dbo.EmailTemplate");
        }
    }
}
