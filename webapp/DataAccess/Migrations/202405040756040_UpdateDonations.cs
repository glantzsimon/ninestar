namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDonations : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Donation", "StripeId");
            DropColumn("dbo.Donation", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Donation", "Status", c => c.String());
            AddColumn("dbo.Donation", "StripeId", c => c.String());
        }
    }
}
