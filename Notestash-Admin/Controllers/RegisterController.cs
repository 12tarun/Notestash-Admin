using Notestash_Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
        // POST: Add a user through registration page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp([Bind(Exclude = "IsEmailVerified,ActivationCode")]signUp User)
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
                    SendVerificationLink(created,User.ActivationCode.ToString());
                    message = "Successful Registration! Activation Code has been sent to your email id.";
                    Status = true;
                    ViewBag.Message = message;
                    ViewBag.Status = Status;
                }
            }
            return View(User);
        }
        // Email Verification
        [NonAction]
        public void SendVerificationLink(string Email, string ActivationCode)
        {
            var verifyUrl = "/Register/VerifyAccount/" + ActivationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var fromEmail = new MailAddress("suryakant.rocky@gmail.com","Notestash");
            var toEmail = new MailAddress(Email);
            var fromEmailPassword = "suryasharma";
            string subject = "Your account is successfully created!";
            string body = "<br/><br/>Please click on the link below to activate your Notestash-Admin account."+"<br/><br/><a href='"+link+"'>"+link+"</a>";

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
    }
}