using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BurpsRSuite.Data;
using BurpsRSuite.Models;
using BurpsRSuite.Models.ManageViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OtpNet;
using Microsoft.AspNetCore.Mvc.Filters;
using QRCoder;

namespace BurpsRSuite.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILoggerFactory logger;
        private readonly ApplicationDbContext context;
        private readonly UrlEncoder urlEncoder; 
        
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        private const string RecoveryCodesKey = nameof(RecoveryCodesKey);

        public ManageController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory logger,
            ApplicationDbContext context,
            UrlEncoder urlEncoder)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.context = context;
            this.urlEncoder = urlEncoder;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public override async void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ApplicationUser user = await userManager.GetUserAsync(User);

            if(user != null && !user.HasSetupChallengeQuestions())
            {
                filterContext.Result = new RedirectToActionResult("SetUpChallengeQuestions", "Account", null);
                return;
            }

            if(user != null && !user.CQVerified)
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
                AccountNumber = user.AccountNumber,
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

        [HttpGet]
        public async Task<IActionResult> CreateTF()
        {
            ApplicationUser user = await userManager.GetUserAsync(User);

            if (user != null && !user.TotpEnabled)
            {
                byte[] keyBytes;
                if(user.TotpSecret != null)
                {
                    keyBytes = user.TotpSecret;
                }
                else
                {
                    keyBytes = KeyGeneration.GenerateRandomKey(20);
                }


                string base32Key = Base32Encoding.ToString(keyBytes);
                byte[] base32KeyBytes = Base32Encoding.ToBytes(base32Key);
                Totp totp = new Totp(base32KeyBytes);
                user.TotpSecret = keyBytes;
                await userManager.UpdateAsync(user);
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode($"otpauth://totp/BurpsRSuite:{user.UserName}?secret={base32Key}&issuer=Burps+R+Suite", QRCodeGenerator.ECCLevel.Q);
                Base64QRCode qrCode = new Base64QRCode(qrCodeData);
                string qrCodeBase64 = qrCode.GetGraphic(10);
                CreateTFViewModel model = new CreateTFViewModel
                {
                    QRCodeBase64 = qrCodeBase64
                };
                return View(model);
            }

            StatusMessage = "Error while looking up User";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTF(CreateTFViewModel model)
        {
            ApplicationUser user = await userManager.GetUserAsync(User);

            if (!ModelState.IsValid || user.TotpEnabled)
            {
                return RedirectToAction(nameof(Index));
            }
            
            if (!user.TotpEnabled)
            {
                string passcode = model.Passcode;
                Totp totp = new Totp(user.TotpSecret);
                if (totp.VerifyTotp(passcode, out long window, VerificationWindow.RfcSpecifiedNetworkDelay))
                {
                    user.TotpEnabled = true;
                    user.TFVerified = true;
                    await userManager.UpdateAsync(user);
                    StatusMessage = "One-time Token Set Up!";
                    return RedirectToAction(nameof(Index));
                    
                }
            }

            StatusMessage = "Invaid Code";
            return RedirectToAction(nameof(Index));
        }
    }
}