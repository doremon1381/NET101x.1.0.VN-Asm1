using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Donation.Models
{
    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        public string UserId { get; set; } = new Guid().ToString();
        public string FullName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false; // Default value is false

        public List<UserDonation> UserDonations { get; set; } = new List<UserDonation>();
    }
}
