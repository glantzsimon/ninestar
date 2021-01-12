namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConsultations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Consultation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContactId = c.Int(nullable: false),
                        ConsultationDuration = c.Int(nullable: false),
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
                .Index(t => t.ContactId)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.UserConsultation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ConsultationId = c.Int(nullable: false),
                        Name = c.String(maxLength: 128),
                        IsSystemStandard = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 255),
                        CreatedOn = c.DateTime(),
                        LastUpdatedBy = c.String(maxLength: 255),
                        LastUpdatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Consultation", t => t.ConsultationId)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ConsultationId)
                .Index(t => t.Name, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserConsultation", "UserId", "dbo.User");
            DropForeignKey("dbo.UserConsultation", "ConsultationId", "dbo.Consultation");
            DropForeignKey("dbo.Consultation", "ContactId", "dbo.Contact");
            DropIndex("dbo.UserConsultation", new[] { "Name" });
            DropIndex("dbo.UserConsultation", new[] { "ConsultationId" });
            DropIndex("dbo.UserConsultation", new[] { "UserId" });
            DropIndex("dbo.Consultation", new[] { "Name" });
            DropIndex("dbo.Consultation", new[] { "ContactId" });
            DropTable("dbo.UserConsultation");
            DropTable("dbo.Consultation");
        }
    }
}
