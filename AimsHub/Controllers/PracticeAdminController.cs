using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AimsHub.ViewModels;
using AimsHub.Models;
using AimsHub.DAL;
using System.Data;
using System.Text;
using AimsHub.Security;
using AimsHub.Extensions;

namespace AimsHub.Controllers
{
    public class PracticeAdminController : Controller
    {
        private PatientLogModel db = new PatientLogModel();
        private const int intDayFallback = -5;

        private IEnumerable<PatientLog> pracAdminQueryGenerator(DateTime fallDate)
        {
            List<string> pcps = new List<string>();
            pcps = DataCollections.getPracticeAdminPCPs(db, AimsHub.Security.HubSecurity.getLoggedInUserID());

            IEnumerable<PatientLog> patients = from p in db.PatientLogs
                                               where p.ServiceDate >= fallDate && 
                                               pcps.Contains(p.PCP_Practice)
                                               select p;

            return patients;
        }

        private IEnumerable<PatientLog> pracAdminSearchQueryGenerator(DateTime fromDate, DateTime toDate, string patient)
        {
            List<string> pcps = new List<string>();
            pcps = DataCollections.getPracticeAdminPCPs(db, AimsHub.Security.HubSecurity.getLoggedInUserID());

            IEnumerable<PatientLog> patients = from p in db.PatientLogs
                                               where (p.ServiceDate >= fromDate && p.ServiceDate <= toDate) && 
                                               pcps.Contains(p.PCP_Practice)
                                               select p;

            if (patient != null && patient != "")
            {
                patients = patients.Where(p => p.PatientName.Contains(patient));
            }

            return patients;
        }

        // GET: PracticeAdmin
        public ActionResult Index()
        {
            PracticeAdminIndexViewModel viewM = new PracticeAdminIndexViewModel();

            if (Session["PracAdminFallback"] != null)
            {
                viewM.dateFallback = Convert.ToDateTime(Session["PracAdminFallback"]);
            }
            else
            {
                viewM.dateFallback = DateTime.Now.AddDays(intDayFallback);
            }

            viewM.Patients = pracAdminQueryGenerator(viewM.dateFallback);

            return View(viewM);
        }

        // GET: PracticeAdmin
        [HttpPost]
        public ActionResult Index(PracticeAdminIndexViewModel viewM)
        {
            PracticeAdminIndexViewModel returnM = new PracticeAdminIndexViewModel();

            if (viewM.dateFallback != null)
            {
                returnM.dateFallback = viewM.dateFallback;
                Session["PracAdminFallback"] = viewM.dateFallback;
            }
            else
            {
                returnM.dateFallback = DateTime.Now.AddDays(intDayFallback);
            }

            returnM.Patients = pracAdminQueryGenerator(returnM.dateFallback);

            return View(returnM);
        }

        public ActionResult Search()
        {
            var fromD = new DateTime();
            var toD = new DateTime();
            string patient;

            if (Session["practiceAdminFromDate"] != null && Session["practiceAdminToDate"] != null)
            {
                fromD = Convert.ToDateTime(Session["practiceAdminFromDate"]);
                toD = Convert.ToDateTime(Session["practiceAdminToDate"]);
            }
            else
            {
                fromD = getValidDate("", true);
                toD = getValidDate("", false);
            }

            if (Session["practiceAdminPatient"] != null)
            {
                patient = Session["practiceAdminPatient"].ToString();
            }
            else {
                patient = "";
            }

            PracticeAdminSearchViewModel viewM = new PracticeAdminSearchViewModel();
            viewM.FromDate = fromD.ToShortDateString();
            viewM.ToDate = toD.ToShortDateString();
            viewM.PatientSearch = patient;
            viewM.Patients = pracAdminSearchQueryGenerator(fromD, toD, patient);

            return View(viewM);
        
        }

        [HttpPost]
        public ActionResult Search(PracticeAdminSearchViewModel viewM)
        {
            var fromD = new DateTime();
            var toD = new DateTime();
            string patient = "";

            fromD = getValidDate(viewM.FromDate, true);
            toD = getValidDate(viewM.ToDate, false);

            if (viewM.PatientSearch != null && viewM.PatientSearch != "")
            {
                patient = viewM.PatientSearch;
            }

            PracticeAdminSearchViewModel returnM = new PracticeAdminSearchViewModel();

            returnM.FromDate = fromD.ToShortDateString();
            returnM.ToDate = toD.ToShortDateString();
            returnM.PatientSearch = patient;
            returnM.Patients = pracAdminSearchQueryGenerator(fromD, toD, patient);

            return View(returnM);
        }
        public ActionResult EditPreferences()
        {
            PracticeAdminEditPreferencesViewModel viewM = new PracticeAdminEditPreferencesViewModel();
            viewM.practices = DataCollections.getPractices(db, HubSecurity.getLoggedInUserID());
            if (viewM.practices.Count() > 1)
            {
                viewM.selectedPrac = DataCollections.getPractice(db, viewM.practices.Skip(1).First().Value);
            }
            else
            {
                viewM.selectedPrac = DataCollections.getPractice(db, viewM.practices.First().Value);
            }
            viewM.hidPrac = viewM.selectedPrac.PracID.ToString();
            viewM.specialties = DataCollections.getSpecialties(db, viewM.selectedPrac.PracID);
            return View(viewM);
        }
        [HttpPost]
        public ActionResult EditPreferences(PracticeAdminEditPreferencesViewModel viewM)
        {
            PracticeAdminEditPreferencesViewModel returnM = new PracticeAdminEditPreferencesViewModel();
            ReferringPractice oldPrac;
            returnM.practices = DataCollections.getPractices(db, HubSecurity.getLoggedInUserID());
            int id;
            if (viewM.UpdatePracticeInformation != null)
            {
                if (viewM.hidAll == "true")
                {
                    foreach (SelectListItem prac in returnM.practices)
                    {
                        if (prac.Value == "ALL")
                        {
                            continue;
                        }
                        id = Convert.ToInt32(prac.Value);
                        oldPrac = (from r in db.ReferringPractices where r.PracID == id select r).Single();
                        oldPrac.Address1 = viewM.selectedPrac.Address1;
                        oldPrac.Address2 = viewM.selectedPrac.Address2;
                        oldPrac.Address3 = viewM.selectedPrac.Address3;
                        oldPrac.City = viewM.selectedPrac.City;
                        oldPrac.State = viewM.selectedPrac.State;
                        oldPrac.Zip = viewM.selectedPrac.Zip;
                        oldPrac.Phone = viewM.selectedPrac.Phone;
                        oldPrac.Fax = viewM.selectedPrac.Fax;
                        oldPrac.OfficeManager = viewM.selectedPrac.OfficeManager;
                        oldPrac.Other = viewM.selectedPrac.Other;
                        oldPrac.PDFPassword = viewM.selectedPrac.PDFPassword;
                        DataSubmissions.SavePractice(db, oldPrac);
                    }
                }
                else
                {
                    oldPrac = (from r in db.ReferringPractices where r.PracID == viewM.selectedPrac.PracID select r).Single();
                    oldPrac.Address1 = viewM.selectedPrac.Address1;
                    oldPrac.Address2 = viewM.selectedPrac.Address2;
                    oldPrac.Address3 = viewM.selectedPrac.Address3;
                    oldPrac.City = viewM.selectedPrac.City;
                    oldPrac.State = viewM.selectedPrac.State;
                    oldPrac.Zip = viewM.selectedPrac.Zip;
                    oldPrac.Phone = viewM.selectedPrac.Phone;
                    oldPrac.Fax = viewM.selectedPrac.Fax;
                    oldPrac.OfficeManager = viewM.selectedPrac.OfficeManager;
                    oldPrac.Other = viewM.selectedPrac.Other;
                    oldPrac.PDFPassword = viewM.selectedPrac.PDFPassword;
                    DataSubmissions.SavePractice(db, oldPrac);
                }
                
            }
            if (viewM.UpdateCommunicationMethod != null)
            {
                if (viewM.hidAll == "true")
                {
                    foreach (SelectListItem prac in returnM.practices)
                    {
                        if (prac.Value == "ALL")
                        {
                            continue;
                        }
                        id = Convert.ToInt32(prac.Value);
                        oldPrac = (from r in db.ReferringPractices where r.PracID == id select r).Single();
                        oldPrac.EmailNotification = viewM.selectedPrac.EmailNotification;
                        oldPrac.FaxNotification = viewM.selectedPrac.FaxNotification;
                        DataSubmissions.SavePractice(db, oldPrac);
                    }
                }
                else
                {
                    oldPrac = (from r in db.ReferringPractices where r.PracID == viewM.selectedPrac.PracID select r).Single();
                    oldPrac.EmailNotification = viewM.selectedPrac.EmailNotification;
                    oldPrac.FaxNotification = viewM.selectedPrac.FaxNotification;
                    DataSubmissions.SavePractice(db, oldPrac);
                }               
            }

            returnM.practices = DataCollections.getPractices(db, HubSecurity.getLoggedInUserID());
            returnM.selectedPrac = DataCollections.getPractice(db, viewM.hidPrac);
            returnM.specialties = DataCollections.getSpecialties(db, viewM.selectedPrac.PracID);
            returnM.hidPrac = viewM.hidPrac;
            returnM.tabReturn = viewM.tabReturn;
            returnM.hidAll = viewM.hidAll;

            return View(returnM);
        }
        public ActionResult Support()
        {
            PracticeAdminSupportViewModel viewM = new PracticeAdminSupportViewModel();

            List<SelectListItem> reasons = new List<SelectListItem>();
            reasons.Add(new SelectListItem { Text = "", Value = "" });
            reasons.Add(new SelectListItem { Text = "Data is not right", Value = "Data is not right" });
            reasons.Add(new SelectListItem { Text = "Help/Question", Value = "Help/Question" });
            reasons.Add(new SelectListItem { Text = "Problem", Value = "Problem" });     
            reasons.Add(new SelectListItem { Text = "Other", Value = "Other" });
            viewM.Reasons = new SelectList(reasons, "Value", "Text");

            return View(viewM);
        }
        [HttpPost]
        public ActionResult Support(PracticeAdminSupportViewModel viewM)
        {
            PracticeAdminSupportViewModel returnM = new PracticeAdminSupportViewModel();
            returnM.Reasons = viewM.Reasons;
            returnM.Body = viewM.Body;
            returnM.Reason = viewM.Reason;
            returnM.Subject = viewM.Subject;
            List<SelectListItem> reasons = new List<SelectListItem>();
            reasons.Add(new SelectListItem { Text = "", Value = "" });
            reasons.Add(new SelectListItem { Text = "Data is not right", Value = "Data is not right" });
            reasons.Add(new SelectListItem { Text = "Help/Question", Value = "Help/Question" });
            reasons.Add(new SelectListItem { Text = "Problem", Value = "Problem" });
            reasons.Add(new SelectListItem { Text = "Other", Value = "Other" });
            returnM.Reasons = new SelectList(reasons, "Value", "Text");
            if (viewM.ReturnEmail == "" || viewM.ReturnEmail == null)
            {
                returnM.ErrorMessage = "You must provide an email that we can contact you with.";
                return View(returnM);
            }
            else
            {
                GenerateComms comm = new GenerateComms();
                comm.SendMail(returnM.ReturnEmail, "cwest@aims.us.com", returnM.Subject, returnM.Body, "", "fax", "aimsfx2345", null);
                returnM.MessageSent = true;
                returnM.ErrorMessage = null;
                return View(returnM);
            }          
        }

        [HttpPost]
        public JsonResult jsonSubmitPracticeInformation(ReferringPractice prac)
        {
            string result = "";
            ReferringPractice newPrac = DataCollections.getPractice(db, prac.PracID);

            newPrac.Address1 = prac.Address1;
            newPrac.Address2 = prac.Address2;
            newPrac.Address3 = prac.Address3;
            newPrac.City = prac.City;
            newPrac.State = prac.State;
            newPrac.Zip = prac.Zip;
            newPrac.Phone = prac.Phone;
            newPrac.Fax = prac.Fax;
            newPrac.Email = prac.Email;
            newPrac.OfficeManager = prac.OfficeManager;
            newPrac.Other = prac.Other;
            newPrac.PDFPassword = prac.PDFPassword;

            DataSubmissions.SavePractice(db, newPrac);

            result = "Practice Information Saved";
            return Json(result);
        }
        [HttpPost]
        public JsonResult jsonSubmitCommunicationMethod(ReferringPractice prac)
        {
            string result = "";
            ReferringPractice newPrac = DataCollections.getPractice(db, prac.PracID);

            newPrac.EmailNotification = prac.EmailNotification;
            newPrac.FaxNotification = prac.FaxNotification;            

            DataSubmissions.SavePractice(db, newPrac);

            result = "Communication Methods Saved";
            return Json(result);
        }
        [HttpPost]
        public JsonResult jsonSubmitSpecialties(List<RefPracSpecialty> specialties)
        {
            DataSubmissions.SaveSpecialties(db, specialties);

            return Json("Specialties Saved");
        }
        [HttpPost]
        public JsonResult jsonSubmitSpecialtiesAll(List<RefPracSpecialty> specialties)
        {
            DataSubmissions.SaveSpecialties(db, specialties, HubSecurity.getLoggedInUserID());

            return Json("Specialties Saved");
        }

        //Returns JSON list of PCPs that match the specified site
        public ActionResult jsonDocuments(int id)
        {
            var query = from c in db.PCPCommunications
                        join p in db.PatientLogs on c.PLRecord equals p.ID into js
                        where c.PLRecord == id
                        select c;

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn { DataType = System.Type.GetType("System.String"), ColumnName = "ServiceDate" });
            dt.Columns.Add(new DataColumn { DataType = System.Type.GetType("System.String"), ColumnName = "DocumentType" });
            dt.Columns.Add(new DataColumn { DataType = System.Type.GetType("System.String"), ColumnName = "Document" });

            foreach (var item in query)
            {
                var row = dt.NewRow();
                row["ServiceDate"] = item.ScheduledTime;
                row["DocumentType"] = item.DocumentType;
                row["Document"] = item.DocumentPath + item.DocumentName;
                dt.Rows.Add(row);
            }
            return Json(DataTableToJsonObj(dt), JsonRequestBehavior.AllowGet);
        }

        public string DataTableToJsonObj(DataTable dt)
        {
            DataSet ds = new DataSet();
            ds.Merge(dt);
            StringBuilder JsonString = new StringBuilder();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                JsonString.Append("[");
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    JsonString.Append("{");
                    for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                    {
                        if (j < ds.Tables[0].Columns.Count - 1)
                        {
                            JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + ds.Tables[0].Rows[i][j].ToString() + "\",");
                        }
                        else if (j == ds.Tables[0].Columns.Count - 1)
                        {
                            JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + ds.Tables[0].Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == ds.Tables[0].Rows.Count - 1)
                    {
                        JsonString.Append("}");
                    }
                    else
                    {
                        JsonString.Append("},");
                    }
                }
                JsonString.Append("]");
                return JsonString.ToString();
            }
            else
            {
                return null;
            }
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
    }
}