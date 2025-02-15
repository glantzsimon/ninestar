namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MissingOTP : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserOTP", "UniqueIdentifier", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserOTP", "UniqueIdentifier", c => c.Guid(nullable: false));
        }
    }
}
