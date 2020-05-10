using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurpsRSuite.Data;
using BurpsRSuite.Models;
using BurpsRSuite.Models.AccountViewModels;
using BurpsRSuite.Models.ManageViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using OtpNet;

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
                ApplicationUser user = await userManager.FindByEmailAsync(model.Email);
                

                if (user != null)
                {
                    if (user.VerifyAccountNumber(model.AccountNumber))
                    {
                        HttpContext.Session.SetString("Email", model.Email);
                        return RedirectToAction("ResetPassword");
                    }
                    ModelState.AddModelError(string.Empty, "Authentication failed.");
                }
            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index), "Home");
            }

            ApplicationUser user = await userManager.FindByEmailAsync(
                HttpContext.Session.GetString("Email"));

            if(user != null)
            {
                await userManager.RemovePasswordAsync(user);
                await userManager.AddPasswordAsync(user, model.NewPassword);
                await userManager.UpdateAsync(user);
                return RedirectToAction(nameof(Index), "Manage");
            }


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            ApplicationUser user = await userManager.GetUserAsync(User);
            user.CQVerified = false;
            user.TFVerified = false;
            await userManager.UpdateAsync(user);
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "home");
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


                    return RedirectToAction(nameof(SetupChallengeQuestions));
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
                    else
                    {
                        return RedirectToAction(nameof(AnswerChallengeQuestions));
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
                return RedirectToAction(nameof(Index), "Home");
            }

            ApplicationUser user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                ChallengeQuestionsViewModel model = new ChallengeQuestionsViewModel
                {
                    ChallengeQuestion1 = "What was your childhood nickname?",
                    ChallengeQuestion2 = "In what city or town did your mother and father meet?",
                };

                return View(model);
            }
            return RedirectToAction(nameof(Index), "Home");
        }

        [HttpPost]
        public async Task<IActionResult> SetupChallengeQuestions(ChallengeQuestionsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index), "Home");
            }

            ApplicationUser user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                user.Question1 = model.ChallengeQuestion1;
                user.Question2 = model.ChallengeQuestion2;

                user.Answer1 = model.Answer1.Trim();
                user.Answer2 = model.Answer2.Trim();

                user.CQVerified = true;
                await userManager.UpdateAsync(user);

                return RedirectToAction(nameof(Index), "Manage");
            }
            return RedirectToAction(nameof(Index), "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AnswerChallengeQuestions()
        {
            ApplicationUser user = await userManager.GetUserAsync(User);

            if(!user.HasSetupChallengeQuestions())
            {
                return RedirectToAction(nameof(Index), "Home");
            }


            ChallengeQuestionsViewModel model = new ChallengeQuestionsViewModel
            {
                ChallengeQuestion1 = "What was your childhood nickname?",
                ChallengeQuestion2 = "In what city or town did your mother and father meet?",
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AnswerChallengeQuestions(ChallengeQuestionsViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index), "Home");
            }

            ApplicationUser user = await userManager.GetUserAsync(User);

            if(user != null)
            {
                if(user.VerifyChallengeAnswers(model.Answer1, model.Answer2))
                {
                    user.CQVerified = true;
                    await userManager.UpdateAsync(user);
                    return RedirectToAction(nameof(Index), "Manage");
                }
            }
            return RedirectToAction(nameof(Index), "Home");
        }

        [HttpGet]
        [Authorize]
        public IActionResult TwoFactor()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> TwoFactor(TwoFactorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index), "Home");
            }

            ApplicationUser user = await userManager.GetUserAsync(User);

            if (user == null || !user.TotpEnabled)
            {
                return RedirectToAction(nameof(Index), "Home");
            }

            Totp totp = new Totp(user.TotpSecret);

            if (!totp.VerifyTotp(model.Passcode, out long window, VerificationWindow.RfcSpecifiedNetworkDelay))
            {
                ModelState.AddModelError(string.Empty, "Verification Failed.");
                model.Passcode = "";
                return View(model);
            }

            user.TFVerified = true;
            await userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index), "Manage");
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}