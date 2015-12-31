using IdentitySample_2.Models;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using IdentitySample_2.App_Start;

namespace IdentitySample_2.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signManager;

        ApplicationDbContext dbContext;

        //constructor
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            set
            {
                _signManager = value;
            }
        }



        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }







        // GET: Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            dbContext = new ApplicationDbContext();

            if (ModelState.IsValid)
            {
                //var userStore = new UserStore<ApplicationUser>(dbContext);
                //var userManager = new UserManager<ApplicationUser>(userStore);


                //var signInManager = new SignInManager<ApplicationUser, string>(userManager, AuthenticationManager);
                //var user = new ApplicationUser { UserName = model.UserName };


                //sign in  the user.
                //await signInManager.SignInAsync(user, false, false);
                var signInStatus = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, false, shouldLockout: false);

                switch (signInStatus)
                {
                    case SignInStatus.Success:
                        return RedirectToLocal(returnUrl);

                    case SignInStatus.LockedOut:
                        break;
                    case SignInStatus.RequiresVerification:
                        break;
                    case SignInStatus.Failure:
                        break;
                    default:
                        break;
                }
            }
            return View();
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Home", "Account");
        }

        public ActionResult SignOut()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }


        // GET: Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
                dbContext = new ApplicationDbContext();

                //var userStore = new UserStore<ApplicationUser>(dbContext);
                //var userManager = new UserManager<ApplicationUser>(userStore);
                //var signInManager = new SignInManager<ApplicationUser, string>(userManager, AuthenticationManager);

                //find user exixts in db
                var user = await UserManager.FindAsync(register.UserName,register.Password);
                //var user = await userManager.FindAsync(register.UserName, register.Password);
                if (user != null)
                {
                    return View();
                }

                //if not create the user
                user = new ApplicationUser { UserName = register.UserName, Email = register.EamilID };

                var result = await UserManager.CreateAsync(user, register.Password);
                if (result.Succeeded)
                {
                    //sign in  the user.
                    //await signInManager.SignInAsync(user, false, false);
                    var signInStatus = await SignInManager.PasswordSignInAsync(register.UserName, register.Password, false, true);
                    return RedirectToAction("Home");
                }

                return View();
            }
            else
            {
                //return with errors
                return View();
            }
        }
        [Authorize]
        public ActionResult Home()
        {
            return View();
        }

    }
}