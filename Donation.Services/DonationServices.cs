using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Donation.Models;
using System.Data.Entity;

namespace Donation.Services
{
    public class DonationServices : IDonationServices
    {
        private readonly ApplicationDbContext _dbContext;
        public DonationServices()
        {
            _dbContext = new ApplicationDbContext();
        }

        public bool CreateDonation(Models.Donation donation)
        {
            if (!ValidateDonation(donation))
                throw new ArgumentException("Invalid donation data", nameof(donation));

            _dbContext.Donations.Add(donation);
            return _dbContext.SaveChanges() > 0; // Returns true if the creation was successful
        }

        private static bool ValidateDonation(Models.Donation donation)
        {
            try
            {
                if (donation == null)
                    throw new ArgumentNullException(nameof(donation), "Donation cannot be null");
                if (string.IsNullOrWhiteSpace(donation.Code))
                    throw new ArgumentException("Donation code cannot be empty", nameof(donation.Code));
                if (string.IsNullOrWhiteSpace(donation.Title))
                    throw new ArgumentException("Donation title cannot be empty", nameof(donation.Title));
                if (donation.StartDate >= donation.EndDate)
                    throw new ArgumentException("Start date must be before end date", nameof(donation));

                return true;
            }
            catch (Exception)
            {
                return false; // Validation failed, return false
            }            
        }

        public void CreateSampleDonation()
        {
            for (int i = 0; i < 10; i++)
            {
                var donation = new Models.Donation()
                {
                    Id = Guid.NewGuid().ToString(),
                    Code = $"DON{i}" + " " + DateTime.Now.ToString("yyyy-MM-dd"),
                    CreatedAt = DateTime.Now,
                    Description = "Sample donation plan for testing purposes.",
                    Title = $"Sample Donation Plan {i}",
                    TotalMoney = 1000.00m + i * 1000,
                    OrganizationName = $"Sample Organization {i}",
                    OrganizationPhone = $"123-456-789{i}",
                    Status = DonationStatus.Created,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(1)
                };

                _dbContext.Donations.Add(donation);
                _dbContext.SaveChanges();
            }
        }

        public Models.Donation FindById(string donationId, bool userDonationsRequired = false)
        {
            if (string.IsNullOrWhiteSpace(donationId))
                throw new ArgumentException("Donation ID cannot be null or empty", nameof(donationId));

            if (userDonationsRequired)
                return _dbContext.Donations.Include(d => d.UserDonations).Where(d => !d.IsDeleted).First(d => d.Id == donationId);
            else
                return _dbContext.Donations.Where(d => !d.IsDeleted).FirstOrDefault(d => d.Id == donationId);
        }

        public IEnumerable<Models.Donation> GetDonationPlans()
        {
            return _dbContext.Donations.Where(d => !d.IsDeleted);
        }

        public bool SoftDelete(string donationId)
        {
            var donation = _dbContext.Donations.Find(donationId);
            if (donation == null)
                return false; // Donation not found
            donation.IsDeleted = true; // Mark as deleted
            _dbContext.Donations.AddOrUpdate(donation);
            return _dbContext.SaveChanges() > 0; // Returns true if the deletion was successful
        }

        public bool UpdateDonation(Models.Donation modifiedDonation)
        {
            var donation = _dbContext.Donations.Find(modifiedDonation.Id);
            if (donation.Status == DonationStatus.Closed)
                throw new Exception("This event is closed and can not be modified!");

            donation.Code = modifiedDonation.Code;
            donation.Title = modifiedDonation.Title;
            donation.StartDate = modifiedDonation.StartDate;
            donation.EndDate = modifiedDonation.EndDate;
            donation.OrganizationName = modifiedDonation.OrganizationName;
            donation.OrganizationPhone = modifiedDonation.OrganizationPhone;
            //donation.Content = modifiedDonation.Content;
            donation.Description = modifiedDonation.Description;

            _dbContext.Donations.AddOrUpdate(donation);
            
            return _dbContext.SaveChanges() > 0; // Returns true if the update was successful
        }

        public bool UpdateDonationStatus(string donationId, string donationStatus)
        {
            var donation = _dbContext.Donations.Where(d => !d.IsDeleted).First(d => d.Id == donationId);
            if (donation == null)
                return false; // Donation not found

            if (Enum.TryParse(donationStatus, out DonationStatus status))
            {
                if (donation.Status > status)
                    throw new Exception("update status can only go forward");

                donation.Status = status;
                _dbContext.SaveChanges();
                return true; // Status updated successfully
            }
            else
            {
                return false; // Invalid status provided
            }
        }

        public void CreateSampleUserDonations(string donationId)
        {
            if (string.IsNullOrWhiteSpace(donationId))
                throw new ArgumentException("Donation ID cannot be null or empty", nameof(donationId));

            var donation = _dbContext.Donations.Where(d => !d.IsDeleted).First(d => d.Id == donationId);

            if (donation == null)
                throw new Exception("Donation not found");

            var users = _dbContext.UserProfiles.ToList();

            int i = 0;
            foreach(var user in users)
            {
                var userDonation = new Models.UserDonation()
                {
                    Id = Guid.NewGuid().ToString(),
                    DonationId = donation.Id,
                    UserId = user.UserId, // Simulating a user ID
                    Money = 10000 + i++ * 10000, // Sample amount
                    CreatedAt = DateTime.Now,
                    Status = UserDonationStatus.Created
                };
                _dbContext.UserDonations.AddOrUpdate(userDonation);
            }
            _dbContext.SaveChanges();
        }

        public void CreateSampleUserProfiles(List<UserProfile> userProfiles)
        {
            try
            {
                foreach (var userProfile in userProfiles)
                {
                    _dbContext.UserProfiles.AddOrUpdate(userProfile);
                    _dbContext.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public UserDonation FindUserDonationById(string id, bool includeRelatedTable = false)
        {
            return _dbContext.UserDonations
                //.Include(ud => ud.Donation)
                //.Include(ud => ud.UserProfile)
                .Where(ud => !ud.IsDeleted)
                .FirstOrDefault(ud => ud.Id == id);
        }

        public bool UpdateUserDonation(string id, string status)
        {
            var userDonation = FindUserDonationById(id);
            if (userDonation == null)
                return false; // User donation not found

            if (Enum.TryParse(status, out UserDonationStatus userStatus))
            {
                userDonation.Status = userStatus;
                _dbContext.UserDonations.AddOrUpdate(userDonation);
                _dbContext.SaveChanges();

                UpdateDonationPlansAfterUserDonationIsAccepted(userDonation, userStatus);
            }

            return true; // Status updated successfully
        }

        private void UpdateDonationPlansAfterUserDonationIsAccepted(UserDonation userDonation, UserDonationStatus userStatus)
        {
            var donation = _dbContext.Donations.Where(d => !d.IsDeleted).First(d => d.Id == userDonation.DonationId);

            if (donation == null)
                throw new Exception("Donation not found");

            if (userStatus == UserDonationStatus.Approved)
            {
                if (donation.Status == DonationStatus.Open)
                {
                    donation.TotalMoney += userDonation.Money;

                    _dbContext.Donations.AddOrUpdate(donation);
                    _dbContext.SaveChanges(); // Returns true if the update was successful
                }
                else
                {
                    throw new Exception("Donation status must be Open to accept user donations.");
                }
            }
        }
    }

    public interface IDonationServices
    {
        bool CreateDonation(Models.Donation donation);
        void CreateSampleDonation();
        void CreateSampleUserDonations(string donationId);
        void CreateSampleUserProfiles(List<UserProfile> userProfiles);
        Donation.Models.Donation FindById(string donationId, bool userDonationsRequired = false);
        UserDonation FindUserDonationById(string id, bool includeRelatedTable = false);
        IEnumerable<Donation.Models.Donation> GetDonationPlans();
        bool SoftDelete(string donationId);
        bool UpdateDonation(Models.Donation donation);
        bool UpdateDonationStatus(string donationId, string donationStatus);
        bool UpdateUserDonation(string id, string status);
    }
}
