using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Assignment1.ViewModel
{
    public class AccountLoginVM
    {
        [Required]
        [Display(Name = "Username/Email")]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}