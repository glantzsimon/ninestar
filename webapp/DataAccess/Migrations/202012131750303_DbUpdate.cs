namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbUpdate : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.EnergyInfo", newName: "NineStarKiPersonalProfile");
            AddColumn("dbo.NineStarKiPersonalProfile", "MainEnergyDescription", c => c.String(nullable: false));
            AddColumn("dbo.NineStarKiPersonalProfile", "Summary", c => c.String(nullable: false));
            AddColumn("dbo.NineStarKiPersonalProfile", "EmotionalEnergyDescription", c => c.String());
            AddColumn("dbo.NineStarKiPersonalProfile", "SurfaceEnergyDescription", c => c.String());
            DropColumn("dbo.NineStarKiPersonalProfile", "EnergyDescription");
            DropColumn("dbo.NineStarKiPersonalProfile", "Childhood");
        }
        
        public override void Down()
        {
            AddColumn("dbo.NineStarKiPersonalProfile", "Childhood", c => c.String());
            AddColumn("dbo.NineStarKiPersonalProfile", "EnergyDescription", c => c.String(nullable: false));
            DropColumn("dbo.NineStarKiPersonalProfile", "SurfaceEnergyDescription");
            DropColumn("dbo.NineStarKiPersonalProfile", "EmotionalEnergyDescription");
            DropColumn("dbo.NineStarKiPersonalProfile", "Summary");
            DropColumn("dbo.NineStarKiPersonalProfile", "MainEnergyDescription");
            RenameTable(name: "dbo.NineStarKiPersonalProfile", newName: "EnergyInfo");
        }
    }
}
