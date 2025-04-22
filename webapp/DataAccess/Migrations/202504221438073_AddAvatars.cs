namespace K9.DataAccessLayer.Database
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAvatars : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserInfo", "AvatarImageUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserInfo", "AvatarImageUrl");
        }
    }
}
