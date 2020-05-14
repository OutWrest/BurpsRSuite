using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BurpsRSuite.Models.AdminViewModels
{
    public class IndexViewModel
    {
        public List<ApplicationUser> users { get; set; }

        public string StatusMessage { get; set; }
    }
}
