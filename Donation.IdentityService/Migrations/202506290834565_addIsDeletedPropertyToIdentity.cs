namespace Donation.IdentityService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addIsDeletedPropertyToIdentity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "IsDeleted");
        }
    }
}
