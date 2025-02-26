namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmailQueueItemResult : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailQueueItem", "Result", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EmailQueueItem", "Result");
        }
    }
}
