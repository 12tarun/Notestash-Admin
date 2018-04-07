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
using System.Net.Mail;
using System.Net;
using SecurityDriven.Inferno;

namespace Notestash_Admin.Controllers
{
    public class LoginController : Controller
    {

        // GET: Display login page.
        [HttpGet]
        public ActionResult signIn()
        {
            if (Session["Login"] != null)
            {
                return RedirectToAction("User_Data", "UserData");
            }

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
                            Session["Login"] = user.Email;
                            
                            // cookie based login

                            //int timeout = User.RememberMe ? 52560 : 20;
                            //var ticket = new FormsAuthenticationTicket(User.Email, User.RememberMe, timeout);
                            //string encrypted = FormsAuthentication.Encrypt(ticket);
                            //var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                            //cookie.Expires = DateTime.Now.AddMinutes(timeout);
                            //cookie.HttpOnly = true;
                            //Response.Cookies.Add(cookie);
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

     // Forgot Password
        [HttpGet]
        public ActionResult forgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult forgotPassword(forgotPassword User)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("BadRequest", "Invalid Request!");
            }
                
            try
            {
                using (Notestash_Database_Entities db = new Notestash_Database_Entities())
                {
                    var emailId = db.tblUsers.FirstOrDefault(e => e.Email == User.Email);
                    emailId.forgotPasswordCode = Guid.NewGuid();
                    db.SaveChanges();
                    changePasswordEmail(emailId.Email, emailId.forgotPasswordCode.ToString());
                    ModelState.AddModelError("Sent", "Link to change password has been sent to your email id.");
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                ModelState.AddModelError("BadRequest", "Invalid Request!");
            }
            return View();
        }

     // Email for changing password
        [NonAction]
        public void changePasswordEmail(string Email, string passwordChangeKey)
        {
            var verifyUrl = "/Login/changePassword/" + passwordChangeKey;
        //  Uri link = new Uri(Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.PathAndQuery, verifyUrl));
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var fromEmail = new MailAddress("suryakant.rocky@gmail.com", "Notestash");
            var toEmail = new MailAddress(Email);
            var fromEmailPassword = "suryasharma";
            string subject = "Change Password";
            string body = "<br/><br/>Please click on the link below to change your password." + "<br/><br/><a href='" + link + "'>" + link + "</a>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword),
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

        [HttpGet]
        public ActionResult changePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult changePassword(string id, changePassword pass)
        {
            try
            {
                using (Notestash_Database_Entities db = new Notestash_Database_Entities())
                {

                    if(pass.newPassword.Equals(pass.confirmNewPassword) && pass.newPassword.Length >= 6 && pass.newPassword.Length <= 15)
                    {
                        var passwordChanged = db.tblUsers.Where(e => e.forgotPasswordCode == new Guid(id)).FirstOrDefault();
                        string newPass = pass.newPassword;

                        var sha384Factory = HmacFactory;
                        var random = new CryptoRandom();

                        byte[] derivedKey;
                        string hashedPassword = null;
                        string passwordText = newPass;

                        byte[] passwordBytes = SafeUTF8.GetBytes(passwordText);
                        var salt = random.NextBytes(384 / 8);

                        using (var pbkdf2 = new PBKDF2(sha384Factory, passwordBytes, salt, 256 * 1000))
                            derivedKey = pbkdf2.GetBytes(384 / 8);


                        using (var hmac = sha384Factory())
                        {
                            hmac.Key = derivedKey;
                            hashedPassword = hmac.ComputeHash(passwordBytes).ToBase16();
                        }

                        passwordChanged.Password = hashedPassword;
                        passwordChanged.Salt = salt;
                        passwordChanged.forgotPasswordCode = null;
                        db.SaveChanges();
                        ModelState.AddModelError("Changed", "Password changed successfully!");
                    }
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                ModelState.AddModelError("BadRequest", "Error occurred, please try again!");
            }
            return View();
        }


        // Logout
        public ActionResult SignOut()
        {
            Session["Login"] = null;
            return RedirectToAction("SignIn", "Login");
        }
    }
}