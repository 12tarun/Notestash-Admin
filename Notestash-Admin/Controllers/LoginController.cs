using Notestash_Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SecurityDriven.Inferno.Kdf;
using PBKDF2 = SecurityDriven.Inferno.Kdf.PBKDF2;
using SecurityDriven.Inferno.Extensions;
using static SecurityDriven.Inferno.SuiteB;
using static SecurityDriven.Inferno.Utils;

namespace Notestash_Admin.Controllers
{
    public class LoginController : Controller
    {

        // GET: Display login page.
        [HttpGet]
        public ActionResult signIn()
        {
            return View();
        }

        // POST: Login admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(signIn User)
        {
            try
            {
                using (Notestash_Database_Entities db = new Notestash_Database_Entities())
                {
                    var user = db.tblUsers.FirstOrDefault(e => e.Email.Equals(User.Email));
                    if (user != null)
                    {
                        var sha384Factory = HmacFactory;
                        byte[] derivedKey;
                        string hashedPassword = null;
                        string suppliedPassword = User.Password;
                        byte[] passwordBytes = SafeUTF8.GetBytes(suppliedPassword);

                        using (var pbkdf2 = new PBKDF2(sha384Factory, passwordBytes, user.Salt, 256 * 1000))
                            derivedKey = pbkdf2.GetBytes(384 / 8);


                        using (var hmac = sha384Factory())
                        {
                            hmac.Key = derivedKey;
                            hashedPassword = hmac.ComputeHash(passwordBytes).ToBase16();
                        }

                        var userCredentials = db.tblUsers.FirstOrDefault(e => e.Email.Equals(user.Email) && e.Password.Equals(hashedPassword) && e.AdminOrUser == 2);

                        if (userCredentials != null)
                        {
                            int timeout = User.RememberMe ? 52560 : 20;
                            var ticket = new FormsAuthenticationTicket(User.Email, User.RememberMe, timeout);
                            string encrypted = FormsAuthentication.Encrypt(ticket);
                            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                            cookie.Expires = DateTime.Now.AddMinutes(timeout);
                            cookie.HttpOnly = true;
                            Response.Cookies.Add(cookie);
                            return RedirectToAction("User_Data", "UserData");                           
                        }
                        else
                        {
                            ModelState.AddModelError("WrongCredentials", "Wrong Credentials!");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("WrongCredentials", "Wrong Credentials!");
                    }
                }
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
                ModelState.AddModelError("BadRequest", "Invalid Request!");
            }
            return View();
        }

        // Logout
        [Authorize]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("SignIn", "Login");
        }
    }
}