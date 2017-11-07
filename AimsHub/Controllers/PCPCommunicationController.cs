using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AimsHub.Models;
using AimsHub.ViewModels;
using System.Data.Entity;
using AimsHub.Security;
using System.Collections.Generic;
using Microsoft.Reporting.WebForms;
using AimsHub.Extensions;

namespace AimsHub.Controllers
{
    public class PCPCommunicationController : Controller
    {
        private PatientLogModel db = new PatientLogModel();
        //private string FAX_FOLDER = "\\AIMSppse05\\C$\\Data\\Fax\\PatientLog\\";
        private string FAX_FOLDER = "C:\\Data\\Fax\\PatientLog\\";

        //private IEnumerable<PCPCommunicationManagePatient> pcpManageQueryGenerator(DateTime fromDate, DateTime toDate, List<string> selectedHosp, List<string> selectedPCP, PCPCommunicationManageViewModel.BasedOn basedOn)
        //{
        //    IEnumerable<PatientLog> query = from p in db.PatientLogs
        //                                    where (p.ServiceType == "DC - STD" || p.ServiceType == "DC - EXT") &&
        //                                    (p.ServiceDate >= fromDate && p.ServiceDate <= toDate)
        //                                    select p;

        //    if (selectedHosp.Any())
        //    {
        //        query = query.Where(h => selectedHosp.Contains(h.Hospital));
        //    }
        //    if (selectedPCP.Any())
        //    {
        //        query = query.Where(s => selectedPCP.Contains(s.ServiceType));
        //    }

        //    List<int> idList = query.Select(p => p.ID).ToList();

        //    IEnumerable<PCPCommunicationManagePatient> ret;

        //}

        private IEnumerable<PatientLog> pcpCommunicationQueryGenerator(DateTime fromDate, DateTime toDate)
        {
            string user = HubSecurity.getLoggedInUserID(); //LINQ doesn't like the function to be in the query
            IEnumerable<PatientLog> query = from d in db.PatientLogs
                                   where (d.ServiceDate >= fromDate && d.ServiceDate <= toDate) && d.Physician == user && (d.PCP_Practice != "No PCP" && d.PCP_Practice != null)
                                   select d;

            //Only select entries that are not registered in PCPCommunication
            query = query.Where(c => !db.PCPCommunications.Select(b => b.PLRecord).Contains(c.ID));

            //Only select valid fax entries
            query = query.Where(c => db.FaxServiceTypes.Select(b => b.Service).Contains(c.ServiceType));

            return query;
        }

        private void sendCommunications(string ids)
        {
            List<int> listofID = new List<int>();
            string[] splitList;
            splitList = ids.Split(new char[] { ',' });  

            foreach (string i in splitList)
            {
                int theID = Convert.ToInt32(i);

                //Query patient record
                PatientLog pat = db.PatientLogs.Find(theID);

                //Determine fax type from table
                string faxType = (from f in db.FaxServiceTypes
                                  where f.Service == pat.ServiceType
                                  select f.FaxType).Single() + "Notice";

                //Break apart PCP name
                string[] splitName;
                splitName = pat.PCP_Practice.Split(new char[] { ',' });
                string pcpName = splitName[1] + " " + splitName[0];
                string firstname = splitName[1].Trim(); //LINQ doesn't like arrays, throws error 
                string lastname = splitName[0].Trim();
                string pcpID = (from u in db.Users where u.FirstName == firstname && u.LastName == lastname select u.UserID).Single();

                //Get practice info of PCP, default to AIMS otherwise
                ReferringPractice prac = new ReferringPractice();
                try
                { 
                prac = (from p in db.ReferringPractices
                       join r in db.RefPracUsers on p.PracID equals r.PracID
                       where r.UserID == pcpID
                       select p).Single();
                }
                catch
                {
                    prac.Fax = "248-354-4807";
                    prac.FaxNotification = true;
                }

                //Create new reportviewer object and set parameters
                ReportViewer report = new ReportViewer();
                report.ProcessingMode = ProcessingMode.Local;
                report.LocalReport.ReportPath = Server.MapPath("~") + "AdDisNotice\\" + faxType + pat.Hospital + ".rdlc";

                //Cannot perform ToShortDateString on pat.ServiceDate because of DateTime? type
                DateTime thed = new DateTime();
                thed = Convert.ToDateTime(pat.ServiceDate);
                DateTime birth = new DateTime();

                //Null values will cause the reportviewer control to error out
                try
                { 
                    birth = Convert.ToDateTime(pat.DOB);
                }
                catch
                {
                    birth = DateTime.Parse("1/1/1900");
                }
                if (pat.PatientName == null)
                {
                    pat.PatientName = "";
                }
                if (pat.Comments == null)
                {
                    pat.Comments = "";
                }
                if (pat.MRN_FIN == null)
                {
                    pat.MRN_FIN = "";
                }

                ReportParameter dateparam = new ReportParameter("Date", thed.ToShortDateString());
                ReportParameter faxparam = new ReportParameter("Fax", prac.Fax);
                ReportParameter faxtoparam = new ReportParameter("FaxTo", pcpName);
                ReportParameter dischargeparam = new ReportParameter("DischargeDate", thed.ToShortDateString());
                ReportParameter patientparam = new ReportParameter("Patient", pat.PatientName);
                ReportParameter treatmentparam = new ReportParameter("Treatment", pat.Comments);
                ReportParameter mrnparam = new ReportParameter("MRN", pat.MRN_FIN);
                ReportParameter physicianparam = new ReportParameter("Physician", HubSecurity.getLoggedInDisplayName());
                ReportParameter dobparam = new ReportParameter("DOB", birth.ToShortDateString());

                ReportParameterCollection theparams = new ReportParameterCollection() {dateparam, faxparam, faxtoparam, dischargeparam, patientparam, treatmentparam, mrnparam, physicianparam, dobparam };
                report.LocalReport.SetParameters(theparams);
                report.LocalReport.Refresh();

                string notificationTypes = "";
                if (prac.FaxNotification == true)
                {
                    notificationTypes += "Fax;";
                }
                if (prac.EmailNotification == true)
                {
                    notificationTypes += "Email;";
                }

                GenerateComms gcomm = new GenerateComms();
                gcomm.SendFax(faxType, report, FAX_FOLDER, prac.Fax, pat.Hospital, HubSecurity.getLoggedInUserID(), pcpID, pcpName, 
                                pat.ID, pat.PatientName, pat.Comments, "", notificationTypes, birth.ToShortDateString(), 
                                prac.PDFPassword, pat.Hospital + faxType, prac.Email, "");
            }
        }

        // GET: PCPCommunication
        public ActionResult Index()
        {
            var fromD = new DateTime();
            var toD = new DateTime();

            if (Session["patientLogFromDate"] != null && Session["patientLogToDate"] != null)
            {
                fromD = Convert.ToDateTime(Session["patientLogFromDate"]);
                toD = Convert.ToDateTime(Session["patientLogToDate"]);
            }
            else
            {
                fromD = DateTime.Now.AddDays(-7);
                toD = DateTime.Now;
            }

            Session["patientLogFromDate"] = fromD;
            Session["patientLogToDate"] = toD;
            
            PCPCommunicationIndexViewModel viewM = new PCPCommunicationIndexViewModel();
            viewM.FromDate = fromD.ToShortDateString();
            viewM.ToDate = toD.ToShortDateString();
            viewM.Patients = pcpCommunicationQueryGenerator(fromD, toD);

            return View(viewM);
        }

        [HttpPost]
        public ActionResult Index(PCPCommunicationIndexViewModel viewM)
        {
            var fromD = new DateTime();
            var toD = new DateTime();
            fromD = getValidDate(viewM.FromDate, true);
            toD = getValidDate(viewM.ToDate, false);

            Session["patientLogFromDate"] = fromD;
            Session["patientLogToDate"] = toD;

            if (Request.Form["sendComms"] != null)
            {
                sendCommunications(viewM.hidIDs);
            }

            viewM.FromDate = fromD.ToShortDateString();
            viewM.ToDate = toD.ToShortDateString();
            viewM.Patients = pcpCommunicationQueryGenerator(fromD, toD);

            return View(viewM);
        }

        // GET: PCPCommunication/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PCPCommunication pCPCommunication = await db.PCPCommunications.FindAsync(id);
            if (pCPCommunication == null)
            {
                return HttpNotFound();
            }
            return View(pCPCommunication);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private DateTime getValidDate(string text, bool isFrom)
        {
            var retDate = new DateTime();
            if (text != null)
            {
                try
                {
                    //var fromD = new DateTime();
                    retDate = DateTime.Parse(text);
                }
                catch
                {
                    if (isFrom) { retDate = DateTime.Now.AddDays(-7); } else { retDate = DateTime.Now; }
                }
            }
            else
            {
                if (isFrom) { retDate = DateTime.Now.AddDays(-7); } else { retDate = DateTime.Now; }
            }

            return retDate;
        }
    }
}
