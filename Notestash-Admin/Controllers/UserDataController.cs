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
    }
}