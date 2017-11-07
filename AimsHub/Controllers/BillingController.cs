using AimsHub.Models;
using AimsHub.Security;
using AimsHub.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AimsHub.DAL;
using System.Net;
using System.Data.Entity;

namespace AimsHub.Controllers
{
    public class BillingController : Controller
    {
        private PatientLogModel db = new PatientLogModel();

        //This function returns results from PatientLog table for all PatientLog Views aside from PatientSort
        private IQueryable<BillingIndexPatient> billingIndexQueryGenerator(DateTime fromDate, DateTime toDate, List<string> selectedPhy, List<string> selectedHosp, List<string> selectedServ, List<string> selectedLastNameFilters,
                                                                bool? notesCompleted, bool? notesCopied, bool? faceSheetEntered, bool? chargePosted, bool? codingCompleted,
                                                                GridFilter.SortDirections direction, string sortColumn, bool purge)
        {
            //Query billing records
            IQueryable<BillingIndexPatient> query = from d in db.PatientLogs
                        join b in db.Billings on d.ID equals b.PLRecord into ds
                        from b in ds.DefaultIfEmpty()
                        where d.ServiceDate >= fromDate &&
                        d.ServiceDate <= toDate && d.ServiceType != "Assigned"
                        select new BillingIndexPatient() { ID = b.ID, PLRecord = d.ID, PatientName = d.PatientName, DOB = d.DOB, MRN_FIN = d.MRN_FIN, Hospital = d.Hospital, ServiceDate = d.ServiceDate, ServiceType = d.ServiceType, Physician = d.Physician,
                            NotesCompleted = b.NotesCompleted, NotesCopied = b.NotesCopied, FaceSheetEntered =  b.FaceSheetEntered, ChargePosted = b.ChargePosted, CodingCompleted = b.CodingCompleted, Purge = b.Purge };

            if (selectedPhy.Any())
            {
                query = query.Where(h => selectedPhy.Contains(h.Physician));
            }
            //Apply hospital filters if any are provided
            if (selectedHosp.Any())
            {
                query = query.Where(h => selectedHosp.Contains(h.Hospital));
            }

            if (selectedServ.Any())
            {
                query = query.Where(s => selectedServ.Contains(s.ServiceType));
            }

            if (selectedLastNameFilters.Any())
            {
                for (int i = 0; i < selectedLastNameFilters.Count() - 1; i++)
                {
                    query = query.Where(f => f.PatientName.StartsWith(selectedLastNameFilters[i]));
                }
            }

            //Filter by notes completed if necessary
            if (notesCompleted == true)
            {
                query = query.Where(n => n.NotesCompleted == true);
            }
            else if (notesCompleted == false)
            {
                query = query.Where(n => n.NotesCompleted == false);
            }
            //Filter by notes copied if necessary
            if (notesCopied == true)
            {
                query = query.Where(n => n.NotesCopied == true);
            }
            else if (notesCopied == false)
            {
                query = query.Where(n => n.NotesCopied == false);
            }
            //Filter by facesheet entere3d if necessary
            if (faceSheetEntered == true)
            {
                query = query.Where(n => n.FaceSheetEntered == true);
            }
            else if (faceSheetEntered == false)
            {
                query = query.Where(n => n.FaceSheetEntered == false);
            }
            //Filter by coding completed if necessary
            if (codingCompleted == true)
            {
                query = query.Where(n => n.CodingCompleted == true);
            }
            else if (codingCompleted == false)
            {
                query = query.Where(n => n.CodingCompleted == false);
            }
            //Filter by charge posted if necessary
            if (chargePosted == true)
            {
                query = query.Where(n => n.ChargePosted == true);
            }
            else if (chargePosted == false)
            {
                query = query.Where(n => n.ChargePosted == false);
            }

            //Apply sorting (default = PatientName ASC)
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
                query = query.OrderBy(sortColumn + " " + "DESC");
            }
            if (purge)
            {
                query = query.Where(n => n.Purge == true);
            }

            List<int> plRecordList = query.Select(p => p.PLRecord).ToList();

            foreach (int id in plRecordList)
            {
                bool exists = (from b in db.Billings
                               where b.PLRecord == id
                               select b.ID).Any();
                if (!exists)
                {
                    DataSubmissions.CreateBillingEntry(db,id);
                }
            }

            //Save preferences
            Dictionary<string, string> filters = new Dictionary<string, string>();
            string pref = null;
            foreach (string phy in selectedPhy)
            {
                pref += phy + ",";
            }
            if (pref != null)
            {
                pref = pref.Substring(0, pref.Length - 1);
            }
            filters.Add("Physician", pref);

            pref = null;
            foreach (string hosp in selectedHosp)
            {
                pref += hosp + ",";
            }
            if (pref != null)
            {
                pref = pref.Substring(0, pref.Length - 1);
            }
            filters.Add("Hospital", pref);

            pref = null;
            foreach (string serv in selectedServ)
            {
                pref += serv + ",";
            }
            if (pref != null)
            {
                pref = pref.Substring(0, pref.Length - 1);
            }
            filters.Add("ServiceType", pref);

            pref = null;
            foreach (string lnf in selectedLastNameFilters)
            {
                pref += lnf + ",";
            }
            if (pref != null)
            {
                pref = pref.Substring(0, pref.Length - 1);
            }
            filters.Add("LastNameFilter", pref);

            if (direction == GridFilter.SortDirections.Ascending)
            {
                filters.Add("SortDirection", "Ascending");
            }
            else
            {
                filters.Add("SortDirection", "Descending");
            }
            filters.Add("SortColumn", sortColumn);

            pref = null;
            if (notesCompleted != null) {
                pref = notesCompleted.ToString();
            }
            filters.Add("NotesCompleted", pref);

            pref = null;
            if (notesCopied != null)
            {
                pref = notesCopied.ToString();
            }
            filters.Add("NotesCopied", pref);

            pref = null;
            if (faceSheetEntered != null)
            {
                pref = faceSheetEntered.ToString();
            }
            filters.Add("FaceSheetEntered", pref);

            pref = null;
            if (chargePosted != null)
            {
                pref = chargePosted.ToString();
            }
            filters.Add("ChargePosted", pref);

            pref = null;
            if (codingCompleted != null)
            {
                pref = codingCompleted.ToString();
            }
            filters.Add("CodingCompleted", pref);

            if (!purge)
            {
                DataSubmissions.SavePreferences(db, "Billing", "BillingIndex", filters);
            }
            else
            {
                DataSubmissions.SavePreferences(db, "Billing", "BillingPurge", filters);
            }
            
            List<int> idList = query.Select(p => p.ID).ToList();
            Session["billingListOfID"] = idList;
            return query;
        }

        private IQueryable<BillingIndexPatient> billingCorrectionQueryGenerator(DateTime fromDate, DateTime toDate, string patient, 
            List<string> selectedHosp, List<string> selectedServ)
        {
            //Query billing records
            IQueryable<BillingIndexPatient> query = from d in db.PatientLogs
                                                    join b in db.Billings on d.ID equals b.PLRecord into ds
                                                    from b in ds.DefaultIfEmpty()
                                                    where d.ServiceDate >= fromDate &&
                                                    d.ServiceDate <= toDate && d.PatientName.Contains(patient)
                                                    select new BillingIndexPatient()
                                                    {
                                                        ID = b.ID,
                                                        PLRecord = d.ID,
                                                        PatientName = d.PatientName,
                                                        DOB = d.DOB,
                                                        MRN_FIN = d.MRN_FIN,
                                                        Hospital = d.Hospital,
                                                        ServiceDate = d.ServiceDate,
                                                        ServiceType = d.ServiceType,
                                                        Physician = d.Physician,
                                                        NotesCompleted = b.NotesCompleted,
                                                        NotesCopied = b.NotesCopied,
                                                        FaceSheetEntered = b.FaceSheetEntered,
                                                        ChargePosted = b.ChargePosted,
                                                        CodingCompleted = b.CodingCompleted,
                                                        Purge = b.Purge
                                                    };

            //Apply hospital filters if any are provided
            if (selectedHosp.Any())
            {
                query = query.Where(h => selectedHosp.Contains(h.Hospital));
            }
            if (selectedServ.Any())
            {
                query = query.Where(s => selectedServ.Contains(s.ServiceType));
            }

            query = query.OrderBy("PatientName");
            return query;
        }


        // GET: Billing
        public ActionResult Index()
        {
            BillingIndexViewModel viewM = new BillingIndexViewModel();
            var fromD = new DateTime();
            var toD = new DateTime();
            List<string> physicians = new List<string>();
            List<string> hospitals = new List<string>();
            List<string> services = new List<string>();
            List<string> lastnamefilters = new List<string>();
            Dictionary<string, string> preferences = new Dictionary<string, string>();
            bool? notesCompleted = null;
            bool? notesCopied = null;
            bool? faceSheetEntered = null;
            bool? codingCompleted = null;
            bool? chargePosted = null;
            string[] splitList;

            if (Session["billingFromDate"] != null && Session["billingToDate"] != null)
            {
                fromD = Convert.ToDateTime(Session["billingFromDate"]);
                toD = Convert.ToDateTime(Session["billingToDate"]);
            }
            else
            {
                fromD = getValidDate("", true);
                toD = getValidDate("", false);
            }
            if (Session["billingSortColumn"] != null)
            {
                viewM.SortColumn = Session["billingSortColumn"].ToString();
                viewM.SortDirection = (GridFilter.SortDirections)Session["billingSortDirection"];
            }

            //Load user preferences into a dictionary
            preferences = DataCollections.LoadPreferences(db, "Billing", "BillingIndex");

            if (preferences.Count > 0)
            {
                if (preferences["Physician"] != null)
                {
                    splitList = preferences["Physician"].Split(new char[] { ',' });
                    physicians = splitList.ToList();
                }

                if (preferences["LastNameFilter"] != null)
                {
                    splitList = preferences["LastNameFilter"].Split(new char[] { ',' });
                    lastnamefilters = splitList.ToList();
                }

                if (preferences["Hospital"] != null)
                {
                    splitList = preferences["Hospital"].Split(new char[] { ',' });
                    hospitals = splitList.ToList();
                }

                if (preferences["ServiceType"] != null)
                {
                    splitList = preferences["ServiceType"].Split(new char[] { ',' });
                    services = splitList.ToList();
                }
                if (preferences["NotesCompleted"] != null)
                {
                    if (preferences["NotesCompleted"] == "true")
                    {
                        notesCompleted = true;
                    }
                    else
                    {
                        notesCompleted = false;
                    }
                }
                if (preferences["NotesCopied"] != null)
                {
                    if (preferences["NotesCopied"] == "true")
                    {
                        notesCopied = true;
                    }
                    else
                    {
                        notesCopied = false;
                    }
                }
                if (preferences["FaceSheetEntered"] != null)
                {
                    if (preferences["FaceSheetEntered"] == "true")
                    {
                        faceSheetEntered = true;
                    }
                    else
                    {
                        faceSheetEntered = false;
                    }
                }
                if (preferences["CodingCompleted"] != null)
                {
                    if (preferences["CodingCompleted"] == "true")
                    {
                        codingCompleted = true;
                    }
                    else
                    {
                        codingCompleted = false;
                    }
                }
                if (preferences["ChargePosted"] != null)
                {
                    if (preferences["ChargePosted"] == "true")
                    {
                        chargePosted = true;
                    }
                    else
                    {
                        chargePosted = false;
                    }
                }
            }

            //if (Session["billingPhysicians"] != null)
            //{
            //    physicians = (List<string>)Session["billingPhysicians"];
            //}
            //if (Session["billingHospitals"] != null)
            //{
            //    hospitals = (List<string>)Session["billingHospitals"];
            //}
            //if (Session["billingServices"] != null)
            //{
            //    services = (List<string>)Session["billingServices"];
            //}
            //if (Session["billingLastNameFilters"] != null)
            //{
            //    lastnamefilters = (List<string>)Session["billingLastNameFilters"];
            //}

            //if (Session["billingNotesCompleted"] != null)
            //{
            //    notesCompleted = (bool)Session["billingNotesCompleted"];
            //}
            //if (Session["billingNotesCopied"] != null)
            //{
            //    notesCopied = (bool)Session["billingNotesCopied"];
            //}
            //if (Session["billingFaceSheetEntered"] != null)
            //{
            //    faceSheetEntered = (bool)Session["billingFaceSheetEntered"];
            //}
            //if (Session["billingCodingCompleted"] != null)
            //{
            //    codingCompleted = (bool)Session["billingCodingCompleted"];
            //}
            //if (Session["billingChargePosted"] != null)
            //{
            //    chargePosted = (bool)Session["billingChargePosted"];
            //}

            Session["billingFromDate"] = fromD;
            Session["billingToDate"] = toD;
            Session["billingSortColumn"] = viewM.SortColumn;
            Session["billingSortDirection"] = viewM.SortDirection;
            //Session["billingPhysicians"] = physicians;
            //Session["billingHospitals"] = hospitals;
            //Session["billingServices"] = services;
            //Session["billingLastNameFilters"] = lastnamefilters;
            //Session["billingNotesCompleted"] = notesCompleted;
            //Session["billingNotesCopied"] = notesCopied;
            //Session["billingFaceSheetEntered"] = faceSheetEntered;
            //Session["billingCodingCompleted"] = codingCompleted;
            //Session["billingChargePosted"] = chargePosted;

            IQueryable<BillingIndexPatient> query = billingIndexQueryGenerator(fromD, toD, physicians, hospitals, services, lastnamefilters, notesCompleted, notesCopied, faceSheetEntered, chargePosted, codingCompleted, viewM.SortDirection, viewM.SortColumn, false);

            viewM.Patients = query.AsEnumerable();
            viewM.FromDate = fromD.ToShortDateString();
            viewM.ToDate = toD.ToShortDateString();
            viewM.PhysicianList = DataCollections.getAIMSPhy(db);
            viewM.HospitalList = DataCollections.getHospital(db);
            viewM.ServiceList = DataCollections.getServiceType(db);
            viewM.LastNameFilterList = DataCollections.getLastNameFilter();
            viewM.YesNo = DataCollections.getYesNo();
            viewM.SelectedPhysicians = physicians;
            viewM.SelectedHospitals = hospitals;
            viewM.SelectedServices = services;
            viewM.SelectedLastNameFilters = lastnamefilters;
            viewM.SelectedNotesCompleted = notesCompleted;
            viewM.SelectedNotesCopied = notesCopied;
            viewM.SelectedFaceSheetEntered = faceSheetEntered;
            viewM.SelectedCodingCompleted = codingCompleted;
            viewM.SelectedChargePosted = chargePosted;
            return View("Index", viewM);
        }

        // GET: Billing
        [HttpPost]
        public ActionResult Index(BillingIndexViewModel viewM)
        {
            BillingIndexViewModel returnM = new BillingIndexViewModel();
            var fromD = new DateTime();
            var toD = new DateTime();
            List<string> physicians = new List<string>();
            List<string> hospitals = new List<string>();
            List<string> services = new List<string>();
            List<string> lastnamefilters = new List<string>();
            bool? notesCompleted = null;
            bool? notesCopied = null;
            bool? faceSheetEntered = null;
            bool? codingCompleted = null;
            bool? chargePosted = null;
            string[] splitList;

            //Manages physician input and builds a List<string> to be used in the query
            if (viewM.hidPhysicians != null && viewM.hidPhysicians != "")
            {
                splitList = viewM.hidPhysicians.Split(new char[] { ',' });
                physicians = splitList.ToList<string>();
            }
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
            if (viewM.hidLastNameFilters != null && viewM.hidLastNameFilters != "")
            {
                splitList = viewM.hidLastNameFilters.Split(new char[] { ',' });
                lastnamefilters = splitList.ToList<string>();
            }
            if (viewM.hidNotesCompleted != null)
            {
                switch (viewM.hidNotesCompleted)
                {
                    case "true":
                        notesCompleted = true;
                        break;
                    case "false":
                        notesCompleted = false;
                        break;
                }
            }
            if (viewM.hidNotesCopied != null)
            {
                switch (viewM.hidNotesCopied)
                {
                    case "true":
                        notesCopied = true;
                        break;
                    case "false":
                        notesCopied = false;
                        break;
                }
            }
            if (viewM.hidFaceSheetEntered != null)
            {
                switch (viewM.hidFaceSheetEntered)
                {
                    case "true":
                        faceSheetEntered = true;
                        break;
                    case "false":
                        faceSheetEntered = false;
                        break;
                }
            }
            if (viewM.hidCodingCompleted != null)
            {
                switch (viewM.hidCodingCompleted)
                {
                    case "true":
                        codingCompleted = true;
                        break;
                    case "false":
                        codingCompleted = false;
                        break;
                }
            }
            if (viewM.hidChargePosted != null)
            {
                switch (viewM.hidChargePosted)
                {
                    case "true":
                        chargePosted = true;
                        break;
                    case "false":
                        chargePosted = false;
                        break;
                }
            }


            fromD = getValidDate(viewM.FromDate, true);
            toD = getValidDate(viewM.ToDate, false);

            IQueryable<BillingIndexPatient> query = billingIndexQueryGenerator(fromD, toD, physicians, hospitals, services, lastnamefilters, notesCompleted, notesCopied, faceSheetEntered, chargePosted, codingCompleted, viewM.SortDirection, viewM.SortColumn, false);

            Session["billingFromDate"] = fromD;
            Session["billingToDate"] = toD;
            Session["billingSortColumn"] = viewM.SortColumn;
            Session["billingSortDirection"] = viewM.SortDirection;
            //Session["billingPhysicians"] = physicians;
            //Session["billingHospitals"] = hospitals;
            //Session["billingServices"] = services;
            //Session["billingLastNameFilters"] = lastnamefilters;
            //Session["billingNotesCompleted"] = notesCompleted;
            //Session["billingNotesCopied"] = notesCopied;
            //Session["billingFaceSheetEntered"] = faceSheetEntered;
            //Session["billingCodingCompleted"] = codingCompleted;
            //Session["billingChargePosted"] = chargePosted;

            returnM.Patients = query.AsEnumerable();
            returnM.FromDate = fromD.ToShortDateString();
            returnM.ToDate = toD.ToShortDateString();
            returnM.PhysicianList = DataCollections.getAIMSPhy(db);
            returnM.HospitalList = DataCollections.getHospital(db);
            returnM.ServiceList = DataCollections.getServiceType(db);
            returnM.LastNameFilterList = DataCollections.getLastNameFilter();
            returnM.YesNo = DataCollections.getYesNo();
            returnM.SelectedPhysicians = physicians;
            returnM.SelectedHospitals = hospitals;
            returnM.SelectedServices = services;
            returnM.SelectedLastNameFilters = lastnamefilters;
            returnM.SelectedNotesCompleted = notesCompleted;
            returnM.SelectedNotesCopied = notesCopied;
            returnM.SelectedFaceSheetEntered = faceSheetEntered;
            returnM.SelectedCodingCompleted = codingCompleted;
            returnM.SelectedChargePosted = chargePosted;
            returnM.SortColumn = viewM.SortColumn;
            returnM.SortDirection = viewM.SortDirection;
            return View("Index", returnM);
        }

        // GET: PatientLog/Edit/5
        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BillingDetailsViewModel viewM = new BillingDetailsViewModel();
            viewM.BillingRecord = db.Billings.Find(id);
            if (viewM.BillingRecord == null)
            {
                return HttpNotFound();
            }
            viewM.Patient = db.PatientLogs.Find(viewM.BillingRecord.PLRecord);
            if (viewM.Patient == null)
            {
                return HttpNotFound();
            }

            List<int> idList = (List<int>)Session["billingListOfID"];

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

            viewM.CPTCodesList = DataCollections.getCPTCodesList(db);
            viewM.MODCodesList = DataCollections.getMODCodesList(db);
            viewM.POSCodesList = DataCollections.getPOSCodesList(db);
            viewM.DXCodesList = DataCollections.getDXCodesList(db);

            return View(viewM);
        }

        public ActionResult Correction()
        {
            string patient = "";
            List<string> hospitals = new List<string>();
            List<string> services = new List<string>();
            BillingCorrectionViewModel viewM = new BillingCorrectionViewModel();

            if (Session["billingCorrectionServiceDate"] != null)
            {
                viewM.SearchServiceDate = Session["billingCorrectionServiceDate"].ToString();
            }
            else
            {
                viewM.SearchServiceDate = DateTime.Now.ToShortDateString();
                Session["billingCorrectionServiceDate"] = viewM.SearchServiceDate;
            }
            if (Session["billingCorrectionPatient"] != null)
            {
                patient = Session["billingCorrectionPatient"].ToString();
            }
            if (Session["billingCorrectionHospitals"] != null)
            {
                hospitals = (List<string>)Session["billingCorrectionHospitals"];
            }
            if (Session["billingCorrectionServices"] != null)
            {
                services = (List<string>)Session["billingCorrectionServices"];
            }

            viewM.Patients = new List<BillingIndexPatient>().AsEnumerable();
            viewM.GenderList = DataCollections.getGender(db);
            viewM.PhysicianList = DataCollections.getAIMSPhy(db);
            viewM.PCPList = DataCollections.getPCP(db, "BRO");
            viewM.HospitalList = DataCollections.getHospital(db);
            viewM.ServiceList = DataCollections.getServiceType(db);
            viewM.SelectedHospitals = hospitals;
            viewM.SelectedServices = services;
            viewM.SearchPatientName = patient;

            return View(viewM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Correction(BillingCorrectionViewModel viewM)
        {
            DateTime fromD = getValidDate(viewM.SearchServiceDate, true);
            DateTime toD = getValidDate(viewM.SearchServiceDate, false);
            string patient = viewM.SearchPatientName;
            List<string> hospitals = new List<string>();
            List<string> services = new List<string>();
            string[] splitList;

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

            BillingCorrectionViewModel returnM = new BillingCorrectionViewModel();

            returnM.Patients = billingCorrectionQueryGenerator(fromD, toD, patient, hospitals, services);
            returnM.GenderList = DataCollections.getGender(db);
            returnM.PhysicianList = DataCollections.getAIMSPhy(db);
            returnM.PCPList = DataCollections.getPCP(db, "BRO");
            returnM.HospitalList = DataCollections.getHospital(db);
            returnM.ServiceList = DataCollections.getServiceType(db);
            returnM.SearchPatientName = patient;
            returnM.SearchServiceDate = fromD.ToShortDateString();
            returnM.SelectedHospitals = hospitals;
            returnM.SelectedServices = services;

            return View(returnM);
        }

        //Returns PatientLog object via JSON
        [HttpGet]
        public ActionResult jsonGetPatient(string patientID)
        {
            int patID = Convert.ToInt32(patientID);
            PatientLog query = (from p in db.PatientLogs
                                where p.ID == patID
                                select p).Single();

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        //Submits a single patient to the server for modification via JSON
        [HttpPost]
        public ActionResult jsonSubmitPatient(PatientLog patientLog)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    PatientLog sourcePatient;
                    sourcePatient = (from p in db.PatientLogs
                                     where p.ID == patientLog.ID
                                     select p).Single();

                    sourcePatient.Notes = patientLog.Notes;
                    sourcePatient.FaceSheet = patientLog.FaceSheet;
                    db.Entry(sourcePatient).State = EntityState.Modified;
                    db.SaveChanges();
                    return Content("Patient information has been saved successfully!");
                }
                else
                {
                    return Content("Model state is not valid.");
                }
            }
            catch (Exception ex) {
                return Content(ex.Message);
            }

        }

        //Submits a single record to the server for modification via JSON
        [HttpPost]
        public ActionResult jsonSubmitRecord(Billing billingRecord)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(billingRecord).State = EntityState.Modified;
                    db.SaveChanges();
                    return Content("Billing record has been saved successfully!");
                }
                else
                {
                    return Content("Something went wrong when saving this record. Please notify IT");
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        //[HttpGet]
        //public ActionResult jsonGetMissingNotesLog(string id)
        //{

        //}

        //[HttpPost]
        //public ActionResult jsonSendEmail(string id, string comment)
        //{
        //    PatientLog pat = new PatientLog();
        //    pat = db.PatientLogs.Find(id);

        //    try
        //    {
        //        string strSub = "Missing Note for Patient: " + pat.PatientName;
        //        string strComments = comment;
        //    }
        //    catch
        //    {

        //    }

        //    MissingNotesLog log = new MissingNotesLog();
        //    log.PLRecord = Convert.ToInt32(id);
        //    log.Comment = comment;
        //    log.PhysicianEmail = physician + "@aims.us.com";
        //    log.Sender = HubSecurity.getLoggedInUserID();
        //    log.SentTime = DateTime.Now;
        //}
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
                    if (isFrom) { retDate = DateTime.Now + new TimeSpan(0, 0, 0); } else { retDate = DateTime.Now + new TimeSpan(23, 59, 59); }
                }
            }
            else
            {
                if (isFrom) { retDate = DateTime.Now + new TimeSpan(0, 0, 0); } else { retDate = DateTime.Now + new TimeSpan(23, 59, 59); }
            }

            return retDate;
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