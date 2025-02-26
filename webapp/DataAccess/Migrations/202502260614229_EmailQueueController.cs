namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmailQueueController : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailQueueItem", "IsProcessed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EmailQueueItem", "IsProcessed");
        }
    }
}
