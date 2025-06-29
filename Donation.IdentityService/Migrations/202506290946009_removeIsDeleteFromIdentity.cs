namespace Donation.IdentityService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeIsDeleteFromIdentity : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "IsDeleted");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "IsDeleted", c => c.Boolean(nullable: false));
        }
    }
}
