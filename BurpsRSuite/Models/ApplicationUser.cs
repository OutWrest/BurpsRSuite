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

        [PersonalData]
        public string FirstName { get; set; }

        [PersonalData]
        public string LastName { get; set; }

        public string AccountNumber { get; set; }
        public bool HasSetupChallengeQuestions()
        {
            return Answer1 != null && Answer2 != null;
        }
        public bool VerifyChallengeAnswers(string answer1, string answer2)
        {
            return Answer1.Trim().ToLower() == answer1.Trim().ToLower() && Answer2.Trim().ToLower() == answer2.Trim().ToLower();
        }

        public bool VerifyAccountNumber(string AccountNumber)
        {
            return this.AccountNumber.Trim().ToLower() == AccountNumber.Trim().ToLower();
        }
    }
}
