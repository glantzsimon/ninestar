namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GenderToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "Gender", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "Gender");
        }
    }
}
