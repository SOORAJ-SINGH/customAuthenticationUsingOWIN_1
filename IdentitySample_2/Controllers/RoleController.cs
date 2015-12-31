using IdentitySample_2.App_Start;
using IdentitySample_2.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IdentitySample_2.Controllers
{
    //[Authorize(Roles = "ADMIN")]
    public class RoleController : Controller
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();
        List<string> userRolesList;
        List<string> rolesList;
        List<string> usersList;

        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public RoleController()
        { }
        public RoleController(ApplicationUserManager userManager,ApplicationRoleManager roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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
        
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            set
            {
                _roleManager = value;
            }
        }

        // GET: Role
        public ActionResult Index()
        {
            //View
            var roles = dbContext.Roles.ToList();
            return View(roles);
        }

        public ActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Create(string RoleName)
        {
            try
            {
                dbContext.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole() { Name = RoleName } );
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View();
            }
        }


        public ActionResult Edit(string RoleName)
        {
            var role = dbContext.Roles.Where(r => r.Name == RoleName).FirstOrDefault();
            if (role != null)
            {
                return View(role);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Edit(string Id, String Name)
        {
            var role = dbContext.Roles.Where(r => r.Id == Id).FirstOrDefault();
            if (role != null)
            {
                role.Name = Name;
                dbContext.Entry(role).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
              
            }
            return RedirectToAction("Index");
           
        }


        //manages the user's Role
        public ActionResult ManageUserRoles()
        {
            // prepopulate roles for the view dropdown
            var list = dbContext.Roles.OrderBy(r => r.Name).ToList().Select(rr =>

new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;
            return View();
        }



        public ActionResult RoleAddToUser()
        {

            using (var context = new ApplicationDbContext())
            {
                //var roleStore = new RoleStore<IdentityRole>(context);
                //var roleManager = new RoleManager<IdentityRole>(roleStore);

                //var userStore = new UserStore<ApplicationUser>(context);
                //var userManager = new UserManager<ApplicationUser>(userStore);

                usersList = (from u in UserManager.Users select u.UserName).ToList();
                rolesList = (from r in  RoleManager.Roles select r.Name).ToList();
            }

            ViewBag.Roles = new SelectList(rolesList);
            ViewBag.Users = new SelectList(usersList);
            return View();
        }

        [HttpPost]
        public ActionResult RoleAddToUser(string userName,string roleName)
        {


            using (var context = new ApplicationDbContext())
            {
                //var roleStore = new RoleStore<IdentityRole>(context);
                //var roleManager = new RoleManager<IdentityRole>(roleStore);


                //var userStore = new UserStore<ApplicationUser>(context);
                //var userManager = new UserManager<ApplicationUser>(userStore);

                usersList = (from u in UserManager.Users select u.UserName).ToList();

                var user = UserManager.FindByName(userName);
                if (user == null)
                    throw new Exception("User not found!");

                var role = RoleManager.FindByName(roleName);// roleManager.FindByName(roleName);
                if (role == null)
                    throw new Exception("Role not found!");

                if (UserManager.IsInRole(user.Id, role.Name))
                {
                    ViewBag.ResultMessage = "This user already has the role specified !";
                }
                else
                {
                    
                    UserManager.AddToRole(user.Id, role.Name);
                    context.SaveChanges();

                    ViewBag.ResultMessage = "Username added to the role succesfully !";
                }

                rolesList = (from r in RoleManager.Roles select r.Name).ToList();
            }

            ViewBag.Roles = new SelectList(rolesList);
            ViewBag.Users = new SelectList(usersList);
            return View();
   

        }



        //get the roles of user 
        
        [ValidateAntiForgeryToken]
        public ActionResult GetRoles(string userName)
        {
            if (!string.IsNullOrWhiteSpace(userName))
            {

                using (var context = new ApplicationDbContext())
                {
                    //var roleStore = new RoleStore<IdentityRole>(context);
                    //var roleManager = new RoleManager<IdentityRole>(roleStore);

                    rolesList = (from r in RoleManager.Roles select r.Name).ToList();

                    //var userStore = new UserStore<ApplicationUser>(context);
                    //var userManager = new UserManager<ApplicationUser>(userStore);

                    usersList = (from u in UserManager.Users select u.UserName).ToList();

                    var user = UserManager.FindByName(userName);
                    if (user == null)
                        throw new Exception("User not found!");

                    var userRoleIds = (from r in user.Roles select r.RoleId);
                    userRolesList = (from id in userRoleIds
                                 let r = RoleManager.FindById(id)
                                 select r.Name).ToList();
                }

                ViewBag.Roles = new SelectList(rolesList);
                ViewBag.Users = new SelectList(usersList);
                ViewBag.RolesForThisUser = userRolesList;
            }

            return View("RoleAddToUser");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleForUser(string userName, string roleName)
        {
            List<string> userRoles;
            List<string> roles;
            List<string> users;
            using (var context = new ApplicationDbContext())
            {
                //var roleStore = new RoleStore<IdentityRole>(context);
                //var roleManager = new RoleManager<IdentityRole>(roleStore);

                roles = (from r in RoleManager.Roles select r.Name).ToList();

                //var userStore = new UserStore<ApplicationUser>(context);
                //var userManager = new UserManager<ApplicationUser>(userStore);

                users = (from u in UserManager.Users select u.UserName).ToList();

                var user = UserManager.FindByName(userName);
                if (user == null)
                    throw new Exception("User not found!");

                if (UserManager.IsInRole(user.Id, roleName))
                {
                    UserManager.RemoveFromRole(user.Id, roleName);
                    context.SaveChanges();

                    ViewBag.ResultMessage = "Role removed from this user successfully !";
                }
                else
                {
                    ViewBag.ResultMessage = "This user doesn't belong to selected role.";
                }

                var userRoleIds = (from r in user.Roles select r.RoleId);
                userRoles = (from id in userRoleIds
                             let r = RoleManager.FindById(id)
                             select r.Name).ToList();
            }

            ViewBag.RolesForThisUser = userRoles;
            ViewBag.Roles = new SelectList(roles);
            ViewBag.Users = new SelectList(users);
            return View("RoleAddToUser");
        }

    }
}