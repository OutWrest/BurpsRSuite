using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BurpsRSuite.Models
{
    public class ApplicationUser : IdentityUser
    {
        public byte[] TotpSecret { get; set; }
        public bool TotpEnabled { get; set; }


        public string Question1 { get; set; }
        public string Question2 { get; set; }
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
            VerifiedAccountNumber = this.AccountNumber.Trim().ToLower() == AccountNumber.Trim().ToLower();
            return VerifiedAccountNumber;
        }

        public bool VerifiedAccountNumber { get; set; }
        public bool CQVerified { get; set; }
        public bool TFVerified { get; set; }
    }
}
