using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Notestash_Admin.Controllers
{
    public class UserDataController : Controller
    {
        // Display user information
        [Authorize]
        public ActionResult User_Data()
        {
            using (Notestash_Database_Entities db = new Notestash_Database_Entities())
            {
                var users = db.tblUsers.ToList();
                return View(users);
            }             
        }
    }
}