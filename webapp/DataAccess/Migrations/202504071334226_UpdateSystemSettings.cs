namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSystemSettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SystemSetting", "IsEnabledAlchemy", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SystemSetting", "IsEnabledAlchemy");
        }
    }
}
