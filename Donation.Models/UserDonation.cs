using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Donation.Models
{
    [Table("UserDonation")]
    public class UserDonation
    {
        public string Id { get; set; } = new Guid().ToString();
        [Required]
        [DataType(DataType.Currency)]
        public decimal Money { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public UserDonationStatus Status { get; set; } = UserDonationStatus.Created; // Default status is Created

        [Required]
        public string UserId { get; set; }
        [Required]
        public string DonationId { get; set; }

        public bool IsDeleted { get; set; } = false; // Default value is false
    }

    public enum UserDonationStatus
    {
        Created,
        //Waitting,
        Approved,
        Rejected
    }
}
