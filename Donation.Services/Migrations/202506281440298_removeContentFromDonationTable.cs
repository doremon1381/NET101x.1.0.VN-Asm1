namespace Donation.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeContentFromDonationTable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Donations", "Content");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Donations", "Content", c => c.String());
        }
    }
}
