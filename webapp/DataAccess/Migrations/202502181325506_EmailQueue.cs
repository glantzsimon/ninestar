namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmailQueue : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmailQueueItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RecipientEmailAddress = c.String(nullable: false, maxLength: 255),
                        RecipientName = c.String(nullable: false, maxLength: 128),
                        Subject = c.String(nullable: false),
                        Body = c.String(nullable: false),
                        SentOn = c.DateTime(),
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
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.EmailQueueItem", new[] { "Name" });
            DropTable("dbo.EmailQueueItem");
        }
    }
}
