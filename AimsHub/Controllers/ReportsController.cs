using AimsHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AimsHub.Controllers
{
    public class ReportsController : Controller
    {
        PatientLogModel db = new PatientLogModel();
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Default()
        {

            return View();
        }

        public ActionResult Default(ReportsParameter viewM)
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