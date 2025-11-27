namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MailingList_SqlQuery : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MailingList", "SqlQuery", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MailingList", "SqlQuery");
        }
    }
}
