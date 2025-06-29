namespace Donation.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateUserDonationProperty : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserDonation", "Money", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserDonation", "Money", c => c.String());
        }
    }
}
