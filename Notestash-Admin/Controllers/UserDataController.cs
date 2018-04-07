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

        [HttpGet]
        public ActionResult SelectedUser(int id)
        {
            // working till here fetch the info from db of selected user now.....
            return View();
        }
    }
}