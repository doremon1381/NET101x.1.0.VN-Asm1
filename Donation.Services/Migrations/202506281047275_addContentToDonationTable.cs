namespace Donation.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addContentToDonationTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Donations", "Content", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Donations", "Content");
        }
    }
}
