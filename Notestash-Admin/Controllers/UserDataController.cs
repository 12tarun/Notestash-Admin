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
            Session["CannotDelete"] = null;

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
                    if (Session["CannotDelete"] != null)
                    {
                        ModelState.AddModelError("CannotDelete", "Admin cannot delete himself!");
                    }

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
                    var deleteUser = db.tblUsers.Where(e => e.Id == id).FirstOrDefault();
                    if (id != (int)Session["Login"])                                                // User cannot delete himself as there should be atleast one admin                                                                                                  
                    {                                                                               // if he deletes all other admins.                    
                        db.tblUsers.Remove(deleteUser);
                        db.SaveChanges();
                        return RedirectToAction("User_Data", "UserData");
                    }
                    else
                    {
                        Session["CannotDelete"] = 1;
                        return RedirectToAction("SelectedUser", "UserData", new { id = deleteUser.Id });
                    }
                }
            }
        }
    }
}