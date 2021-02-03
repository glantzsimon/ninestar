namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHideSexuality : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserRelationshipCompatibilityReading", "IsHideSexuality", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserRelationshipCompatibilityReading", "IsHideSexuality");
        }
    }
}
