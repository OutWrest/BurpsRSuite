using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurpsRSuite.Data;
using BurpsRSuite.Models;
using BurpsRSuite.Models.AdminViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;

namespace BurpsRSuite.Controllers
{
    [Authorize(Roles = AuthorizationRoles.Administrator)]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILoggerFactory logger;
        private readonly ApplicationDbContext context;
        private readonly RoleManager<IdentityRole> roleManager;
        private static Random random = new Random();
        private const short CAPTCHALENGTH = 8;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory logger,
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.context = context;
            this.roleManager = roleManager;
        }
        
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public override async void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ApplicationUser user = await userManager.GetUserAsync(User);

            if (user != null && !user.HasSetupChallengeQuestions())
            {
                filterContext.Result = new RedirectToActionResult("SetUpChallengeQuestions", "Account", null);
                return;
            }

            if (user != null && !user.CQVerified)
            {
                filterContext.Result = new RedirectToActionResult("AnswerChallengeQuestions", "Account", null);
                return;
            }

            if (user != null && user.TotpEnabled && !user.TFVerified)
            {
                filterContext.Result = new RedirectToActionResult("TwoFactor", "Account", null);
                return;
            }

        }

        [TempData]
        public string StatusMessage { get; set; }
        [TempData]
        public string Captcha { get; set; }

        public IActionResult Index()
        {
            List<ApplicationUser> userslist = userManager.Users.ToList<ApplicationUser>();


            IndexViewModel model = new IndexViewModel 
            { 
                users = userslist,
                StatusMessage = StatusMessage
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditAsync(string Id)
        {
            if(string.IsNullOrEmpty(Id))
            {
                return RedirectToAction(nameof(Index));
            }
            
            ApplicationUser user = await userManager.FindByIdAsync(Id);

            if(user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            UserViewModel model = new UserViewModel
            {
                user = user,
                StatusMessage = StatusMessage
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            ApplicationUser user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            user.UserName = model.UserName;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.AccountNumber = model.AccountNumber;

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                StatusMessage = "Changes Saved Successfully";
                return RedirectToAction(nameof(Index));
            }

            StatusMessage = "Error While Changing Information";

            UserViewModel vm = new UserViewModel
            {
                user = user,
                StatusMessage = StatusMessage
            };


            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                StatusMessage = "Error occured while resetting the password";

                return RedirectToAction(nameof(Index));
            }

            ApplicationUser user = await userManager.FindByIdAsync(Id);

            if (user == null)
            {
                StatusMessage = "Error occured while resetting the password";

                return RedirectToAction(nameof(Index));
            }

            Captcha = RandomString(CAPTCHALENGTH);

            UserViewModel model = new UserViewModel
            {
                user = user,
                Captcha = Captcha,
                StatusMessage = StatusMessage
            };

            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> ResetPasswordAsync(UserViewModel model)
        {
            ApplicationUser user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return RedirectToAction(nameof(Index), "Admin");
            }

            if (string.IsNullOrEmpty(model.CaptchaIn) || !Captcha.Equals(model.CaptchaIn.Trim()))
            {
                StatusMessage = "Error captcha is wrong";

                return RedirectToAction(nameof(ResetPassword));
            }

            await userManager.RemovePasswordAsync(user);
            await userManager.AddPasswordAsync(user, user.AccountNumber.ToString());

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                StatusMessage = $"{user.UserName}'s password was reset successfully";
                return RedirectToAction(nameof(Index));
            }

            StatusMessage = $"Error while changing {user.UserName}'s password";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ResetQuestions(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                StatusMessage = "Error occured while resetting the password";

                return RedirectToAction(nameof(Index));
            }

            ApplicationUser user = await userManager.FindByIdAsync(Id);

            if (user == null)
            {
                StatusMessage = "Error occured while resetting the password";

                return RedirectToAction(nameof(Index));
            }

            Captcha = RandomString(CAPTCHALENGTH);

            UserViewModel model = new UserViewModel
            {
                user = user,
                Captcha = Captcha,
                StatusMessage = StatusMessage
            };

            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> ResetQuestionsAsync(UserViewModel model)
        {
            ApplicationUser user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return RedirectToAction(nameof(Index), "Admin");
            }

            if (string.IsNullOrEmpty(model.CaptchaIn) || !Captcha.Equals(model.CaptchaIn.Trim()))
            {
                StatusMessage = "Error captcha is wrong";

                return RedirectToAction(nameof(ResetPassword));
            }

            user.Answer1 = null;
            user.Answer2 = null;
            user.CQVerified = false;

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                StatusMessage = $"{user.UserName}'s challenge questions were reset successfully";
                return RedirectToAction(nameof(Index));
            }

            StatusMessage = $"Error while resetting {user.UserName}'s questions";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Reset2Factor(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                StatusMessage = "Error occured while resetting the password";

                return RedirectToAction(nameof(Index));
            }

            ApplicationUser user = await userManager.FindByIdAsync(Id);

            if (user == null)
            {
                StatusMessage = "Error occured while resetting the password";

                return RedirectToAction(nameof(Index));
            }

            Captcha = RandomString(CAPTCHALENGTH);

            UserViewModel model = new UserViewModel
            {
                user = user,
                Captcha = Captcha,
                StatusMessage = StatusMessage
            };

            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> Reset2FactorAsync(UserViewModel model)
        {
            ApplicationUser user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return RedirectToAction(nameof(Index), "Admin");
            }

            if (string.IsNullOrEmpty(model.CaptchaIn) || !Captcha.Equals(model.CaptchaIn.Trim()))
            {
                StatusMessage = "Error captcha is wrong";

                return RedirectToAction(nameof(ResetPassword));
            }

            user.TotpEnabled = false;
            user.TotpSecret = null;
            user.TFVerified = false;

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                StatusMessage = $"{user.UserName}'s two factor was reset successfully";
                return RedirectToAction(nameof(Index));
            }

            StatusMessage = $"Error while resetting {user.UserName}'s two factor";

            return RedirectToAction(nameof(Index));
        }
    }
}