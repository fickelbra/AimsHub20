using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AimsHub.Models;
using AimsHub.Security;
using AimsHub.ViewModels;
using System.Linq.Dynamic;
using OfficeOpenXml;
using System.IO;
using System.Data;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Data.Entity;
using AimsHub.DAL;
using AimsHub.Extensions;

namespace AimsHub.Controllers
{
    public class PatientAssignmentController : Controller
    {
        private PatientLogModel db = new PatientLogModel();
        private const int intDayFallback = -1;
        
        private IQueryable<PatientLog> patientAssignmentQueryGenerator(DateTime fromDate, DateTime toDate, List<string> selectedPhy, string selectedHosp, List<string> selectedServ,
                                                                GridFilter.SortDirections direction, string sortColumn)
        {
            IQueryable<PatientLog> query;
            if (sortColumn == null)
            {
                //Query PatientLog by physicians and date/time range
                query = from d in db.PatientLogs
                        where d.ServiceDate >= fromDate &&
                        d.ServiceDate <= toDate
                        && selectedHosp == d.Hospital
                        orderby d.PatientName, d.ServiceDate
                        select d;
            }
            else
            {
                query = from d in db.PatientLogs
                        where d.ServiceDate >= fromDate &&
                        d.ServiceDate <= toDate
                        && selectedHosp == d.Hospital
                        select d;

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
            }

            //Apply hospital filters if any are provided
            if (selectedPhy.Any())
            {
                query = query.Where(h => selectedPhy.Contains(h.Physician));
            }

            if (selectedServ.Any())
            {
                query = query.Where(s => selectedServ.Contains(s.ServiceType));
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

            filters.Add("Hospital", selectedHosp);

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

            //if (direction == GridFilter.SortDirections.Ascending)
            //{
            //    filters.Add("SortDirection", "Ascending");
            //}
            //else
            //{
            //    filters.Add("SortDirection", "Descending");
            //}

            DataSubmissions.SavePreferences(db, "PatientAssignment", "PatientAssignmentIndex", filters);

            List<int> idList = query.Select(p => p.ID).ToList();
            Session["patientAssignmentListOfID"] = idList;
            return query;
        }


        //GET: PatientLog
        public ActionResult Index()
        {
            var viewM = new PatientAssignmentIndexViewModel();
            var fromD = new DateTime();
            var toD = new DateTime();
            Dictionary<string, string> preferences = new Dictionary<string, string>();
            List<string> physicians = new List<string>();
            string hospitals = "BRO";
            List<string> services = new List<string>();
            string[] splitList;

            if (Session["patientAssignmentFromDate"] != null && Session["patientAssignmentToDate"] != null)
            {
                fromD = Convert.ToDateTime(Session["patientAssignmentFromDate"]);
                toD = Convert.ToDateTime(Session["patientAssignmentToDate"]);
            }
            else
            {
                fromD = DateTime.Now.AddDays(intDayFallback);
                fromD = fromD.Date + new TimeSpan(0, 0, 0);
                toD = DateTime.Now;
                toD = toD.Date + new TimeSpan(23, 59, 59);
            }
            if (Session["patientAssignmentSortColumn"] != null)
            {
                //sortColumn = Session["patientLogSortColumn"].ToString();
                //sortDirection = (GridFilter.FilterTypes)Session["patientLogSortDirection"];
                viewM.SortColumn = Session["patientAssignmentSortColumn"].ToString();
                viewM.SortDirection = (GridFilter.SortDirections)Session["patientAssignmentSortDirection"];
            }

            //Load user preferences into a dictionary
            preferences = DataCollections.LoadPreferences(db, "PatientAssignment", "PatientAssignmentIndex");

            if (preferences.Count > 0)
            {
                if (preferences["Physician"] != null)
                {
                    splitList = preferences["Physician"].Split(new char[] { ',' });
                    physicians = splitList.ToList();
                }

                if (preferences["Hospital"] != null)
                {
                    hospitals = preferences["Hospital"];
                }

                if (preferences["ServiceType"] != null)
                {
                    splitList = preferences["ServiceType"].Split(new char[] { ',' });
                    services = splitList.ToList();
                }
            }
            //if (Session["patientAssignmentPhysicians"] != null)
            //{
            //    physicians = (List<string>)Session["patientAssignmentPhysicians"];
            //}
            //if (Session["patientAssignmentHospitals"] != null)
            //{
            //    string hosp = Session["patientAssignmentHospitals"].ToString();
            //    //tempList.Add(hosp);
            //    hospitals = hosp;
            //}
            //else
            //{
            //    //List<string> tempList = new List<string>();
            //    //string usr = HubSecurity.getLoggedInUserID();
            //    ////string hosp = (from u in db.UserDetails
            //    ////              where u.UserID == usr && u.UserType == "AIMSPhy"
            //    ////              select u.DefaultHospital).Single().ToString();
            //    //string hosp = "HFM"; //DEBUG
            //    //tempList.Add(hosp);
            //    hospitals = "BRO";
            //}
            //if (Session["patientAssignmentServices"] != null)
            //{
            //    services = (List<string>)Session["patientAssignmentServices"];
            //}

            


            Session["patientAssignmentFromDate"] = fromD;
            Session["patientAssignmentToDate"] = toD;
            Session["patientAssignmentSortColumn"] = viewM.SortColumn;
            Session["patientAssignmentSortDirection"] = viewM.SortDirection;
            //Session["patientAssignmentPhysicians"] = physicians;
            //Session["patientAssignmentHospitals"] = hospitals;
            //Session["patientAssignmentServices"] = services;

            IQueryable<PatientLog> query = patientAssignmentQueryGenerator(fromD, toD, physicians, hospitals, services, viewM.SortDirection, viewM.SortColumn);

            viewM.Patients = query.AsEnumerable<PatientLog>();
            //viewM.ToLookupTally = viewM.Patients.ToLookup(p => p.Physician, p => p.ServiceType);
            viewM.ToLookupTally = query.ToLookup(p => p.Physician, p => p.ServiceType).ToDictionary(p => p.Key, p => p.ToArray());
            if (viewM.ToLookupTally.Count == 0)
            {
                viewM.ToLookupTally.Add("No data", new string[] { "" });
            }
            viewM.FromDate = fromD.ToShortDateString();
            viewM.ToDate = toD.ToShortDateString();
            viewM.PhysicianList = DataCollections.getAIMSPhy(db);
            viewM.HospitalList = DataCollections.getHospital(db);
            viewM.ServiceList = DataCollections.getServiceType(db);
            viewM.ImportColumns = DataCollections.getImportColumns(db);
            viewM.SelectedPhysicians = physicians;
            viewM.SelectedHospitals = hospitals;
            viewM.SelectedServices = services;

            return View("Index", viewM);
        }

        [HttpPost]
        public ActionResult Index(PatientAssignmentIndexViewModel viewM)
        {
            //Value will always be null unless CopyButton caused post
            if (viewM.CopyButton != null)
            {
                DataSubmissions.CopyPatients(db, viewM.hidSelectedIDs);
            }
            //Value will always be null unless DeleteButton caused post
            if (viewM.DeleteButton != null)
            {
                DataSubmissions.DeletePatients(db, viewM.hidSelectedIDs);
            }

            var fromD = new DateTime();
            var toD = new DateTime();
            List<string> physicians = new List<string>();
            //List<string> hospitals = new List<string>();
            string hospitals = viewM.SelectedHospitals;
            List<string> services = new List<string>();
            string[] splitList;

            //Manages physician input and builds a List<string> to be used in the query
            if (viewM.hidPhysicians != null && viewM.hidPhysicians != "")
            {
                splitList = viewM.hidPhysicians.Split(new char[] { ',' });
                physicians = splitList.ToList<string>();
            }

            //Manages physician input and builds a List<string> to be used in the query
            //if (viewM.hidHospitals != null && viewM.hidHospitals != "")
            //{
            //    //splitList = viewM.hidHospitals.Split(new char[] { ',' });
            //    //hospitals = splitList.ToList<string>();
            //    hospitals = viewM.hidHospitals;
            //}
            if (viewM.hidServices != null && viewM.hidServices != "")
            {
                splitList = viewM.hidServices.Split(new char[] { ',' });
                services = splitList.ToList<string>();
            }


            fromD = getValidDate(viewM.FromDate, true);
            toD = getValidDate(viewM.ToDate, false);

            PatientAssignmentIndexViewModel returnM = new PatientAssignmentIndexViewModel();

            returnM.FromDate = fromD.ToShortDateString();
            returnM.ToDate = toD.ToShortDateString();
            returnM.PhysicianList = DataCollections.getAIMSPhy(db);
            returnM.HospitalList = DataCollections.getHospital(db);
            returnM.ServiceList = DataCollections.getServiceType(db);
            returnM.ImportColumns = DataCollections.getImportColumns(db);
            returnM.SelectedPhysicians = physicians;
            returnM.SelectedHospitals = hospitals;
            returnM.SelectedServices = services;
            returnM.SortColumn = viewM.SortColumn;
            returnM.SortDirection = viewM.SortDirection;

            Session["patientAssignmentFromDate"] = fromD;
            Session["patientAssignmentToDate"] = toD;
            Session["patientAssignmentSortColumn"] = returnM.SortColumn;
            Session["patientAssignmentSortDirection"] = returnM.SortDirection;
            //Session["patientAssignmentPhysicians"] = physicians;
            //Session["patientAssignmentHospitals"] = hospitals;
            //Session["patientAssignmentServices"] = services;

            IQueryable<PatientLog> query = patientAssignmentQueryGenerator(fromD, toD, physicians, hospitals, services, returnM.SortDirection, returnM.SortColumn);

            returnM.Patients = query.AsEnumerable<PatientLog>();
            returnM.ToLookupTally = query.ToLookup(p => p.Physician, p => p.ServiceType).ToDictionary(p => p.Key, p => p.ToArray());
            if (returnM.ToLookupTally.Count == 0)
            {
                returnM.ToLookupTally.Add("No data", new string[] { "" });
            }
            return View(returnM);
        }

        //public FileResult ExportFile()
        //{


        //    return File("C:\\Users\\Ficke\\Desktop\\Import2.xlsx", "application/xlsx", "NewImportYay.xlsx");
        //}

        // GET: PatientAssignment/EditAll
        public ActionResult EditAll()
        {
            var fromD = new DateTime();
            var toD = new DateTime();
            //List<string> physicians = new List<string>();
            //List<string> hospitals = new List<string>();
            //string hospitals;

            if (Session["patientAssignmentFromDate"] != null && Session["patientAssignmentToDate"] != null)
            {
                fromD = Convert.ToDateTime(Session["patientAssignmentFromDate"]);
                toD = Convert.ToDateTime(Session["patientAssignmentToDate"]);
            }
            else
            {
                fromD = DateTime.Now.AddDays(intDayFallback);
                toD = DateTime.Now;
            }
            //if (Session["patientAssignmentPhysicians"] != null)
            //{
            //    physicians = (List<string>)Session["patientAssignmentPhysicians"];
            //}
            //else
            //{
            //    physicians.Add(HubSecurity.getLoggedInUserID());
            //}
            //if (Session["patientAssignmentHospitals"] != null)
            //{
            //    hospitals = Session["patientAssignmentHospitals"].ToString();
            //}

            PatientAssignmentEditAllViewModel viewM = new PatientAssignmentEditAllViewModel();
            viewM.FromDate = fromD.ToShortDateString();
            viewM.ToDate = toD.ToShortDateString();
            viewM.isAdmin = (HubSecurity.isAdmin || HubSecurity.isSiteLeader);

            PatientLogEditViewModel oneEntry;
            List<int> patientList = (List<int>)Session["patientAssignmentListOfID"];

            //Checks if list is null, this should never be the case but stops page from erroring out. This session variable is only empty
            //if you browse to this page before using the Index page
            if (patientList != null)
            {
                foreach (int patID in patientList)
                {
                    oneEntry = new PatientLogEditViewModel();
                    oneEntry.Patient = db.PatientLogs.Find(patID);

                    oneEntry.GenderList = DataCollections.getGender(db, oneEntry.Patient.Gender);
                    oneEntry.HospitalList = DataCollections.getHospital(db, oneEntry.Patient.Hospital);
                    oneEntry.PCPList = DataCollections.getPCP(db, oneEntry.Patient.Hospital, oneEntry.Patient.PCP_Practice);
                    oneEntry.ServiceTypeList = DataCollections.getServiceType(db, oneEntry.Patient.ServiceType);
                    oneEntry.PatientClassList = DataCollections.getPatientClass(db, oneEntry.Patient.PatientClass);
                    oneEntry.PhysicianList = DataCollections.getAIMSPhy(db, oneEntry.Patient.Hospital, oneEntry.Patient.Physician);

                    viewM.Patients.Add(oneEntry);
                }
            }

            return View("EditAll", viewM);
        }

        [HttpPost]
        public JsonResult jsonImportFile(HttpPostedFileBase file)
        {
            file.SaveAs("C:\\Data\\PatientAssignment\\" + file.FileName);
            FileInfo savedFile = new FileInfo("C:\\Data\\PatientAssignment\\" + file.FileName);
            DataTable result = new DataTable();
            ExcelPackage pack = new ExcelPackage(savedFile);
            result = ToDataTable(pack);
            var newResult = JsonConvert.SerializeObject(result, Formatting.Indented,
                new JsonSerializerSettings
                {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            return Json(newResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult jsonImportFileCommit(HttpPostedFileBase file, string columns, bool copyDayBefore, bool sourceListFinal, string hospital)
        {
            file.SaveAs("C:\\Data\\PatientAssignment\\" + file.FileName);
            FileInfo savedFile = new FileInfo("C:\\Data\\PatientAssignment\\" + file.FileName);
            DataTable result = new DataTable();
            ExcelPackage pack = new ExcelPackage(savedFile);
            result = ToDataTable(pack);
            DateTime srvDate = DateTime.Now.Date + new TimeSpan(00, 00, 00); 
            string[] splitColumns = columns.Split(new char[] { ',' });
            List<string> listMRNs = new List<string>(); //List of imported MRN's used to evaluate whether the record should be deleted if sourceListFinal is true
            string[] dcs = new string[] { "DC - STD", "DC - EXT" };

            //Copies from day before if selected to do so
            if (copyDayBefore)
            {
                DateTime dayBefore = srvDate.AddDays(-1);
                var query = from p in db.PatientLogs
                            where p.ServiceDate == dayBefore && p.Hospital == hospital && !dcs.Contains(p.ServiceType)
                            select p;

                foreach (PatientLog pat in query)
                {
                    pat.ServiceDate = srvDate;
                    pat.ServiceType = "Assigned";
                    db.PatientLogs.Add(pat);
                }
                db.SaveChanges();
            }
            DataValidations validate = new DataValidations();

            //Parses the Excel spreadsheet to create a Patient object
            for (int i = 0; i < result.Rows.Count; i++)
            {
                PatientLog patient = new PatientLog();

                for (int j = 0; j < splitColumns.Length; j++)
                {
                    switch (splitColumns[j])
                    {
                        case "Comments":
                            patient.Comments = result.Rows[i][j].ToString();
                            break;
                        case "DOB":
                            try
                            {
                                patient.DOB = DateTime.Parse(result.Rows[i][j].ToString());
                            }
                            catch
                            {
                                patient.DOB = DateTime.Parse("1/1/1900");
                            }
                            break;
                        case "Gender":
                            patient.Gender = validate.checkGender(db, result.Rows[i][j].ToString());
                            break;
                        case "Hospital":
                            patient.Hospital = result.Rows[i][j].ToString();
                            break;
                        case "MRN_FIN":
                            patient.MRN_FIN = result.Rows[i][j].ToString();
                            listMRNs.Add(patient.MRN_FIN);
                            break;
                        case "PatientClass":
                            patient.PatientClass = result.Rows[i][j].ToString();
                            break;
                        case "PatientName":
                            patient.PatientName = result.Rows[i][j].ToString();
                            break;
                        case "PCP_Practice":
                            patient.PCP_Practice = validate.checkPCPandHosp(db, result.Rows[i][j].ToString(), hospital);
                            break;
                        case "Physician":
                            patient.Physician = validate.checkAIMSPhyAndHosp(db, result.Rows[i][j].ToString(), hospital);
                            break;
                        case "RoomNo":
                            patient.RoomNo = result.Rows[i][j].ToString();
                            break;
                        case "ServiceDate":
                            try
                            {
                                patient.ServiceDate = DateTime.Parse(result.Rows[i][j].ToString());
                            }
                            catch
                            {
                                patient.ServiceDate = DateTime.Parse(DateTime.Now.ToShortDateString());
                            }
                            break;
                        case "ServiceType":
                            patient.ServiceType = result.Rows[i][j].ToString();
                            break;
                    }
                }
                patient.Hospital = hospital;
                if (patient.ServiceDate == null)
                {
                    patient.ServiceDate = srvDate;
                }
                

                //Set default service type if none exists
                if (patient.ServiceType == null || patient.ServiceType == "")
                {
                    patient.ServiceType = "Assigned";
                }
                
                //Detect if patient already exists 
                bool alreadyExists = (from p in db.PatientLogs
                                      where p.PatientName == patient.PatientName && p.MRN_FIN == patient.MRN_FIN && p.ServiceDate == patient.ServiceDate
                                      select p).Any();

                //If does not exist, add, otherwise check if sourceFinalList is true and evaluate eligibility
                if (!alreadyExists)
                {
                    //Set physician to unassigned if none exists
                    if (patient.Physician == null || patient.Physician == "")
                    {
                        patient.Physician = "Unassigned";
                    }
                    patient.DateCreated = DateTime.Now;
                    db.PatientLogs.Add(patient);
                }
                else
                {
                    PatientLog oldData = (from p in db.PatientLogs
                                          where p.PatientName == patient.PatientName && p.MRN_FIN == patient.MRN_FIN && p.ServiceDate == patient.ServiceDate
                                          select p).First();

                    if (patient.Comments != null && patient.Comments != "")
                    {
                        oldData.Comments = patient.Comments;
                    }

                    if (patient.DOB != null)
                    {
                        oldData.DOB = patient.DOB;
                    }

                    if (patient.Gender != null && patient.Gender != "")
                    {
                        oldData.Gender = patient.Gender;
                    }

                    if (patient.Hospital != null && patient.Hospital != "")
                    {
                        oldData.Hospital = patient.Hospital;
                    }

                    if (patient.PatientClass != null && patient.PatientClass != "")
                    {
                        oldData.PatientClass = patient.PatientClass;
                    }

                    if (patient.PCP_Practice != null && patient.PCP_Practice != "")
                    {
                        oldData.PCP_Practice = patient.PCP_Practice;
                    }

                    if (patient.Physician != null && patient.Physician != "")
                    {
                        oldData.Physician = patient.Physician;
                    }

                    if (patient.RoomNo != null && patient.RoomNo != "")
                    {
                        oldData.RoomNo = patient.RoomNo;
                    }

                    if (patient.ServiceType != null && patient.ServiceType != "")
                    {
                        oldData.ServiceType = patient.ServiceType;
                    }

                    db.Entry(oldData).State = EntityState.Modified;
                    //oldData = MergePatient(oldData, patient);
                    //db.SaveChanges();
                }

                //patient.Hospital = hospital;
                //importList.Add(patient);
            }
            db.SaveChanges();


            //If sourceListFinal is true and this MRN is not found, it is deleted
            if (sourceListFinal)
            {
                var todaysPatients = from p in db.PatientLogs
                                     where p.ServiceDate == srvDate && p.Hospital == hospital && !dcs.Contains(p.ServiceType)
                                     select p;
                foreach (PatientLog pat in todaysPatients)
                {
                    if (!listMRNs.Contains(pat.MRN_FIN))
                    {
                        db.PatientLogs.Remove(pat);
                    }
                }
            }

            db.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult jsonCopyPatients(string ids)
        {
            string[] split = ids.Split(new char[] { ',' });

            DataSubmissions.CopyPatients(db, split);
            //for (int i = 0; i < ids.Length; i++)
            //{
            //    int id = Convert.ToInt32(split[i]);
            //    PatientLog patientLog = db.PatientLogs.Find(id);

            //    //Reset values for copied record
            //    patientLog.ServiceDate = DateTime.Parse(DateTime.Now.ToShortDateString());
            //    patientLog.DateCreated = DateTime.Now;
            //    patientLog.LastUpdated = null;
            //    patientLog.Notes = null;

            //    db.PatientLogs.Add(patientLog);
            //}
            //db.SaveChanges();
            return Content("Boo");
        }

        [HttpPost]
        public JsonResult jsonDeletePatients(string ids)
        {
            string[] split = ids.Split(new char[] { ',' });

            DataSubmissions.DeletePatients(db, split);
            //for (int i = 0; i < ids.Length; i++)
            //{
            //    int id = Convert.ToInt32(split[i]);
            //    PatientLog patient = db.PatientLogs.Find(id);
            //    //PatientLogTmp pt = DataSubmissions.CreateTmpPatient(patient);
            //    //db.PatientLogTmps.Add(pt);
            //    //db.PatientLogs.Remove(patient);
            //    DataSubmissions.DeletePatients()
            //}
            //db.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult jsonExportData(string[][] listData, string fileType)
        { 
            if (fileType == "excel")
            {
                FileManager.CreateExcelFile(listData, "test.xlsx");
            }
            else if (fileType == "pdf")
            {
                FileManager.CreatePDF(listData, "test.pdf");
            }

            //return Json(filePath, JsonRequestBehavior.AllowGet);
            return Json("test.xlsx", JsonRequestBehavior.AllowGet);
        }

        private PatientLog MergePatient(PatientLog existing, PatientLog newData)
        {
            PatientLog returnPatient = new PatientLog();

            returnPatient.ID = existing.ID;
            returnPatient.AIMSComments = existing.AIMSComments;
            returnPatient.MRN_FIN = existing.MRN_FIN;
            returnPatient.ServiceDate = existing.ServiceDate;
            returnPatient.PatientName = existing.PatientName;
            returnPatient.DateCreated = existing.DateCreated;
            returnPatient.LastUpdated = existing.LastUpdated;
            returnPatient.FaceSheet = existing.FaceSheet;
            returnPatient.Notes = existing.Notes;

            if (newData.Comments != null && newData.Comments != "")
            {
                returnPatient.Comments = newData.Comments;
            }
            else
            {
                returnPatient.Comments = existing.Comments;
            }

            if (newData.DOB != null)
            {
                returnPatient.DOB = newData.DOB;
            }
            else
            {
                returnPatient.DOB = existing.DOB;
            }

            if (newData.Gender != null && newData.Gender != "")
            {
                returnPatient.Gender = newData.Gender;
            }
            else
            {
                returnPatient.Gender = existing.Gender;
            }

            if (newData.Hospital != null && newData.Hospital != "")
            {
                returnPatient.Hospital = newData.Hospital;
            }
            else
            {
                returnPatient.Hospital = existing.Hospital;
            }

            if (newData.PatientClass != null && newData.PatientClass != "")
            {
                returnPatient.PatientClass = newData.PatientClass;
            }
            else
            {
                returnPatient.PatientClass = existing.PatientClass;
            }

            if (newData.PCP_Practice != null && newData.PCP_Practice != "")
            {
                returnPatient.PCP_Practice = newData.PCP_Practice;
            }
            else
            {
                returnPatient.PCP_Practice = existing.PCP_Practice;
            }

            if (newData.Physician != null && newData.Physician != "")
            {
                returnPatient.Physician = newData.Physician;
            }
            else
            {
                returnPatient.Physician = existing.Physician;
            }

            if (newData.RoomNo != null && newData.RoomNo != "")
            {
                returnPatient.RoomNo = newData.RoomNo;
            }
            else
            {
                returnPatient.RoomNo = existing.RoomNo;
            }

            if (newData.ServiceType != null && newData.ServiceType != "")
            {
                returnPatient.ServiceType = newData.ServiceType;
            }
            else
            {
                returnPatient.ServiceType = existing.ServiceType;
            }

            return returnPatient;
        }

        private DataTable ToDataTable(ExcelPackage package)
        {
            DataTable ret = new DataTable();
            ExcelWorksheet ws = package.Workbook.Worksheets.First();

            for (int i = 0; i < ws.Dimension.End.Column; i++)
            {
                ret.Columns.Add(i.ToString());
            }

            for (int rown = 1; rown <= ws.Dimension.End.Row; rown++)
            {
                var row = ws.Cells[rown, 1, rown, ws.Dimension.End.Column];
                var newRow = ret.NewRow();
                foreach (var cell in row)
                {
                    newRow[cell.Start.Column - 1] = cell.Text;
                }
                ret.Rows.Add(newRow);
            }

            return ret;
        }

        private string DataTableToJSON(DataTable dt)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("0", dt);
            JavaScriptSerializer json = new JavaScriptSerializer();
            return json.Serialize(dict);
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