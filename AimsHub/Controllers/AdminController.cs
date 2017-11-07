using AimsHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AimsHub.Controllers
{
    public class AdminController : Controller
    {
        private PatientLogModel db = new PatientLogModel();

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}