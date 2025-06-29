using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Donation.Services
{
    public class ApplicationDbContext : DbContext
    {
        // This class can be used to implement application-related services.
        // For example, you can add methods for managing application data, etc.
        // Currently, it is empty and serves as a placeholder for future implementations.
        public ApplicationDbContext() : base("name=DonationDbContext")
        {
            // Initialize the database context with the connection string named "DonationDbContext"
        }
        // Add DbSet properties for your entities here, e.g.:
        // public DbSet<YourEntity> YourEntities { get; set; }
        public DbSet<Donation.Models.UserProfile> UserProfiles { get; set; }
        public DbSet<Donation.Models.Donation> Donations { get; set; }
        public DbSet<Donation.Models.UserDonation> UserDonations { get; set; }

        // You can also override OnModelCreating method to configure your model
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // TODO
            //modelBuilder.Entity<Models.UserDonation>()
            //    .HasOptional(c => c.UserProfile)
            //    .WithMany(p => p.UserDonations)
            //    .HasForeignKey(c => c.UserId)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Models.UserDonation>()
            //    .HasOptional(c => c.Donation)
            //    .WithMany(p => p.UserDonations)
            //    .HasForeignKey(c => c.DonationId)
            //    .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
            // Add custom configurations here if needed
        }
    }
}
