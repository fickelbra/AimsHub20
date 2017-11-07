using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AimsHub.Models;
using AimsHub.ViewModels;
using AimsHub.DAL;

namespace AimsHub.Controllers
{
    public class ReferringPracticeController : Controller
    {
        private PatientLogModel db = new PatientLogModel();
        private IQueryable<ReferringPractice> refPracQueryGenerator(string pracname, string address1, string address2, string city, string fax, List<string> hosp, List<string> phy)
        {
            IQueryable<ReferringPractice> query = from r in db.ReferringPractices
                                                  select r;

            if (pracname != "")
            {
                query = query.Where(r => r.PracName.Contains(pracname));
            }
            if (address1 != "")
            {
                query = query.Where(r => r.Address1.Contains(address1));
            }
            if (address2 != "")
            {
                query = query.Where(r => r.Address2.Contains(address2));
            }
            if (city != "")
            {
                query = query.Where(r => r.City.Contains(city));
            }
            if (fax != "")
            {
                query = query.Where(r => r.Fax.Contains(fax));
            }
            if (hosp.Any())
            {
                List<int> ids = new List<int>();

                //ids = (from r in db.RefPractUser
                //       join u in db.UserDetails on r.RefPracUser equals u.UserID
                //       where hosp.Contains(u.DefaultHospital)
                //       select r.RefPracID).ToList();
                query = query.Where(r => ids.Contains(r.PracID));

            }
            if (phy.Any())
            {
                List<int> ids = new List<int>();
                //Return ids

                query = query.Where(r => ids.Contains(r.PracID));
            }

            return query;
        }

        // GET: ReferringPractice
        public ActionResult Index()
        {
            ReferringPracticeIndexViewModel viewM = new ReferringPracticeIndexViewModel();
            string strPracName = "";
            string strAddress1 = "";
            string strAddress2 = "";
            string strCity = "";
            string strFax = "";
            List<string> hospitals = new List<string>();
            List<string> refphys = new List<string>();

            if (Session["refPracName"] != null)
            {
                strPracName = Session["refPracName"].ToString();
            }
            if (Session["refAddress1"] != null)
            {
                strAddress1 = Session["refAddress1"].ToString();
            }
            if (Session["refAddress2"] != null)
            {
                strAddress2 = Session["refAddress2"].ToString();
            }
            if (Session["refCity"] != null)
            {
                strCity = Session["refCity"].ToString();
            }
            if (Session["refFax"] != null)
            {
                strFax = Session["refFax"].ToString();
            }

            if (Session["refHospitals"] != null)
            {
                hospitals = (List<string>)Session["refHospitals"];
            }
            if (Session["refRefPhys"] != null)
            {
                refphys = (List<string>)Session["refRefPhys"];
            }

            viewM.ReferringPractices = refPracQueryGenerator(strPracName, strAddress1, strAddress2, strCity, strFax, hospitals, refphys).AsEnumerable();
            viewM.filterPracticeName = strPracName;
            viewM.filterAddressLine1 = strAddress1;
            viewM.filterCity = strCity;
            viewM.filterFax = strFax;
            viewM.SelectedHospitals = hospitals;
            viewM.SelectedRefPhy = refphys;
            viewM.HospitalList = DataCollections.getHospital(db);
            viewM.RefPhyList = DataCollections.getPCP(db);

            return View(viewM);
        }

        [HttpPost]
        public ActionResult Index(ReferringPracticeIndexViewModel viewM)
        {
            string strPracName = "";
            string strAddress1 = "";
            string strAddress2 = "";
            string strCity = "";
            string strFax = "";
            string[] splitList;
            List<string> hospitals = new List<string>();
            List<string> refphys = new List<string>();

            //if (viewM.filterPracticeName != null)
            //{
            //    strPracName = viewM.filterPracticeName;
            //}
            //if (viewM.filterAddressLine1 != null)
            //{
            //    strAddress1 = viewM.filterAddressLine1;
            //}
            //if (viewM.filterCity != null)
            //{
            //    strCity = viewM.filterCity;
            //}
            //if (viewM.filterFax != null)
            //{
            //    strFax = viewM.filterFax;
            //}
            //if (viewM.hidHospitals != null)
            //{
            //    splitList = viewM.hidHospitals.Split(new char[] { ',' });
            //    hospitals = splitList.ToList();
            //}
            //if (viewM.hidRefPhy != null)
            //{
            //    splitList = viewM.hidRefPhy.Split(new char[] { ',' });
            //    refphys = splitList.ToList();
            //}

            ReferringPracticeIndexViewModel returnM = new ReferringPracticeIndexViewModel();

            returnM.ReferringPractices = refPracQueryGenerator(strPracName, strAddress1, strAddress2, strCity, strFax, hospitals, refphys).AsEnumerable();
            returnM.filterPracticeName = strPracName;
            returnM.filterAddressLine1 = strAddress1;
            returnM.filterCity = strCity;
            returnM.filterFax = strFax;
            returnM.SelectedHospitals = hospitals;
            returnM.SelectedRefPhy = refphys;
            returnM.HospitalList = DataCollections.getHospital(db);
            returnM.RefPhyList = DataCollections.getPCP(db);
            return View(returnM);
        }

        // GET: ReferringPractice/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReferringPractice referringPractice = db.ReferringPractices.Find(id);
            if (referringPractice == null)
            {
                return HttpNotFound();
            }
            return View(referringPractice);
        }

        // GET: ReferringPractice/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReferringPractice/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PracID,PracName,Address1,Address2,Address3,City,State,Zip,Phone,Fax,Email,OfficeManager,Other,PDFPassword,EmailNotification,FaxNotification")] ReferringPractice referringPractice)
        {
            if (ModelState.IsValid)
            {
                db.ReferringPractices.Add(referringPractice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(referringPractice);
        }

        // GET: ReferringPractice/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReferringPractice referringPractice = db.ReferringPractices.Find(id);
            if (referringPractice == null)
            {
                return HttpNotFound();
            }
            return View(referringPractice);
        }

        // POST: ReferringPractice/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PracID,PracName,Address1,Address2,Address3,City,State,Zip,Phone,Fax,Email,OfficeManager,Other,PDFPassword,EmailNotification,FaxNotification")] ReferringPractice referringPractice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(referringPractice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(referringPractice);
        }

        // GET: ReferringPractice/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReferringPractice referringPractice = db.ReferringPractices.Find(id);
            if (referringPractice == null)
            {
                return HttpNotFound();
            }
            return View(referringPractice);
        }

        // POST: ReferringPractice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ReferringPractice referringPractice = db.ReferringPractices.Find(id);
            db.ReferringPractices.Remove(referringPractice);
            db.SaveChanges();
            return RedirectToAction("Index");
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
