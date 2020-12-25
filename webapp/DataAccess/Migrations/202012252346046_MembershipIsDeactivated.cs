namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MembershipIsDeactivated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserMembership", "IsDeactivated", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserMembership", "IsDeactivated");
        }
    }
}
