namespace Donation.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addDonationTitle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Donations", "Title", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Donations", "Title");
        }
    }
}
