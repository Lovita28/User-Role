using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using MVCUserRole.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MVCUserRole.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        AttendanceDBEntities2 cone = new AttendanceDBEntities2();
            // GET: Users 
            public Boolean isAdminUser()
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity;
                    ApplicationDbContext context = new ApplicationDbContext();
                    var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                    var s = UserManager.GetRoles(user.GetUserId());
                    if (s[0].ToString() == "Admin")
                    {
                        return true;
                    ViewBag.rule = "Admin";
                    }
                    else
                    {
                        return false;
                    ViewBag.rule = "User";
                    }
                }
                return false;
            }
            // GET: Users
            public ActionResult Index()
         {
            var userId = User.Identity.GetUserId();

            var conn = (from con in db.Users select con).ToList();
            ViewBag.loop = conn;
            var pict = (from pi in cone.Fille select pi).ToList();
            ViewBag.pictt = pict;

            ViewBag.del = TempData["delete"];
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ViewBag.Name = user.Name;

                ViewBag.displayMenu = "No";

                if (isAdminUser())
                {
                    ViewBag.displayMenu = "Yes";
                }
                return View();
            }
            else
            {
                ViewBag.Name = "Not Logged IN";
            }
            return View();
        }

        public ActionResult Delete (string id)
        {
            var carr = (from search in db.Users where search.Id == id select search).ToList();
            ViewBag.hap = carr;
            return View();
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var hapus = db.Users.Find(id);
            db.Users.Remove(hapus);
            db.SaveChanges();
            TempData["Delete"] = "Data has been deleted";
            return RedirectToAction("Index");
        }
        public ActionResult Status(string id)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var userId = UserManager.FindById(id).Id;
            if (UserManager.GetLockoutEnabled(userId) == true )
            {
                UserManager.SetLockoutEnabled(userId, false);
                
            }
            else
            {
                UserManager.SetLockoutEnabled(userId, true);
              
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}