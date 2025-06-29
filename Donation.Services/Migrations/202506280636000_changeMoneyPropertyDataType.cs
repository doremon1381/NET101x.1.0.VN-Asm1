namespace Donation.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeMoneyPropertyDataType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Donations", "ExpectedMoney", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Donations", "TotalMoney", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Donations", "TotalMoney", c => c.String());
            AlterColumn("dbo.Donations", "ExpectedMoney", c => c.String());
        }
    }
}
