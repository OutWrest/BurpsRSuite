using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BurpsRSuite.Models
{
    public class Item
    {
        public int Id { get; set; }

        public decimal Price { get; set; }
        public string Title { get; set; }
        public ApplicationUser Author { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
    }
}
