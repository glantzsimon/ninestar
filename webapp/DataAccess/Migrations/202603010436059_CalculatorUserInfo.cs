namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CalculatorUserInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserInfo", "CalculationMethod", c => c.Int(nullable: false));
            AddColumn("dbo.UserInfo", "CalculatorType", c => c.Int(nullable: false));
            AddColumn("dbo.UserInfo", "HousesDisplay", c => c.Int(nullable: false));
            AddColumn("dbo.UserInfo", "InvertDailyAndHourlyKiForSouthernHemisphere", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserInfo", "InvertDailyAndHourlyCycleKiForSouthernHemisphere", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserInfo", "InvertDailyAndHourlyCycleKiForSouthernHemisphere");
            DropColumn("dbo.UserInfo", "InvertDailyAndHourlyKiForSouthernHemisphere");
            DropColumn("dbo.UserInfo", "HousesDisplay");
            DropColumn("dbo.UserInfo", "CalculatorType");
            DropColumn("dbo.UserInfo", "CalculationMethod");
        }
    }
}
