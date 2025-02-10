namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateConsulation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Consultation", "ScheduledOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Consultation", "ScheduledOn");
        }
    }
}
