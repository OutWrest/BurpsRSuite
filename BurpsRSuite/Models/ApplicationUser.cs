using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BurpsRSuite.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ChallengeQuestion Question1 { get; set; }
        public virtual ChallengeQuestion Question2 { get; set; }
        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
    }
}
