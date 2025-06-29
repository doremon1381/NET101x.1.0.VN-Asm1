using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Donation.IdentityModels
{
    //[Table("Users")]
    public class AppIdentityUser: IdentityUser
    {
        public string Notes { get; set; }
        public string Status { get; set; }
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
        [MaxLength(50)]
        public string FirstName { get; set; }
        [MaxLength(50)]
        public string LastName { get; set; }
        public string Address { get; set; }
        [Required]
        public bool Active { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
    }
}
