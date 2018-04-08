using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Notestash_Admin.Controllers
{
    public class UserDataController : Controller
    {

     // Show user data
        [HttpGet]
        public ActionResult User_Data()
        {
            if(Session["Login"] == null)
            {
                return RedirectToAction("SignIn", "Login");
            }

            using (Notestash_Database_Entities db = new Notestash_Database_Entities())
            {
                var users = db.tblUsers.ToList();
                return View(users);
            }             
        }


     // Display selected User's complete activity
        [HttpGet]
        public ActionResult SelectedUser(int id)
        {
            if (Session["Login"] == null)
            {
                return RedirectToAction("SignIn", "Login");
            }
            else
            {
                using (Notestash_Database_Entities db = new Notestash_Database_Entities())
                {
                    var selectUser = db.tblUsers.Where(e => e.Id == id).FirstOrDefault();
                    return View(selectUser);
                }
            }
        }

        
        public ActionResult DeleteSelectedUser(int id)
        {
            if (Session["Login"] == null)
            {
                return RedirectToAction("SignIn", "Login");
            }
            else
            {
                using (Notestash_Database_Entities db = new Notestash_Database_Entities())
                {
                    var deleteUser = db.tblUsers.Where(e => e.Id == id).FirstOrDefault();
                    db.tblUsers.Remove(deleteUser);
                    db.SaveChanges();
                    return RedirectToAction("User_Data", "UserData");
                }
            }
        }
    }
}