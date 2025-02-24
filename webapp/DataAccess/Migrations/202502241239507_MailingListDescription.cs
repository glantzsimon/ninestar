namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MailingListDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MailingList", "Details", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MailingList", "Details");
        }
    }
}
