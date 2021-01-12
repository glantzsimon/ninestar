namespace K9.DataAccessLayer.Database
{
    using System.Data.Entity.Migrations;

    public partial class AddConsultationCompletedOn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Consultation", "CompletedOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Consultation", "CompletedOn");
        }
    }
}
