using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurpsRSuite.Data;
using BurpsRSuite.Models;
using BurpsRSuite.Models.ManageViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace BurpsRSuite.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILoggerFactory logger;
        private readonly ApplicationDbContext context;

        public ManageController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory logger,
            ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.context = context;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public override async void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ApplicationUser user = await userManager.GetUserAsync(User);
            if(user == null || !user.HasSetupChallengeQuestions())
            {
                filterContext.Result = new RedirectToActionResult("SetUpChallengeQuestions", "Account", null);
                return;
            }

        }

        public async Task<ActionResult> Index()
        {

            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                throw new ApplicationException($"What the '{userManager.GetUserId(User)}'.");
            }

            IndexViewModel model = new IndexViewModel
            {
                UserName = user.UserName,
                StatusMessage = StatusMessage
            };

            return View(model);
        }

        
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if(!ModelState.IsValid)
            {
                StatusMessage = "Error?";
                return RedirectToAction(nameof(Index));
            }

            var user = await userManager.GetUserAsync(User);

            if(user != null)
            {
                var result = await userManager.ChangePasswordAsync(
                    user,
                    model.Password.Trim(),
                    model.NewPassword.Trim()
                );
                
                if(result.Succeeded)
                {
                    StatusMessage = "Password successfully changed";
                    return RedirectToAction(nameof(Index));
                }
            }
            StatusMessage = "Error occurred during password change";
            return RedirectToAction(nameof(Index));
        }

        
    }
}