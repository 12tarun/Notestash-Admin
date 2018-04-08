using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Notestash_Admin.Controllers
{
    public class NotesController : Controller
    {
     // Display Notes
        [HttpGet]
        public ActionResult Notes_Data()
        {
            if (Session["Login"] == null)
            {
                return RedirectToAction("SignIn", "Login");
            }

            using (Notestash_Database_Entities db = new Notestash_Database_Entities())
            {
                var notes = db.tblNotes.ToList();
                return View(notes);
            }
        }
    }
}