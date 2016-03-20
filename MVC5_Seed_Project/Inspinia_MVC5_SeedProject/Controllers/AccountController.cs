using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using Inspinia_MVC5_SeedProject.Models;
using Inspinia_MVC5_SeedProject.CodeTemplates;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin;
namespace Inspinia_MVC5_SeedProject.Controllers
{
  //   public class ApplicationUserManager : UserManager<ApplicationUser>
  //{
  //        public ApplicationUserManager(): base(new UserStore<ApplicationUser>(new ApplicationDbContext()))
  //        {
  //              PasswordValidator = new MinimumLengthValidator (0);
  //        }
  //}
    //public class MyUserManager : UserManager<ApplicationUser>
    //{
    //    public MyUserManager() :
    //        base(new UserStore<ApplicationUser>(new ApplicationDbContext()))
    //    {
    //        PasswordValidator = new MinimumLengthValidator(0);
    //    }
    //}
    //following code is for email verification
    public class AppUserManager : UserManager<ApplicationUser>
{
public AppUserManager(IUserStore<ApplicationUser> store) : base(store) { }

    public static AppUserManager Create(IdentityFactoryOptions<AppUserManager> options, IOwinContext context)
    {
        ApplicationDbContext db = context.Get<ApplicationDbContext>();
        AppUserManager manager = new AppUserManager(new UserStore<ApplicationUser>(db));

        //manager.PasswordValidator = new PasswordValidator { 
        //    RequiredLength = 6,
        //    RequireNonLetterOrDigit = false,
        //    RequireDigit = false,
        //    RequireLowercase = true,
        //    RequireUppercase = true
        //};

        //manager.UserValidator = new UserValidator<ApplicationUser>(manager)
        //{
        //    AllowOnlyAlphanumericUserNames = true,
        //    RequireUniqueEmail = true
        //};

        var dataProtectionProvider = options.DataProtectionProvider;

        //token life span is 3 hours
        if (dataProtectionProvider != null)
        {
            manager.UserTokenProvider =
               new DataProtectorTokenProvider<ApplicationUser>
                  (dataProtectionProvider.Create("ConfirmationToken"))
               {
                   TokenLifespan = TimeSpan.FromHours(3)
               };
        }

        //defining email service
       // manager.EmailService = new EmailService();

        return manager;
    } //Create

  } //class


    [Authorize]
    public class AccountController : Controller
    {
        private Entities db = new Entities();

        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
           
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }
        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            var provider = new DpapiDataProtectionProvider("http://newtemp.apphb.com/");
            UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser, string>(provider.Create("UserToken"))
                as IUserTokenProvider<ApplicationUser, string>;

            IdentityResult result;
            try
            {
                result = await UserManager.ConfirmEmailAsync(userId, code);
            }
            catch (InvalidOperationException ioe)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }

            if (result.Succeeded)
            {
                return Json("Done", JsonRequestBehavior.AllowGet);
            }
            return Json("Error", JsonRequestBehavior.AllowGet);
            
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> UserLogin(string email, string password)
        {
            var data = db.AspNetUsers.First(x => x.UserName.Equals(email));
            bool isSaved = true;
            try {
                 isSaved = (bool)data.IsPasswordSaved;
            }catch(Exception e)
            {
                isSaved = false;
            }
            if (!isSaved)
            {
                UserManager.PasswordValidator = new PasswordValidator
                {
                    RequiredLength = -1
                };
                IdentityResult result = await UserManager.ChangePasswordAsync(data.Id, "aa", password);
                if (!result.Succeeded)
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }
                data.IsPasswordSaved = true;
               await db.SaveChangesAsync();
            }
            var user = await UserManager.FindAsync(email, password);

            try
            {
                var provider = new DpapiDataProtectionProvider("http://newtemp.apphb.com/");
                UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser, string>(provider.Create("UserToken"))
                    as IUserTokenProvider<ApplicationUser, string>;

                //var provider = new DpapiDataProtectionProvider("Sample");

                //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>());

                //userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
                //    provider.Create("EmailConfirmation"));

                var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = Url.Action(
                   "ConfirmEmail", "Account",
                   new { userId = user.Id, code = code },
                   protocol: Request.Url.Scheme);

                ElectronicsController.sendEmail(user.UserName, "Welcome to dealkar.pk - Confirm Email address", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
           //    await ConfirmEmail(user.Id, code);
            }
            catch (Exception e)
            {
                string s = e.ToString();
            }

            if (user != null)
            {
                await SignInAsync(user, true);
                return Json("Done",JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }
        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Email or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        public void MakeAdmin()
        {
            
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> RegisterUser(string email, string password = "aa")
        {

            //var roleManager = new RoleManager<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));


            //if (!roleManager.RoleExists("Admin"))
            //{
            //    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
            //    role.Name = "Admin";
            //    roleManager.Create(role);

            //}


            var ab = email.Split('@');

            var user = new ApplicationUser() { UserName = email };
            user.Email = ab[0];
            UserManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = -1
            };
            var result = await UserManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                //var currentUser = UserManager.FindByName(user.UserName);

                //var roleresult = UserManager.AddToRole(currentUser.Id, "Admin");

                try { 
                await SignInAsync(user, isPersistent: true);
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                }
                var id = user.Id;
                var data =await db.AspNetUsers.FindAsync(id);
                if (password == "aa")
                {
                    data.IsPasswordSaved = false;
                }
                else
                {
                    data.IsPasswordSaved = true;
                }
                data.hideEmail = true;
                data.hidePhoneNumber = true;
                db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
                
                
                return Json("Done", JsonRequestBehavior.AllowGet);
            }
            return Json("Error", JsonRequestBehavior.AllowGet);
        }
        
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<JsonResult> RegisterOnEmail(string email)
        //{
        //    var user = new ApplicationUser() { UserName = email };
        //    var result = await UserManager.CreateAsync(user, password);
        //    if (result.Succeeded)
        //    {
        //        await SignInAsync(user, isPersistent: true);
        //        return Json("Done", JsonRequestBehavior.AllowGet);
        //    }

        //    return Json("Error", JsonRequestBehavior.AllowGet);
        //}
        [HttpPost]
        public async Task<JsonResult> SubmitName(string name)
        {
            if (Request.IsAuthenticated)
            {
                string userId = User.Identity.GetUserId();
                var data =await db.AspNetUsers.FindAsync(userId);
                if (data != null)
                {
                    name = name.Trim();
                    data.Email = name;
                    await db.SaveChangesAsync();
                    return Json("Done", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("Error", JsonRequestBehavior.AllowGet);
        }
        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
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
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            var path = Request["path"];
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
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
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}