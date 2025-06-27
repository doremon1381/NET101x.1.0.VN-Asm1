namespace Donation.IdentityService.Migrations
{
    using Donation.IdentityModels;
    using Donation.IdentityServices;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Helpers;

    internal sealed class Configuration : DbMigrationsConfiguration<Donation.IdentityServices.AppIdentityDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Donation.IdentityServices.AppIdentityDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
            if (!context.Database.Exists())
            {
                context.Database.CreateIfNotExists();
            }

            // check if there are any roles in the database
            if (context.Roles.Count() == 0)
            {
                // if there is no role, then create an admin role
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                var adminRole = new IdentityRole()
                {
                    Name = Roles.ADMIN
                };
                var userRole = new IdentityRole()
                {
                    Name = Roles.USER
                };

                roleManager.Create(adminRole);
                roleManager.Create(userRole);
            }

            // Check if there are any users in the database
            if (context.Users.Count() == 0)
            {
                // if there is no user, then create an admin user
                var admin = new AppIdentityUser()
                {
                    UserName = "Admin",
                    PasswordHash = Crypto.HashPassword("admin123"),
                    FirstName = "Tuan",
                    LastName = "Nguyen",
                    Email = "doremon1380@gmail.com",
                    Active = true
                };

                var userStore = new AppUserStore(context);
                var userManager = new AppUserManager(userStore);

                var result = userManager.Create(admin);

                if (result.Succeeded)
                    // add admin role for this account
                    userManager.AddToRole(admin.Id, Roles.ADMIN);
            }
        }
    }
}
