using Notestash_Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Notestash_Admin.Controllers
{
    public class RegisterController : Controller
    {
        // GET: Display registration page
        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }
        //POST: Add a user through registration page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(signUp User)
        {
            bool Status = false;
            string message = "";

            if (!ModelState.IsValid)
                message = "Invalid Request!";

            signUp ob = new signUp();
            //use out for exceptions
            int created = ob.Create(User);

            if (created == 2)
            {
                ModelState.AddModelError("EmailExist", "Email Already Exists!");
                return View(User);
            }
            else if(created == 1)
            {
                message = "Successful Registration!";
                Status = true;
                
            }
            else
            {
                message = "Unsuccessful Registration!";
            }
            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(User);
        }
    }
}