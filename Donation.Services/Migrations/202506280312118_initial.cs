namespace Donation.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Donations",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Code = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        Description = c.String(),
                        ExpectedMoney = c.String(),
                        TotalMoney = c.String(),
                        OrganizationName = c.String(),
                        OrganizationPhone = c.String(),
                        Status = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserDonation",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Money = c.String(),
                        Note = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        DonationId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Donations", t => t.DonationId, cascadeDelete: true)
                .ForeignKey("dbo.UserProfile", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.DonationId);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        FullName = c.String(),
                        PhoneNumber = c.String(),
                        Address = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserDonation", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.UserDonation", "DonationId", "dbo.Donations");
            DropIndex("dbo.UserDonation", new[] { "DonationId" });
            DropIndex("dbo.UserDonation", new[] { "UserId" });
            DropTable("dbo.UserProfile");
            DropTable("dbo.UserDonation");
            DropTable("dbo.Donations");
        }
    }
}
