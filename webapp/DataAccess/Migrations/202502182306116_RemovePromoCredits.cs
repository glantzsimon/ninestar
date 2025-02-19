namespace K9.DataAccessLayer.Database
{
    using System.Data.Entity.Migrations;

    public partial class RemovePromoCredits : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.PromoCode", "Credits");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PromoCode", "Credits", c => c.Int(nullable: false));
        }
    }
}
