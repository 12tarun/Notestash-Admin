using Notestash_Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
    public class RegisterController : Controller
    {
        // GET: Display registration page.
        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }
        // POST: Add a user through registration page.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp([Bind(Exclude = "IsEmailVerified,ActivationCode,ProfilePicture")]signUp User)
        {
            bool Status = false;
            string message = "";

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("BadRequest", "Registration failed!");
            }
            else
            {
                signUp ob = new signUp();
                string created = ob.Create(User);

                if (created == "exists")
                {
                    ModelState.AddModelError("EmailExist", "Email Already Exists!");
                }
                else if (created == "error")
                {
                    ModelState.AddModelError("BadRequest", "Registration failed!");
                }
                else
                {
                    int i = created.IndexOf(" ");
                    int sizeOfCode = created.Length - i - 1;
                    string passedEmail = created.Substring(0, i);
                    string passedCode = created.Substring((i + 1), sizeOfCode);
                    SendVerificationLink(passedEmail, passedCode);
                    message = "Successful Registration! Activation Code has been sent to your email id.";
                    Status = true;
                    ViewBag.Message = message;
                    ViewBag.Status = Status;
                }
            }
            return View(User);
        }

        // Send email verification link.
        [NonAction]
        public void SendVerificationLink(string Email, string ActivationCode)
        {
            var verifyUrl = "/Register/VerifyAccount/" + ActivationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var fromEmail = new MailAddress("suryakant.rocky@gmail.com", "Notestash");
            var toEmail = new MailAddress(Email);
            var fromEmailPassword = "suryasharma";
            string subject = "Your account is successfully created!";
            string body = "<br/><br/>Please click on the link below to activate your Notestash-Admin account." + "<br/><br/><a href='" + link + "'>" + link + "</a>";

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
        // GET: Verify account on clicking on activation code sent in email.
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (Notestash_Database_Entities db = new Notestash_Database_Entities())
            {
                var activate = db.tblUsers.Where(e => e.ActivationCode == new Guid(id)).FirstOrDefault();
                if (activate != null)
                {
                    activate.IsEmailVerified = 1;
                    db.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request!";
                }
            }
            ViewBag.Status = Status;
            return View();
        }
        // GET: Display login page.
        //    [HttpGet]
        //   public ActionResult SignIn()
        // {
        //     return View();
        // }
        // POST: Login user.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult SignIn(signIn User, string ReturnUrl = "")
        //{
        //    try
        //    {
        //        using (Notestash_Database_Entities db = new Notestash_Database_Entities())
        //        {
        //            var user = db.tblUsers.FirstOrDefault(e => e.Email.Equals(User.Email));

        //            var sha384Factory = HmacFactory;
        //            byte[] derivedKey;
        //            string hashedPassword = null;
        //            string suppliedPassword = User.Password;
        //            byte[] passwordBytes = SafeUTF8.GetBytes(suppliedPassword);

        //            using (var pbkdf2 = new PBKDF2(sha384Factory, passwordBytes, user.Salt, 256 * 1000))
        //                derivedKey = pbkdf2.GetBytes(384 / 8);


        //            using (var hmac = sha384Factory())
        //            {
        //                hmac.Key = derivedKey;
        //                hashedPassword = hmac.ComputeHash(passwordBytes).ToBase16();
        //            }



        //            var userCredentials =
        //                db.tblUsers.FirstOrDefault(e => e.Email.Equals(User.Email) && e.Password.Equals(hashedPassword));

        //            if (userCredentials != null)
        //            {
        //                int timeout = User.RememberMe ? 52560 : 20;
        //                var ticket = new FormsAuthenticationTicket(User.Email, User.RememberMe, timeout);
        //                string encrypted = FormsAuthentication.Encrypt(ticket);
        //                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
        //                cookie.Expires = DateTime.Now.AddMinutes(timeout);
        //                cookie.HttpOnly = true;
        //                Response.Cookies.Add(cookie);

        //                if (Url.IsLocalUrl(ReturnUrl))
        //                {
        //                    return Redirect(ReturnUrl);
        //                }
        //                else
        //                {
        //                    return RedirectToAction("Index", "Home");
        //                }
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("WrongCredentials", "Wrong Credentials!");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("BadRequest", "Invalid Request!");
        //    }
        //    return View();
        //}
        // Logout
        [Authorize]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("SignIn", "Register");
        }
    }
}