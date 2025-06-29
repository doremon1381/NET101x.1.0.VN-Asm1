namespace Donation.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeDonationExpectedMoney : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Donations", "ExpectedMoney");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Donations", "ExpectedMoney", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
