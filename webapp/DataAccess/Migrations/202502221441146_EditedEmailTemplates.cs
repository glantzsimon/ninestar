namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditedEmailTemplates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailTemplate", "SystemEmailTemplate", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EmailTemplate", "SystemEmailTemplate");
        }
    }
}
