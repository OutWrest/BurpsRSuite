using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BurpsRSuite.Controllers
{
    public class StoreController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Browse(string type)
        {

            return View(type);
        }
        public IActionResult Details(string Id)
        {
            return View();
        }
    }
}