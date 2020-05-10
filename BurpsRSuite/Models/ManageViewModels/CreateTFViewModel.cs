using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BurpsRSuite.Models.ManageViewModels
{
    public class CreateTFViewModel
    {
        public string QRCodeBase64 { get; set; }

        [Required]
        [MinLength(6)]
        public string Passcode { get; set; }
    }
}
