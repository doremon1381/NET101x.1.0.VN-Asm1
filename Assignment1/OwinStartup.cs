using Donation.IdentityServices;
using Donation.Services;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Owin;
using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(Assignment1.OwinStartup))]

namespace Assignment1
{
    public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            // add cookie authenticatoin
            app.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions()
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
            });

            this.SeedUserProfile();
        }

        private void SeedUserProfile()
        {
            var donationDbContext = new ApplicationDbContext();

            if (donationDbContext.UserProfiles.Count() == 0)
            {
                // I assume after the first migration, identity will have atleast one.
                var identityDbContext = new AppIdentityDbContext();
                foreach (var user in identityDbContext.Users)
                {
                    donationDbContext.UserProfiles.AddOrUpdate(new Donation.Models.UserProfile()
                    {
                        UserId = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        CreatedAt = user.CreateAt,
                        Address = user.Address
                    });
                }                

                donationDbContext.SaveChanges();
            }
        }
    }
}
