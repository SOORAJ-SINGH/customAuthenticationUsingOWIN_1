using IdentitySample_2.Models;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;


namespace IdentitySample_2.Controllers
{
    public class AccountController : Controller
    {

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        ApplicationDbContext dbContext = new ApplicationDbContext();

        // GET: Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model,string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var userStore = new UserStore<ApplicationUser>( dbContext );
                var userManager = new UserManager<ApplicationUser>(userStore);

                var signInManager = new SignInManager<ApplicationUser, string>(userManager, AuthenticationManager);
                var user = new IdentityUser { UserName = model.UserName };

                //sign in  the user.
                //await signInManager.SignInAsync(user, false, false);
                var signInStatus = await signInManager.PasswordSignInAsync(model.UserName, model.Password, false, true);

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
                var userStore = new UserStore<ApplicationUser>(dbContext);
                var userManager = new UserManager<ApplicationUser>(userStore);

                var signInManager = new SignInManager<ApplicationUser, string>(userManager, AuthenticationManager);

                //find user exixts in db
                var user = await userManager.FindAsync(register.UserName, register.Password);
                if (user != null)
                {
                    return View();
                }

                //if not create the user
                user = new ApplicationUser { UserName = register.UserName, Email = register.EamilID };

                var result = await userManager.CreateAsync(user, register.Password);
                if (result.Succeeded)
                {
                    //sign in  the user.
                    //await signInManager.SignInAsync(user, false, false);
                    var signInStatus = await signInManager.PasswordSignInAsync(register.UserName, register.Password, false, true);

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