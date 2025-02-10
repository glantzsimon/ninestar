namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSlots : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Slot",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartsOn = c.DateTime(nullable: false),
                        ConsultationDuration = c.Int(nullable: false),
                        IsTaken = c.Boolean(nullable: false),
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
            DropIndex("dbo.Slot", new[] { "Name" });
            DropTable("dbo.Slot");
        }
    }
}
