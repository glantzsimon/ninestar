namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CalculatorUserInfo2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.UserInfo", "InvertDailyAndHourlyCycleKiForSouthernHemisphere");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserInfo", "InvertDailyAndHourlyCycleKiForSouthernHemisphere", c => c.Boolean(nullable: false));
        }
    }
}
