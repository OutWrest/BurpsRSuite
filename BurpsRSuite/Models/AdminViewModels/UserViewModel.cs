using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BurpsRSuite.Models.AdminViewModels
{
    public class UserViewModel
    {
        public ApplicationUser user { get; set; }

        public string Id { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [PersonalData]
        [Display(Name = "First name")]
        public string FirstName { get; set; }


        [PersonalData]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        public string Captcha { get; set; }

        [Compare(nameof(CaptchaIn), ErrorMessage = "The Captcha does not match.")]
        public string CaptchaIn { get; set; }
        public string StatusMessage { get; set; }
    }
}
