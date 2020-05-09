using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurpsRSuite.Data;
using BurpsRSuite.Models;
using BurpsRSuite.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BurpsRSuite.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILoggerFactory logger;
        private readonly ApplicationDbContext context;

        public AccountController(
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

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    if (user.VerifyAccountNumber(model.AccountNumber))
                    {
                        return RedirectToAction("ResetPassword");
                    }
                    ModelState.AddModelError(string.Empty, "Authentication failed.");
                }
            }

            return View();
        }







        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Random random = new Random();
                var user = new ApplicationUser { 
                    UserName = model.UserName, 
                    Email = model.Email, 
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    AccountNumber = random.Next(0, 999999999).ToString("000000000")
            };

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("index", "home");
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }


            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(
                    model.UserName, 
                    model.Password, 
                    model.RememberMe, 
                    false);

                if (result.Succeeded)
                {
                    var user = userManager.Users.FirstOrDefault(u => u.UserName == model.UserName);

                    if(!user.HasSetupChallengeQuestions())
                    {
                        return RedirectToAction(nameof(SetupChallengeQuestions));
                    }

                    if(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Manage");
                    }
                }

                ModelState.AddModelError(string.Empty, "Invaid Login Attempt");
            }


            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SetupChallengeQuestions()
        {
            if (!ModelState.IsValid)
            {
                return Redirect(nameof(Index));
            }

            ApplicationUser user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                SetupChallengeQuestionsViewModel model = new SetupChallengeQuestionsViewModel
                {
                    ChallengeQuestion1 = "What was your childhood nickname?",
                    ChallengeQuestion2 = "In what city or town did your mother and father meet?",
                };

                return View(model);
            }
            return Redirect(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SetupChallengeQuestions(SetupChallengeQuestionsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Redirect(nameof(Index));
            }

            ApplicationUser user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                //set questions in database
                user.Question1 = new ChallengeQuestion
                {
                    Id = 1,
                    Question = model.ChallengeQuestion1
                };
                user.Question1 = new ChallengeQuestion
                {
                    Id = 2,
                    Question = model.ChallengeQuestion2
                };
                user.Answer1 = model.Answer1.Trim();
                user.Answer2 = model.Answer2.Trim();

                await userManager.UpdateAsync(user);
                return RedirectToAction(nameof(Index), "Manage");
            }
            return Redirect(nameof(Index));
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}