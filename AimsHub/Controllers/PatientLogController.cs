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
using AimsHub.Security;
using System.Linq.Dynamic;
using AimsHub.DAL;
using static AimsHub.Security.ControllerAuthentication;

namespace AimsHub.Controllers
{
    public class PatientLogController : Controller
    {
        private PatientLogModel db = new PatientLogModel();
        private const int intDayFallback = -1;


        //This function returns results from PatientLog table for all PatientLog Views aside from PatientSort
        private IQueryable<PatientLog> patientLogQueryGenerator(DateTime fromDate, DateTime toDate, List<string> selectedHosp, List<string> selectedServ,
                                                                GridFilter.SortDirections direction, string sortColumn, bool assigned)
        {
            //Query PatientLog by physicians and date/time range
            string user = HubSecurity.getLoggedInUserID();
            IQueryable<PatientLog> query;

            //Alter query to show all Assigned entries if selected to do so
            if (!assigned)
            {
                if (sortColumn == null)
                {
                    query = from d in db.PatientLogs
                            where d.ServiceDate >= fromDate &&
                            d.ServiceDate <= toDate
                            && d.Physician == user
                            orderby d.PatientName, d.ServiceDate
                            select d;
                }
                else
                {
                    query = from d in db.PatientLogs
                            where d.ServiceDate >= fromDate &&
                            d.ServiceDate <= toDate
                            && d.Physician == user
                            select d;
                }
            }
            else
            {
                if (sortColumn == null)
                {
                    query = from d in db.PatientLogs
                            where d.Physician == user && d.ServiceType == "Assigned"
                            orderby d.PatientName, d.ServiceDate
                            select d;
                }
                else
                {
                    query = from d in db.PatientLogs
                            where d.Physician == user && d.ServiceType == "Assigned"
                            select d;
                }
            }

            //Apply hospital filters if any are provided
            if (selectedHosp.Any())
            {
                query = query.Where(h => selectedHosp.Contains(h.Hospital));
            }

            //Apply service type filters only if not showing assigned view
            if (!assigned)
            {
                if (selectedServ.Any())
                {
                    query = query.Where(s => selectedServ.Contains(s.ServiceType));
                }
            }

            //Apply sorting
            if (direction == GridFilter.SortDirections.Ascending)
            {
                if (sortColumn == "PatientName")
                {
                    query = query.OrderBy(sortColumn + ", ServiceDate");
                }
                else
                {
                    query = query.OrderBy(sortColumn);
                }
            }
            else
            {
                query = query.OrderBy(sortColumn + " DESC");
            }

            List<int> idList = query.Select(p => p.ID).ToList();
            Session["patientLogListOfID"] = idList;

            //Update user preferences if Assigned is not selected
            if (!assigned)
            {
                Dictionary<string, string> filters = new Dictionary<string, string>();
                string hospPref = null;
                foreach (string hosp in selectedHosp)
                {
                    hospPref += hosp + ",";
                }
                if (hospPref != null)
                {
                    hospPref = hospPref.Substring(0, hospPref.Length - 1);
                }
                filters.Add("Hospital", hospPref);

                string servPref = null;
                foreach (string serv in selectedServ)
                {
                    servPref += serv + ",";
                }
                if (servPref != null)
                {
                    servPref = servPref.Substring(0, servPref.Length - 1);
                }
                filters.Add("ServiceType", servPref);

                if (direction == GridFilter.SortDirections.Ascending)
                {
                    filters.Add("SortDirection", "Ascending");
                }
                else
                {
                    filters.Add("SortDirection", "Descending");
                }
                filters.Add("SortColumn", sortColumn);

                DataSubmissions.SavePreferences(db, "PatientLog", "PatientLogIndex", filters);
            }
            
            return query;
        }

        //GET: PatientLog
        public ActionResult Index()
        {
            var viewM = new PatientLogIndexViewModel();
            var fromD = new DateTime();
            var toD = new DateTime();
            //List<string> physicians = new List<string>();
            List<string> hospitals = new List<string>();
            List<string> services = new List<string>();
            string[] splitList;
            string pref;
            bool assigned;
            //PatientLogIndexViewModel viewM;

            
            if (Session["patientLogFromDate"] != null && Session["patientLogToDate"] != null)
            {
                fromD = Convert.ToDateTime(Session["patientLogFromDate"]);
                toD = Convert.ToDateTime(Session["patientLogToDate"]);
            }
            else
            {
                fromD = getValidDate("", true);
                toD = getValidDate("", false);
            }
            if (Session["patientLogSortColumn"] != null)
            {
                viewM.SortColumn = Session["patientLogSortColumn"].ToString();
                viewM.SortDirection = (GridFilter.SortDirections)Session["patientLogSortDirection"];
            }
            
            pref = DataCollections.LoadPreference(db, "PatientLog", "PatientLogIndex", "Hospital");

            if (pref != null)
            {
                splitList = pref.Split(new char[] { ',' });
                hospitals = splitList.ToList();
            }
            pref = DataCollections.LoadPreference(db, "PatientLog", "PatientLogIndex", "ServiceType");
            if (pref != null)
            {
                splitList = pref.Split(new char[] { ',' });
                services = splitList.ToList();
            }

            if (Session["patientLogAssigned"] != null)
            {
                assigned = (bool)Session["patientLogAssigned"];
            }
            else
            {
                assigned = false;
            }

            Session["patientLogFromDate"] = fromD;
            Session["patientLogToDate"] = toD;
            Session["patientLogSortColumn"] = viewM.SortColumn;
            Session["patientLogSortDirection"] = viewM.SortDirection;
            //Session["patientLogPhysicians"] = physicians;
            //Session["patientLogHospitals"] = hospitals;
            //Session["patientLogServices"] = services;

            IQueryable<PatientLog> query = patientLogQueryGenerator(fromD, toD, hospitals, services, viewM.SortDirection, viewM.SortColumn, assigned);

            viewM.Patients = query.AsEnumerable();
            //Determines risk factor for each patient
            viewM.RiskFactor = new Dictionary<string, string>();
            foreach (PatientLog pat in viewM.Patients)
            {
                bool isHigh = (from p in db.PatientLogs where pat.MRN_FIN == p.MRN_FIN && p.ServiceType.Contains("DC -") && (p.ServiceDate >= DbFunctions.AddDays(pat.ServiceDate, -7) && p.ServiceDate < pat.ServiceDate) select p.ID).Any();

                if (isHigh)
                {
                    viewM.RiskFactor.Add(pat.ID.ToString(), "H");
                    continue;
                }

                bool isMed = (from p in db.PatientLogs where pat.MRN_FIN == p.MRN_FIN && p.ServiceType.Contains("DC -") && (p.ServiceDate >= DbFunctions.AddDays(pat.ServiceDate, -30) && p.ServiceDate < pat.ServiceDate) select p.ID).Any();

                if (isMed)
                {
                    viewM.RiskFactor.Add(pat.ID.ToString(), "M");
                }
            }

            if (viewM.RiskFactor == null)
            {
                viewM.RiskFactor.Add("testkey", "val1");
            }
            viewM.FromDate = fromD.ToShortDateString();
            viewM.ToDate = toD.ToShortDateString();
            //viewM.PhysicianList = DataCollections.getAIMSPhy(db);
            viewM.HospitalList = DataCollections.getHospital(db);
            viewM.ServiceList = DataCollections.getServiceType(db);
            //viewM.SelectedPhysicians = physicians;
            viewM.SelectedHospitals = hospitals;
            viewM.SelectedServices = services;

            return View("Index", viewM);
        }

        [HttpPost]
        public ActionResult Index(PatientLogIndexViewModel viewM)
        {
            var fromD = new DateTime();
            var toD = new DateTime();
            //List<string> physicians = new List<string>();
            List<string> hospitals = new List<string>();
            List<string> services = new List<string>();
            string[] splitList;
            bool assigned;

            //Manages physician input and builds a List<string> to be used in the query
            if (viewM.hidHospitals != null && viewM.hidHospitals != "")
            {
                splitList = viewM.hidHospitals.Split(new char[] { ',' });
                hospitals = splitList.ToList<string>();
            }
            if (viewM.hidServices != null && viewM.hidServices != "")
            {
                splitList = viewM.hidServices.Split(new char[] { ',' });
                services = splitList.ToList<string>();
            }
            if (viewM.Assigned)
            {
                assigned = (bool)viewM.Assigned;               
            }
            else
            {
                assigned = false;
            }


            fromD = getValidDate(viewM.FromDate, true);
            toD = getValidDate(viewM.ToDate, false);

            PatientLogIndexViewModel returnM = new PatientLogIndexViewModel();

            returnM.FromDate = fromD.ToShortDateString();
            returnM.ToDate = toD.ToShortDateString();
            //returnM.PhysicianList = DataCollections.getAIMSPhy(db);
            returnM.HospitalList = DataCollections.getHospital(db);
            returnM.ServiceList = DataCollections.getServiceType(db);
            //returnM.SelectedPhysicians = physicians;
            returnM.SelectedHospitals = hospitals;
            returnM.SelectedServices = services;
            returnM.Assigned = assigned;
            returnM.SortColumn = viewM.SortColumn;
            returnM.SortDirection = viewM.SortDirection;

            Session["patientLogFromDate"] = fromD;
            Session["patientLogToDate"] = toD;
            Session["patientLogSortColumn"] = returnM.SortColumn;
            Session["patientLogSortDirection"] = returnM.SortDirection;
            //Session["patientLogPhysicians"] = physicians;
            //Session["patientLogHospitals"] = hospitals;
            //Session["patientLogServices"] = services;
            //Session["patientLogAssigned"] = assigned;

            IQueryable<PatientLog> query = patientLogQueryGenerator(fromD, toD, hospitals, services, returnM.SortDirection, returnM.SortColumn, assigned);

            returnM.Patients = query.AsEnumerable<PatientLog>();
            returnM.RiskFactor = new Dictionary<string, string>();
            foreach (PatientLog pat in returnM.Patients)
            {
                bool isHigh = (from p in db.PatientLogs where pat.MRN_FIN == p.MRN_FIN && p.ServiceType.Contains("DC -") && (p.ServiceDate >= DbFunctions.AddDays(pat.ServiceDate, -7) && p.ServiceDate < pat.ServiceDate) select p.ID).Any();

                if (isHigh)
                {
                    returnM.RiskFactor.Add(pat.ID.ToString(), "H");
                    continue;
                }

                bool isMed = (from p in db.PatientLogs where pat.MRN_FIN == p.MRN_FIN && p.ServiceType.Contains("DC -") && (p.ServiceDate >= DbFunctions.AddDays(pat.ServiceDate, -30) && p.ServiceDate < pat.ServiceDate) select p.ID).Any();

                if (isMed)
                {
                    returnM.RiskFactor.Add(pat.ID.ToString(), "M");
                }
            }
            return View(returnM);
        }

        // GET: PatientLog sorted by a single patient's history in descending order
        public async Task<ActionResult> PatientSort(string patientName)
        {
            IQueryable<PatientLog> query = from d in db.PatientLogs
                                           where d.PatientName == patientName
                                           orderby d.ServiceDate descending
                                           select d;
            //ViewBag.isAdmin = (HubSecurity.isAdmin || HubSecurity.isSiteLeader) ? true : false;

            //Session["patientLogListOfID"] = new List<int>();
            return View(await query.ToListAsync());
        }

        // GET: PatientLog/Create
        public ActionResult Create()
        {
            PatientLogCreateViewModel viewM = new PatientLogCreateViewModel();

            viewM.GenderList = DataCollections.getGender(db);
            viewM.ServiceTypeList = DataCollections.getServiceType(db);
            viewM.HospitalList = DataCollections.getHospital(db);
            viewM.PatientClassList = DataCollections.getPatientClass(db);
            viewM.PCPList = DataCollections.getPCP(db, "BRO");
            return View(viewM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PatientLogCreateViewModel viewM)
        {
            if (ModelState.IsValid)
            {
                viewM.Patient.Physician = HubSecurity.getLoggedInUserID();
                DataSubmissions.CreatePatient(db, viewM.Patient);
                return RedirectToAction("Index");
            }
            return View(viewM.Patient);
        }

        // GET: PatientLog/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientLog patientLog = await db.PatientLogs.FindAsync(id);
            PatientLogEditViewModel viewM = new PatientLogEditViewModel();
            if (patientLog == null)
            {
                return HttpNotFound();
            }
            viewM.Patient = patientLog;

            //Checks if service type is faxable
            bool foundType = db.FaxServiceTypes.AsEnumerable().Any(f => f.Service == patientLog.ServiceType);

            //If faxable, check that a notification was not sent already, and return the result
            if (foundType)
            {
                string faxType = (from f in db.FaxServiceTypes
                                  where f.Service == patientLog.ServiceType
                                  select f.FaxType).Single() + "Notification";

                bool commSent = db.PCPCommunications.AsEnumerable().Where(c => c.PLRecord == id && c.DocumentType == faxType).Any();
                viewM.isFaxable = !commSent;
                viewM.alreadyFaxed = commSent;
            }
            else
            {
                viewM.isFaxable = false;
            }

            List<int> idList = (List<int>)Session["patientLogListOfID"];

            //Populates index variables that the view uses to set up previous/next logic
            for (int i = 0; i < idList.Count; i++)
            {
                if (id == idList[i])
                {
                    viewM.Indexer = i;
                    break;
                }
            }
            viewM.IndexerDisplay = viewM.Indexer + 1;
            if (idList.Count() == 1)
            {
                viewM.SafeIndexerPrev = 0;
                viewM.SafeIndexerNext = 0;
            }
            else
            {
                if (viewM.Indexer == 0)
                {
                    viewM.SafeIndexerPrev = 1;
                }
                else
                {
                    viewM.SafeIndexerPrev = viewM.Indexer - 1;
                }
                if (viewM.Indexer == (idList.Count() - 1))
                {
                    viewM.SafeIndexerNext = viewM.Indexer - 1;
                }
                else
                {
                    viewM.SafeIndexerNext = viewM.Indexer + 1;
                }
            }

            //Populates dropdownlists with selected value of current patient
            viewM.GenderList = DataCollections.getGender(db, viewM.Patient.Gender);
            viewM.HospitalList = DataCollections.getHospital(db, viewM.Patient.Hospital);
            viewM.PCPList = DataCollections.getPCP(db, viewM.Patient.Hospital, viewM.Patient.PCP_Practice);
            viewM.ServiceTypeList = DataCollections.getServiceType(db, viewM.Patient.ServiceType);
            viewM.PatientClassList = DataCollections.getPatientClass(db, viewM.Patient.PatientClass);
            viewM.PhysicianList = DataCollections.getAIMSPhy(db, viewM.Patient.Hospital, viewM.Patient.Physician);
            return View(viewM);
        }

        // POST: PatientLog/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Physician,Hospital,PCP_Practice,MRN_FIN,PatientName,DateCreated,ServiceType,Comments,LastUpdated,ServiceDate,RoomNo,AIMSComments,FaceSheet,Notes,AIMSBillingCodes,DOB,Gender,PatientClass")] PatientLog patientLog)
        {
            if (ModelState.IsValid)
            {
                patientLog.LastUpdated = DateTime.Now;
                db.Entry(patientLog).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(patientLog);
        }

        // GET: PatientLog/EditAll
        public ActionResult EditAll()
        {
            var fromD = new DateTime();
            var toD = new DateTime();

            //List<string> hospitals = new List<string>();

            if (Session["patientLogFromDate"] != null && Session["patientLogToDate"] != null)
            {
                fromD = Convert.ToDateTime(Session["patientLogFromDate"]);
                toD = Convert.ToDateTime(Session["patientLogToDate"]);
            }
            else
            {
                fromD = DateTime.Now.AddDays(intDayFallback);
                toD = DateTime.Now;
            }

            //if (Session["patientLogHospitals"] != null)
            //{
            //    hospitals = (List<string>)Session["patientLogHospitals"];
            //}

            PatientLogEditAllViewModel viewM = new PatientLogEditAllViewModel();
            viewM.FromDate = fromD.ToShortDateString();
            viewM.ToDate = toD.ToShortDateString();
            //viewM.isAdmin = (HubSecurity.isAdmin || HubSecurity.isSiteLeader);

            PatientLogEditViewModel oneEntry;
            List<int> patientList = (List<int>)Session["patientLogListOfID"];

            //Checks if list is null, this should never be the case but stops page from erroring out. This session variable is only empty
            //if you browse to this page before using the Index page
            if (patientList != null)
            {
                foreach (int patID in patientList)
                {
                    oneEntry = new PatientLogEditViewModel();
                    oneEntry.Patient = db.PatientLogs.Find(patID);

                    //oneEntry.GenderList = getGender(oneEntry.Patient.Gender);
                    oneEntry.HospitalList = DataCollections.getHospital(db, oneEntry.Patient.Hospital);
                    oneEntry.PCPList = DataCollections.getPCP(db, oneEntry.Patient.Hospital, oneEntry.Patient.PCP_Practice);
                    oneEntry.ServiceTypeList = DataCollections.getServiceType(db, oneEntry.Patient.ServiceType);
                    //oneEntry.PatientClassList = getPatientClass(oneEntry.Patient.PatientClass);
                    //oneEntry.PhysicianList = getAIMSPhy(oneEntry.Patient.Hospital, oneEntry.Patient.Physician);

                    viewM.Patients.Add(oneEntry);
                }
            }

            return View("EditAll", viewM);
        }

        // GET: PatientLog/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientLog patientLog = await db.PatientLogs.FindAsync(id);
            if (patientLog == null)
            {
                return HttpNotFound();
            }
            return View(patientLog);
        }

        // POST: PatientLog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DataSubmissions.DeletePatient(db, id);
            return RedirectToAction("Index");
        }

        // GET: PatientLog/Copy/5
        public async Task<ActionResult> Copy(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatientLog patientLog = await db.PatientLogs.FindAsync(id);
            if (patientLog == null)
            {
                return HttpNotFound();
            }
            return View(patientLog);
        }

        // POST: PatientLog/Copy/5
        [HttpPost, ActionName("Copy")]
        [ValidateAntiForgeryToken]
        public ActionResult CopyConfirmed(int id)
        {
            DataSubmissions.CopyPatient(db, id);
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

        //Parses date input; if invalid, From date is set to a week ago and To date is set to Today
        private DateTime getValidDate(string text, bool isFrom)
        {
            var retDate = new DateTime();
            if (text != null)
            {
                try
                {
                    //var fromD = new DateTime();
                    retDate = DateTime.Parse(text);
                    if (isFrom) { retDate = retDate.Date + new TimeSpan(0, 0, 0); } else { retDate = retDate.Date + new TimeSpan(23, 59, 59); }
                }
                catch
                {
                    if (isFrom) { retDate = DateTime.Now.AddDays(intDayFallback) + new TimeSpan(0, 0, 0); } else { retDate = DateTime.Now + new TimeSpan(23, 59, 59); }
                }
            }
            else
            {
                if (isFrom) { retDate = DateTime.Now.AddDays(intDayFallback) + new TimeSpan(0, 0, 0); } else { retDate = DateTime.Now + new TimeSpan(23, 59, 59); }
            }

            return retDate;
        }

        //Returns JSON list of PCPs that match the specified site
        public ActionResult jsonPCP(string site)
        {
            //var query = from u in db.Users
            //            join ud in db.UserDetails on u.UserID equals ud.UserID
            //            where (ud.UserType == "RefPhy") && (ud.DefaultHospital == site)
            //            orderby u.LastName
            //            select new { Value = (u.LastName + ", " + u.FirstName) };

            var query = DataCollections.getPCP(db, site);
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        //Returns JSON list of AIMS Physicians that match the specified site
        public ActionResult jsonAIMSPhy(string site)
        {
            //var query = from u in db.Users
            //            join ud in db.UserDetails on u.UserID equals ud.UserID
            //            where (ud.UserType == "AIMSPhy" && ud.Active == true) && (ud.DefaultHospital == site)
            //            orderby u.LastName
            //            select new { Text = (u.LastName + ", " + u.FirstName), Value = u.UserID };

            var query = DataCollections.getAIMSPhy(db, site);
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        //Submits a single patient to the server for modification via JSON
        [HttpPost]
        public async Task<ActionResult> jsonSubmitPatient(PatientLog patientLog)
        {
            if (ModelState.IsValid)
            {
                patientLog.LastUpdated = DateTime.Now;
                db.Entry(patientLog).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Content("Patient information has been saved successfully!");
            }
            else
            {
                return Content("Something went wrong when saving this patient. Please notify IT");
            }

        }

        //Submits a single patient to the server for modification via JSON
        [HttpPost]
        public async Task<ActionResult> jsonSubmitPatientAndFax(PatientLog patientLog)
        {
            if (ModelState.IsValid)
            {
                patientLog.LastUpdated = DateTime.Now;
                db.Entry(patientLog).State = EntityState.Modified;
                await db.SaveChangesAsync();

                string commType = (from f in db.FaxServiceTypes
                                   where f.Service == patientLog.ServiceType
                                   select f.FaxType).Single().ToString();

                PCPCommunication comm = new PCPCommunication(patientLog.Physician, commType + "Notification", patientLog.Hospital, patientLog.PatientName, patientLog.ID, "313-867-5309", patientLog.DOB);
                db.Entry(comm).State = EntityState.Added;
                await db.SaveChangesAsync();
                return Content("Patient information has been saved successfully!");
            }
            else
            {
                return Content("Something went wrong when saving this patient. Please notify IT");
            }

        }

        //Submits a list of patients to the server for modification via JSON
        [HttpPost]
        public async Task<ActionResult> jsonSubmitPatientList(List<PatientLog> patientLogs)
        {
            if (ModelState.IsValid)
            {
                foreach (PatientLog pat in patientLogs)
                {
                    pat.LastUpdated = DateTime.Now;
                    db.Entry(pat).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                return Content("List saved successfully");
            }
            return Content("Something went wrong?");
        }

        //Saves notes/comments from PatientLog Index notes popup
        [HttpPost]
        public ActionResult jsonSaveNotes(string ID, string Notes, string Comments)
        {
            int newID = Convert.ToInt32(ID);
            PatientLog pat = db.PatientLogs.Find(newID);
            if (pat == null)
            {
                return Json("Something went wrong, please notify IT immediately");
            }
            else
            {
                pat.Notes = Notes;
                pat.Comments = Comments;

                db.Entry(pat).State = EntityState.Modified;
                db.SaveChanges();
                return Json("Success");
            }           
        }

        //Checks faxable status of patient from service type and id
        public ActionResult jsonCheckFaxability(string serviceType, int id)
        {
            //Check to see if this is a faxable service type before doing anything
            if (db.FaxServiceTypes.Any(f => f.Service == serviceType))
            {
                //Get communication type for this service type
                string commType = (from f in db.FaxServiceTypes
                                   where f.Service == serviceType
                                   select f.FaxType).Single().ToString();

                //Check if this particular comm exists, reporting back to the page either way
                if (db.PCPCommunications.Any(p => p.DocumentType == commType + "Notification" && p.PLRecord == id))
                {
                    return Content("Faxable, but already has been!");
                }
                else
                {
                    return Content("Faxable!");
                }
            }
            else
            {
                return Content("That's a big negative ghost rider");
            }
        }
    }
}
