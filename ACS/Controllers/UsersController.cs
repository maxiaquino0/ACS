using ACS.Models;
using ACS.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACS.Controllers
{
    [Authorize(Roles ="Administrator")]
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Users
        public ActionResult Index()
        {
            var users = GetUsers();
            var usersView = new List<UserView>();

            foreach (var user in users)
            {
                var userView = new UserView
                {
                    UserID = user.Id,
                    Name = user.UserName,
                    Email = user.Email
                };
                usersView.Add(userView);
            }
            return View(usersView);
        }

        // GET: Roles
        public ActionResult Roles(string userID)
        {
            var users = GetUsers();
            var user = users.ToList().Find(u => u.Id == userID);
            if(user == null)
            {
                return HttpNotFound();
            }

            var roles = GetRoles();
            var rolesView = new List<RoleView>();

            foreach (var item in user.Roles)
            {
                var role = roles.ToList().Find(r => r.Id == item.RoleId);
                var roleView = new RoleView
                {
                    RoleID = role.Id,
                    Name = role.Name
                };
                rolesView.Add(roleView);
            }

            var userView = new UserView
            {
                UserID = user.Id,
                Name = user.UserName,
                Email = user.Email,
                Roles = rolesView
            };

            return View(userView);
        }

        // GET: AddRole
        public ActionResult AddRole(string userID)
        {
            var users = GetUsers();
            var user = users.Find(u => u.Id == userID);
            if(user == null)
            {
                return HttpNotFound();
            }
            var userView = new UserView
            {
                UserID = user.Id,
                Name = user.UserName,
                Email = user.Email
            };

            
            var list = GetRoles();
            list.Add(new IdentityRole { Id = "", Name = "[Select a Role]"});
            list = list.OrderBy(l => l.Name).ToList();
            ViewBag.RoleID = new SelectList(list, "Id", "Name");

            return View(userView);
        }

        // POST: AddRole
        [HttpPost]
        public ActionResult AddRole(string userID, FormCollection form)
        {
            var roleID = Request["RoleID"];

            var users = GetUsers();
            var roles = GetRoles();

            var user = users.Find(u => u.Id == userID);

            var userView = new UserView
            {
                UserID = user.Id,
                Name = user.UserName,
                Email = user.Email
            };

            // Verifico si se selecciono algun rol
            if (string.IsNullOrEmpty(roleID))
            {
                ViewBag.Error = "You must select a role.";

                var list = GetRoles();
                list.Add(new IdentityRole { Id = "", Name = "[Select a Role]" });
                list = list.OrderBy(l => l.Name).ToList();
                ViewBag.RoleID = new SelectList(list, "Id", "Name");
                return View(userView);
            }

            var role = roles.Find(r => r.Id == roleID);
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            if (!userManager.IsInRole(user.Id, role.Name)){
                userManager.AddToRole(user.Id, role.Name);
            }

            var rolesView = new List<RoleView>();
            foreach (var item in user.Roles)
            {
                role = roles.Find(r => r.Id == item.RoleId);

                var roleView = new RoleView
                {
                    RoleID = role.Id,
                    Name = role.Name
                };
                rolesView.Add(roleView);
            }
            userView.Roles = rolesView;

            return View("Roles", userView);
        }

        public ActionResult Delete(string userID, string roleID)
        {
            if(string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(roleID))
            {
                return HttpNotFound();
            }
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            var user = GetUsers().Find(u => u.Id == userID);
            var role = GetRoles().Find(r => r.Id == roleID);

            if(userManager.IsInRole(user.Id, role.Name))
            {
                userManager.RemoveFromRole(user.Id, role.Name);
            }
            var users = GetUsers();
            var roles = GetRoles();

            var rolesView = new List<RoleView>();
            foreach (var item in user.Roles)
            {
                role = roles.Find(r => r.Id == item.RoleId);
                var roleView = new RoleView
                {
                    RoleID = role.Id,
                    Name = role.Name
                };
                rolesView.Add(roleView);
            }

            var userView = new UserView
            {
                UserID = user.Id,
                Name = user.UserName,
                Email = user.Email,
                Roles = rolesView
            };

            return View("Roles", userView);
        }

        // Methods

        public List<IdentityRole> GetRoles()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var roles = roleManager.Roles.ToList();
            return roles;
        }

        public List<ApplicationUser> GetUsers()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var users = userManager.Users.ToList();
            return users;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}