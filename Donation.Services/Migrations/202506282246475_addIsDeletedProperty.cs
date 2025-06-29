namespace Donation.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addIsDeletedProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserDonation", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserProfile", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfile", "IsDeleted");
            DropColumn("dbo.UserDonation", "IsDeleted");
        }
    }
}
