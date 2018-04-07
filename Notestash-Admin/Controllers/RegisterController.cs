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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Notestash_Admin.Controllers
{
    public class RegisterController : Controller
    {
        // GET: Display registration page.
        [HttpGet]
        public ActionResult SignUp()
        {
            if (Session["Login"] == null)
            {
                return RedirectToAction("SignIn", "Login");
            }

            return View();
        }
        // POST: Add an Admin through registration page.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp([Bind(Exclude = "IsEmailVerified,ActivationCode,ProfilePicture")]signUp User)
        {
            bool Status = false;
            bool Invalid = false;

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
                    Invalid = true;
                    ViewBag.Invalid = Invalid;
                    ModelState.AddModelError("BadRequest", "Registration failed!");
                }
                else
                {
                    int i = created.IndexOf(" ");
                    int sizeOfCode = created.Length - i - 1;
                    string passedEmail = created.Substring(0, i);
                    string passedCode = created.Substring((i + 1), sizeOfCode);
                    SendVerificationLink(passedEmail, passedCode);
                    ModelState.AddModelError("Success", "Admin has been added successfully! Activation Code has been sent to his email id.");
                    Status = true;             
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
           // bool Status = false;
            using (Notestash_Database_Entities db = new Notestash_Database_Entities())
            {
                bool deleted = false;
                bool expired = false;
                bool activated = false;
                var activate = db.tblUsers.Where(e => e.ActivationCode == new Guid(id)).FirstOrDefault();
                if (activate == null)
                {
                    deleted = true;
                    ViewBag.Deleted = deleted;
                    ModelState.AddModelError("Deleted", "Account deleted! Create Again!");
                    return View();
                }
                else
                {
                    DateTime expire = activate.Created_at.Value.AddDays(1);
                    DateTime present = DateTime.Now;
                    if (present >= expire)
                    {
                        expired = true;
                        ViewBag.Expired = expired;
                        ModelState.AddModelError("Expired", "Link Expired! Register Again!");
                        return View();
                    }
                    else
                    {
                        activate.IsEmailVerified = 1;
                        db.SaveChanges();
                        activated = true;
                        ViewBag.Activated = activated;
                        ModelState.AddModelError("Activated", "Congratulations! Account activated! You are also a Notestash admin now.");
                        return View();
                    }
                }
            }          
        }
    }
}