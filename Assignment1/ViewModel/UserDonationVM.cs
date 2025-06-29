using Donation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assignment1.ViewModel
{
    public class UserDonationVM
    {
        public string Id { get; set; }
        public decimal Money { get; set; }
        public string Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserDonationStatus Status { get; set; }

        public string UserFullName { get; set; }

    }
}