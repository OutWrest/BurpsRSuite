using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BurpsRSuite.Models.AccountViewModels
{
    
    public class ChallengeQuestionsViewModel
    {
        public string ChallengeQuestion1 { get; set; }
        [Required]
        public string Answer1 { get; set; }

        public string ChallengeQuestion2 { get; set; }
        [Required]
        public string Answer2 { get; set; }

    }
}
