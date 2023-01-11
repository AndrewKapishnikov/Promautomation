using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using Store.DAL.Interfaces;
using System.Threading.Tasks;
using AsuBlog.Models;
using System.Security.Claims;
using Store.DAL.Entities;
using Microsoft.AspNet.Identity;
using System.Net;
using Newtonsoft.Json.Linq;
using Store.DAL.Identity;
using System.Linq;

namespace AsuBlog.Controllers
{   
    
    public class AccountController : Controller
    {
        private ApplicationSignInManager signInManager;
        private IUnitOfWork UserService
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<IUnitOfWork>();
            }
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication; 
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return signInManager ?? new ApplicationSignInManager(UserService.UserManager,AuthenticationManager);
            }
            private set
            {
                signInManager = value;
            }
        }


        /// <summary>
        /// Called only once when the database is first initialized
        /// </summary>
        /// <returns></returns>
        public async Task SetInitialDataAsync()
        {
            List<string> roles = new List<string> { "user", "admin" };
            foreach (string roleName in roles)
            {
                var role = await UserService.RoleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    role = new ApplicationRole { Name = roleName };
                    await UserService.RoleManager.CreateAsync(role);
                }
            }
            
            ApplicationUser user = new ApplicationUser { UserName = "Andrew", Email = "kapishnikovaa@yandex.ru"};
            await UserService.UserManager.CreateAsync(user, "e*********");
            await UserService.UserManager.AddToRoleAsync(user.Id, "admin");
            await UserService.SaveAsync();
        }
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {

                // var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, false, shouldLockout: false);
                ClaimsIdentity claim = null;
                ApplicationUser user = await UserService.UserManager.FindAsync(model.UserName, model.Password);

                if (user != null)
                {
                    if (user.EmailConfirmed == true)
                    {
                        claim = await UserService.UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                        AuthenticationManager.SignOut();
                        AuthenticationManager.SignIn(new AuthenticationProperties
                        {
                            IsPersistent = true
                        }, claim);
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Не подтвержден email. Зайдите на ваш email для подтверждения.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неверный логин или пароль");
                }

             
            }

            return View(model);
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Blog");
        }

        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Blog");
        }

        public ActionResult Register()
        {
            return View();
        }
             
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            var response = Request["g-recaptcha-response"];
            string secretKey = "*******";
            var client = new WebClient();
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
            var obj = JObject.Parse(result);
            var status = (bool)obj.SelectToken("success");
 
            // await SetInitialDataAsync();
            if (ModelState.IsValid && status)
            {
                ApplicationUser userName = await UserService.UserManager.FindByNameAsync(model.UserName);
                if (userName != null)
                    ModelState.AddModelError("UserName", "Пользователь с таким именем уже существует!");
                ApplicationUser userEmail = await UserService.UserManager.FindByEmailAsync(model.Email);
                if (userEmail != null)
                    ModelState.AddModelError("Email", "Пользователь с такой электронной почтой уже существует!");
             
                if (userName == null && userEmail == null)
                {
                    ApplicationUser user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                    user.EmailConfirmed = true;
                    var resultUser = await UserService.UserManager.CreateAsync(user, model.Password);
                    // add Role
                    await UserService.UserManager.AddToRoleAsync(user.Id, "user");
                    await UserService.SaveAsync();

                    if (resultUser.Succeeded)
                    {
                        ClaimsIdentity claim = await UserService.UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                        AuthenticationManager.SignOut();
                        AuthenticationManager.SignIn(new AuthenticationProperties
                        {
                            IsPersistent = true
                        }, claim);
                        return View("RegisterSuccess");
                    }

                    //if (resultUser.Succeeded)
                    //{
                    //    // generate a token to confirm registration
                    //    var code = await UserService.UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //    //create a link for confirmation
                    //    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code },
                    //               protocol: Request.Url.Scheme);
                    //    // sending letter
                    //    //await UserService.UserManager.SendEmailAsync(user.Id, "Подтверждение электронной почты",
                    //    //           "Для завершения регистрации перейдите по ссылке:: <a href=\""
                    //    //                                           + callbackUrl + "\">завершить регистрацию</a>");
                    //    return View("RegisterConfirm");
                    //}

                    ModelState.AddModelError("Create", "Пользователя зарегистрировать не удалось!");
            
                }
              
            }
           return View(model);
        }

      
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            
            if (userId != null && code != null)
            {
                ApplicationUser user = UserService.UserManager.FindById(userId);
                if(user != null)
                {
                    var result = await UserService.UserManager.ConfirmEmailAsync(userId, code);
                    if (result.Succeeded)
                    {
                        user.EmailConfirmed = true;
                        await UserService.UserManager.UpdateAsync(user);

                        ClaimsIdentity claim = await UserService.UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

                        if (claim != null)
                        {
                            AuthenticationManager.SignOut();
                            AuthenticationManager.SignIn(new AuthenticationProperties
                            {
                                IsPersistent = true
                            }, claim);

                        }

                        return View("RegisterSuccess");
                    }
                   
                }
                
            }
          
            return View("RegisterFailed");
            
        }


        // POST: /Account/ExternalLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }


        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }
            
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                var result = await UserService.UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserService.UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserService.UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        protected override void Dispose(bool disposing)
        {
            UserService.Dispose();
            base.Dispose(disposing);
        }

    }
}