namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CleanUp : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.NineStarKiPersonalProfile", new[] { "Name" });
            DropTable("dbo.NineStarKiPersonalProfile");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.NineStarKiPersonalProfile",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Energy = c.Int(nullable: false),
                        EnergyType = c.Int(nullable: false),
                        Trigram = c.String(nullable: false),
                        MainEnergyDescription = c.String(nullable: false),
                        EmotionalEnergyDescription = c.String(),
                        SurfaceEnergyDescription = c.String(),
                        Health = c.String(),
                        Occupations = c.String(),
                        PersonalDevelopemnt = c.String(),
                        Examples = c.String(),
                        Language = c.Int(nullable: false),
                        Name = c.String(maxLength: 128),
                        IsSystemStandard = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 255),
                        CreatedOn = c.DateTime(),
                        LastUpdatedBy = c.String(maxLength: 255),
                        LastUpdatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.NineStarKiPersonalProfile", "Name", unique: true);
        }
    }
}
