
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using PortfolioBuilder.Models;
using System.Threading.Tasks;
using PortfolioBuilder.ViewModels;
using System.Text.Encodings.Web;

namespace PortfolioBuilder.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = new ApplicationUser { UserName = model.Username, Email = model.Email, FullName = model.FullName };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // generate email confirmation token and send (app writes to Emails folder)
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
                await _emailSender.SendEmailAsync(user.Email, "Confirm your email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(confirmUrl)}'>clicking here</a>.");
                return RedirectToAction("Index", "Home");
            }
            foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
            return View(model);
        }

        public IActionResult Login(string returnUrl = null) { ViewData["ReturnUrl"] = returnUrl; return View(); }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && !await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Email not confirmed. Check the Emails folder for the confirmation link.");
                return View(model);
            }
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl)) return Redirect(returnUrl);
                return RedirectToAction("Index", "Dashboard");
            }
            ModelState.AddModelError("", "Invalid login attempt");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null) return RedirectToAction("Index", "Home");
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded) return View("ConfirmEmail");
            return View("Error"); 
        }

        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email)) return View();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return RedirectToAction("ForgotPasswordConfirmation");
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, token = token }, Request.Scheme);
            await _emailSender.SendEmailAsync(user.Email, "Reset Password", $"Reset your password by <a href='{HtmlEncoder.Default.Encode(resetUrl)}'>clicking here</a>.");
            return RedirectToAction("ForgotPasswordConfirmation");
        }

        public IActionResult ForgotPasswordConfirmation() => View();

        public IActionResult ResetPassword(string userId, string token) { return View(new ResetPasswordViewModel{ UserId = userId, Token = token }); }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return RedirectToAction("ResetPasswordConfirmation");
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded) return RedirectToAction("ResetPasswordConfirmation");
            foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
            return View(model);
        }

        public IActionResult ResetPasswordConfirmation() => View();
    }
}
