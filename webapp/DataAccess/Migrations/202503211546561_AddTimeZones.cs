namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTimeZones : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TimeZone",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimeZoneId = c.String(),
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
            DropIndex("dbo.TimeZone", new[] { "Name" });
            DropTable("dbo.TimeZone");
        }
    }
}
