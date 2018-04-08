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

     // Show users' data
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

     // Delete a user or an admin  
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
                    if(id != (int)Session["Login"])                                                // User cannot delete himself as there should be atleast one admin                                                                                                  
                    {                                                                              // if he deletes all other admins.
                        var deleteUser = db.tblUsers.Where(e => e.Id == id).FirstOrDefault();      ///////////////
                        db.tblUsers.Remove(deleteUser);
                        db.SaveChanges();
                        return RedirectToAction("User_Data", "UserData");
                    }
                    else
                    {
                        return RedirectToAction("User_Data", "UserData");
                    }
                }
            }
        }
    }
}