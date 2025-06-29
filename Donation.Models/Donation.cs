using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Donation.Models
{
    [Table("Donations")]
    public class Donation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Code { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Description { get; set; }
        [Required]
        public string Title { get; set; }
        //[Required]
        //[DataType(DataType.Currency)]
        //public decimal ExpectedMoney { get; set; }
        [DataType(DataType.Currency)]
        public decimal TotalMoney { get; set; }
        //public string Currency { get; set; } = "USD"; // Default currency is USD
        public string OrganizationName { get; set; }
        public string OrganizationPhone { get; set; } = string.Empty;
        public DonationStatus Status { get; set; } = DonationStatus.Created; // Default status is Pending
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public bool IsDeleted { get; set; } = false; // Default value is false

        public List<UserDonation> UserDonations { get; set; } = new List<UserDonation>();
    }

    public enum DonationStatus
    {
        //Approved,
        Created,
        Open,
        Done,
        Closed
    }
}
