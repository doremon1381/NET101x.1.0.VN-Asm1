namespace Donation.IdentityService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateIdentityUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "CreateAt", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "CreateAt");
        }
    }
}
