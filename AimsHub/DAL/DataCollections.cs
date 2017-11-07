using AimsHub.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AimsHub.Security;
using System.Data;
using static AimsHub.Extensions.EventManager;
using System.Globalization;

namespace AimsHub.DAL
{
    public static class DataCollections
    {
        //Returns DISTINCT SelectList containing all AIMSPhy users
        public static SelectList getAIMSPhy(PatientLogModel db)
        {
            IQueryable<SelectListItem> phyModel = from u in db.Users
                                                  join ud in db.UserDetails on u.UserID equals ud.UserID
                                                  where (ud.UserType == "AIMSPhy" && ud.Active == true)
                                                  select new SelectListItem { Text = (u.LastName + ", " + u.FirstName), Value = u.UserID };

            phyModel = phyModel.Distinct().OrderBy(u => u.Text);
            List<SelectListItem> list = phyModel.ToList();
            list.Insert(0, new SelectListItem { Text = "Unassigned", Value = "Unassigned" });
            return new SelectList(list, "Value", "Text");
        }
        //Returns DISTINCT SelectList containing all AIMSPhy users (This is a copy of above with ALL instead of Unassigned)
        //public static SelectList getAIMSPhy2(PatientLogModel db)
        //{
        //    IQueryable<SelectListItem> phyModel = from u in db.Users
        //                                          join ud in db.UserDetails on u.UserID equals ud.UserID
        //                                          where (ud.UserType == "AIMSPhy" && ud.Active == true)
        //                                          select new SelectListItem { Text = (u.LastName + ", " + u.FirstName), Value = u.UserID };

        //    phyModel = phyModel.Distinct().OrderBy(u => u.Text);
        //    List<SelectListItem> list = phyModel.ToList();
        //    list.Insert(0, new SelectListItem { Text = "ALL", Value = "ALL" });
        //    return new SelectList(list, "Value", "Text");
        //}
        //Returns DISTINCT SelectList containing all AIMSPhy users associated with a hospital
        public static SelectList getAIMSPhy(PatientLogModel db, string hosp)
        {
            IQueryable<SelectListItem> phyModel = from u in db.Users
                                                  join ud in db.UserDetails on u.UserID equals ud.UserID
                                                  where (ud.UserType == "AIMSPhy" && ud.Active == true) && (ud.DefaultHospital == hosp)
                                                  select new SelectListItem { Text = (u.LastName + ", " + u.FirstName), Value = u.UserID };
            List<SelectListItem> list = phyModel.ToList();
            list.Insert(0, new SelectListItem { Text = "Unassigned", Value = "Unassigned" });
            return new SelectList(list, "Value", "Text");
        }
        //Returns DISTINCT SelectList containing all AIMSPhy users associated with a hospital and a default physician selected in the list
        public static SelectList getAIMSPhy(PatientLogModel db, string hosp, string phy)
        {
            IQueryable<SelectListItem> phyModel = from u in db.Users
                                                  join ud in db.UserDetails on u.UserID equals ud.UserID
                                                  where (ud.UserType == "AIMSPhy" && ud.Active == true) && (ud.DefaultHospital == hosp)
                                                  select new SelectListItem { Text = (u.LastName + ", " + u.FirstName), Value = u.UserID };

            List<SelectListItem> list = phyModel.ToList();
            list.Insert(0, new SelectListItem { Text = "Unassigned", Value = "Unassigned" });
            return new SelectList(list, "Value", "Text", phy);
        }
        //Returns SelectList containing all PCP's
        public static SelectList getPCP(PatientLogModel db)
        {
            IQueryable<SelectListItem> pcpModel = from u in db.Users
                           join ud in db.UserDetails on u.UserID equals ud.UserID
                           where (ud.UserType == "RefPhy")
                           orderby u.LastName
                           select new SelectListItem { Value = (u.LastName + ", " + u.FirstName), Text = (u.LastName + ", " + u.FirstName) };
            pcpModel = pcpModel.Distinct().OrderBy(u => u.Value);

            List<SelectListItem> list = pcpModel.ToList();
            list.Insert(0, new SelectListItem { Text = "No PCP", Value = "No PCP" });
            return new SelectList(list, "Value", "Value");
        }

        //Returns SelectList containing PCP's from a single site
        public static SelectList getPCP(PatientLogModel db, string hosp)
        {
            IQueryable<SelectListItem> pcpModel = from u in db.Users
                           join ud in db.UserDetails on u.UserID equals ud.UserID
                           where (ud.UserType == "RefPhy") && (ud.DefaultHospital == hosp)
                           orderby u.LastName
                           select new SelectListItem { Value = (u.LastName + ", " + u.FirstName), Text = (u.LastName + ", " + u.FirstName) };

            List<SelectListItem> list = pcpModel.ToList();
            list.Insert(0, new SelectListItem { Text = "No PCP", Value = "No PCP" });
            return new SelectList(list, "Value", "Value");
        }

        //Returns SelectList containing PCP's from a single site with the passed PCP selected
        public static SelectList getPCP(PatientLogModel db, string hosp, string pcp)
        {
            IQueryable<SelectListItem> pcpModel = from u in db.Users
                           join ud in db.UserDetails on u.UserID equals ud.UserID
                           where (ud.UserType == "RefPhy") && (ud.DefaultHospital == hosp)
                           orderby u.LastName
                           select new SelectListItem  { Value = (u.LastName + ", " + u.FirstName), Text = (u.LastName + ", " + u.FirstName) };


            List<SelectListItem> list = pcpModel.ToList();
            list.Insert(0, new SelectListItem { Text = "No PCP", Value = "No PCP" });
            return new SelectList(list, "Value", "Value", pcp);
        }

        //Returns List<string> containing PCP names based on an admin user
        public static List<string> getPracticeAdminPCPs(PatientLogModel db, string userID)
        {
            List<string> retList = new List<string>();

            List<string> practices = (from r in db.RefPracAdmins
                             where r.UserID == userID
                             select r.UserID).Distinct().ToList();

            var pcpmodel = (from u in db.Users
                           join ud in db.UserDetails on u.UserID equals ud.UserID
                           where (ud.UserType == "RefPhy" && practices.Contains(ud.UserID)) 
                           orderby u.LastName
                           select new { Value = (u.LastName + ", " + u.FirstName) }).Distinct();

            retList = pcpmodel.Select(x => x.Value).ToList();
            return retList;
        }

        //Returns SelectList containing PatientClass types
        public static SelectList getPatientClass(PatientLogModel db)
        {
            return new SelectList(db.PatientClasses, "ShortName", "ShortName", "None");
        }

        //Returns SelectList containing PatientClass types with passed pclass as selected
        public static SelectList getPatientClass(PatientLogModel db, string pclass)
        {
            return new SelectList(db.PatientClasses, "ShortName", "ShortName", pclass);
        }

        //Returns genders
        public static SelectList getGender(PatientLogModel db)
        {
            return new SelectList(db.Genders, "Gender1", "Gender1", "Male");
        }

        //Returns SelectList of genders with specified gender as default
        public static SelectList getGender(PatientLogModel db, string gen)
        {
            return new SelectList(db.Genders, "Gender1", "Gender1", gen);
        }

        //Returns servicetypes
        public static SelectList getServiceType(PatientLogModel db)
        {
            return new SelectList(db.ServiceTypes, "Service", "Service");
        }
        //Returns SelectList of servicetype with specified service as default
        public static SelectList getServiceType(PatientLogModel db, string serv)
        {
            return new SelectList(db.ServiceTypes, "Service", "Service", serv);
        }

        //Returns list of hospitals
        public static SelectList getHospital(PatientLogModel db)
        {
            var hospModel = from h in db.Hospitals
                            where h.HospitalType == "HOS" && h.Active == true
                            select h;
            return new SelectList(hospModel, "ShortName", "ShortName");
        }

        //Returns list of hospitals with specified hosp as default
        public static SelectList getHospital(PatientLogModel db, string hosp)
        {
            var hospModel = from h in db.Hospitals
                            where h.HospitalType == "HOS" && h.Active == true
                            select h;
            return new SelectList(hospModel, "ShortName", "ShortName", hosp);
        }
        //Returns SelectList of letters A-Z
        public static SelectList getLastNameFilter()
        {
            List<string> alphaList = new List<string>();
            for (int i = 65; i < 91; i++)
            {
                char character = (char)i;
                string input = character.ToString();
                alphaList.Add(input);
            }
            return new SelectList(alphaList);
        }
        //Returns SelectList of Yes/No that have the value of true/false
        public static SelectList getYesNo()
        {
            List<SelectListItem> YesNoList = new List<SelectListItem>();
            YesNoList.Add(new SelectListItem { Text = "Yes", Value = "true" });
            YesNoList.Add(new SelectListItem { Text = "No", Value = "false" });
            return new SelectList(YesNoList, "Value", "Text");
        }
        //Returns list of hospitals with specified hosp as default
        public static IEnumerable<SelectListItem> getHospitalList(PatientLogModel db)
        {
            IEnumerable<SelectListItem> returnList = from h in db.Hospitals
                                                     where h.HospitalType == "HOS" && h.Active == true
                                                     select new SelectListItem
                                                     {
                                                         Value = h.ShortName,
                                                         Text = h.ShortName
                                                     };
            return returnList;
        }

        public static SelectList getAllLocationsByType(PatientLogModel db, bool addNone)
        {
            IQueryable<Hospital> hosps = from h in db.Hospitals
                                          where h.Active == true
                                          select h;

            hosps = hosps.OrderBy(h => (h.HospitalType == "HOS") ? 0 : (h.HospitalType == "NH") ? 1 : 2).ThenBy(h => h.ShortName);
            List<SelectListItem> list = new List<SelectListItem>();
            if (addNone)
            {
                list.Add(new SelectListItem { Text = "-none-", Value = "none" });
            }
            foreach (Hospital hosp in hosps)
            {
                list.Add(new SelectListItem { Text = hosp.ShortName, Value = hosp.ShortName });
            }
            return new SelectList(list, "Value", "Text");
        }
        public static SelectList getScheduleTypes(PatientLogModel db)
        {
            IQueryable<SelectListItem> types = from t in db.ScheduleTypes
                                               orderby t.Event
                                               select new SelectListItem { Value = t.Event, Text = t.Event };
            List<SelectListItem> list = types.ToList();
            return new SelectList(list, "Value", "Text");
        }
        public static SelectList getScheduleRoundingTypes(PatientLogModel db)
        {
            IQueryable<SelectListItem> types = from t in db.ScheduleRoundingTypes
                                               orderby t.Event
                                               select new SelectListItem { Value = t.Event, Text = t.Event };
            List<SelectListItem> list = types.ToList();
            return new SelectList(list, "Value", "Text");
        }
        //Returns list of CPT Codes
        public static SelectList getCPTCodesList(PatientLogModel db)
        {
            var returnList = from c in db.BillingCPTCodes
                             select new
                             {
                                 Value = c.Code,
                                 Text = c.Code + " - " + c.Description
                             };
            return new SelectList(returnList, "Value", "Text");
        }
        //Returns list of POS Codes
        public static SelectList getPOSCodesList(PatientLogModel db)
        {
            var returnList = from c in db.BillingPOSCodes
                             select new
                             {
                                 Value = c.Code,
                                 Text = c.Code + " - " + c.Description
                             };
            return new SelectList(returnList, "Value", "Text");
        }
        //Returns list of MOD Codes
        public static SelectList getMODCodesList(PatientLogModel db)
        {
            var returnList = from c in db.BillingMODCodes
                             select new
                             {
                                 Value = c.Code,
                                 Text = c.Code + " - " + c.Description
                             };
            return new SelectList(returnList, "Value", "Text");
        }
        //Returns list of ICD 10 Codes
        public static SelectList getDXCodesList(PatientLogModel db)
        {
            var returnList = from c in db.BillingICD10Codes
                             select new SelectListItem
                             {
                                 Value = c.Code,
                                 Text = c.Code + " - " + c.Description
                             };
            return new SelectList(returnList, "Value", "Text");
        }
        //Returns list of ICD 10 Codes
        public static SelectList getDXCodesList(PatientLogModel db, string searcher)
        {
            var returnList = from c in db.BillingICD10Codes
                             where (c.Code.Contains(searcher) || c.Description.Contains(searcher))
                             select new SelectListItem
                             {
                                 Value = c.Code,
                                 Text = c.Code + " - " + c.Description
                             };
            return new SelectList(returnList, "Value", "Text");
            
        }
        //Returns SelectList of all data from FileImportColumns table
        public static SelectList getImportColumns(PatientLogModel db)
        {
            var col = from f in db.FileImportColumns
                      select f;

            return new SelectList(col, "Column", "DisplayName");
        }
        //public static List<string> getArcturusPCPs(PatientLogModel db)
        //{
        //    var returnList = from u in db.Users
        //                     join ud in db.UserDetails on u.UserID equals ud.UserID
        //                     where (ud.UserType == "RefPhy") && (from rp in db.RefPractUser join prac in db.ReferringPractices on rp.PracID equals prac.PracID where prac.LegacyShortName.StartsWith("ARCH") select rp.RefPracUser).Contains(ud.UserID)
        //                     orderby u.LastName
        //                     select new { Value = (u.LastName + ", " + u.FirstName) };

        //    return returnList.ToList();
        //}
        //Returns JSON list of PCPs that match the specified site
        //public static IQueryable< jsonPCP(string site)
        //{
        //    var query = from u in db.Users
        //                join ud in db.UserDetails on u.UserID equals ud.UserID
        //                where (ud.UserType == "RefPhy") && (ud.DefaultHospital == site)
        //                orderby u.LastName
        //                select new { Value = (u.LastName + ", " + u.FirstName) };

        //    return Json(query, JsonRequestBehavior.AllowGet);
        //}

        ////Returns JSON list of AIMS Physicians that match the specified site
        //public ActionResult jsonAIMSPhy(string site)
        //{
        //    var query = from u in db.Users
        //                join ud in db.UserDetails on u.UserID equals ud.UserID
        //                where (ud.UserType == "AIMSPhy" && ud.Active == true) && (ud.DefaultHospital == site)
        //                orderby u.LastName
        //                select new { Text = (u.LastName + ", " + u.FirstName), Value = u.UserID };

        //    return Json(query, JsonRequestBehavior.AllowGet);
        //}

        public static string LoadPreference(PatientLogModel db, string controller, string viewModel, string filterName)
        {
            //Code already operates on null values so no casting is needed from this call
            string userID = HubSecurity.getLoggedInUserID();
            string result = (from u in db.UserPreferences where u.UserID == userID && u.Controller == controller && u.ViewModel == viewModel && u.FilterName == filterName select u.FilterValue).FirstOrDefault();
            return result;
        }

        public static Dictionary<string,string> LoadPreferences(PatientLogModel db, string controller, string viewModel)
        {
            Dictionary<string, string> preferences = new Dictionary<string, string>();
            string userID = HubSecurity.getLoggedInUserID();
            preferences = (from u in db.UserPreferences where u.UserID == userID && u.Controller == controller && u.ViewModel == viewModel select new { Key = u.FilterName, Value = u.FilterValue }).ToDictionary(t => t.Key, t => t.Value);
            return preferences;
        }

        public static ReferringPractice getPractice(PatientLogModel db, int PracID)
        {
            ReferringPractice prac = (from r in db.ReferringPractices where r.PracID == PracID select r).Single();
            return prac;
        }

        public static ReferringPractice getPractice(PatientLogModel db, string PracID)
        {
            int ID = Convert.ToInt32(PracID);
            ReferringPractice prac = (from r in db.ReferringPractices where r.PracID == ID select r).Single();
            return prac;
        }

        //Returns SelectList containing practice friendly name and practice ID for value
        public static SelectList getPractices(PatientLogModel db, string userID)
        {
            IQueryable<SelectListItem> practices = from p in db.ReferringPractices
                                                  join a in db.RefPracAdmins on p.PracID equals a.PracID
                                                  where a.UserID == userID
                                                  orderby p.PracName
                                                  select new SelectListItem { Value = p.PracID.ToString() , Text = p.PracName };

            List<SelectListItem> list = practices.ToList();

            if (list.Count > 1)
            {
                list.Insert(0, new SelectListItem { Text = "ALL", Value = "ALL" });
            }
            //list.Insert(0, new SelectListItem { Text = "No PCP", Value = "No PCP" });
            return new SelectList(list, "Value", "Text");
        }

        //Returns SelectList containing practice friendly name and practice ID for value with provided practice selected
        public static SelectList getPractices(PatientLogModel db, string userID, string selectedPrac)
        {
            IQueryable<SelectListItem> practices = from p in db.ReferringPractices
                                                   join a in db.RefPracAdmins on p.PracID equals a.PracID
                                                   where a.UserID == userID
                                                   orderby p.PracName
                                                   select new SelectListItem { Value = p.PracID.ToString(), Text = p.PracName };

            List<SelectListItem> list = practices.ToList();

            if (list.Count > 1)
            {
                list.Insert(0, new SelectListItem { Text = "ALL", Value = "ALL" });
            }
            //list.Insert(0, new SelectListItem { Text = "No PCP", Value = "No PCP" });
            return new SelectList(list, "Value", "Text", selectedPrac);
        }

        //Searches RefPracSpecialty for provided practice, if found returns results, else returns a blank default of common specialties
        public static List<RefPracSpecialty> getSpecialties(PatientLogModel db, int selectedPrac)
        {
            IQueryable<RefPracSpecialty> specials = from s in db.RefPracSpecialties
                                                    where s.PracID == selectedPrac
                                                    select s;

            List<RefPracSpecialty> list = new List<RefPracSpecialty>();
            if (specials.Any() == false)
            {
                list.Add(new RefPracSpecialty { Specialty = "Allergy and Immunology", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Anesthesiology", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Cardiology - EP", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Cardiology - General", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Cardiology - Interventional", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Cardiology - Surgery", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Dermatology", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Endocrinology", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "ENT", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Gastroenterology", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Hematology/Oncology", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Infectious Disease", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Nephrology", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Neurology", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Neurosurgery", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "OB/GYN", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "OMFS", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Ophthamology", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Orthopedic Spine", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Orthopedic Surgery", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Orthopedics", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Other 1", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Other 2", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Other 3", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Other 4", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Other 5", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Plastic Surgery", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "PMR", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Podiatry", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Psychiatry", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Pulmonary/CCM", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Rheumatology", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Surgery - Cardio Thoracic", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Surgery - Colon & Rectal", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Surgery - General", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Surgery - Neurological", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Surgery - Orthopedic", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Surgery - Plastic", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Surgery - Thoracic", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Surgery - Urological", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Surgery - Vascular", FirstChoice = "", Backup = "", Comments = "", ID = 0 });
                list.Add(new RefPracSpecialty { Specialty = "Urology", FirstChoice = "", Backup = "", Comments = "", ID = 0 });

            }
            else
            {
                list = specials.ToList();
            }

            return list;
        }
        public static SelectList getScheduleUsers(PatientLogModel db)
        {
            string userTypes = "AIMSPhy,ContractPhy,Staff";
            IQueryable<SelectListItem> users = (from u in db.Users
                                                join ud in db.UserDetails on u.UserID equals ud.UserID
                                                where userTypes.Contains(ud.UserType)
                                                select new SelectListItem { Value = u.UserID, Text = u.LastName + ", " + u.FirstName }).Distinct();
            users = users.OrderBy(v => v.Text);
            List<SelectListItem> list = users.ToList();
            return new SelectList(list, "Value", "Text");
        }

        public static ScheduleFavorite getFavorite(PatientLogModel db, string id)
        {
            int newID = Convert.ToInt32(id);
            ScheduleFavorite ret = (from f in db.ScheduleFavorites where f.ID == newID select f).Single();
            return ret;
        }
        //Returns SelectList containing practice friendly name and practice ID for value with provided practice selected
        public static SelectList getFavorites(PatientLogModel db)
        {
            string userID = HubSecurity.getLoggedInUserID();
            IQueryable<SelectListItem> practices = from f in db.ScheduleFavorites
                                                   where f.UserID == userID
                                                   select new SelectListItem { Value = f.ID.ToString(), Text = f.Name};

            List<SelectListItem> list = practices.ToList();
            //if (list.Count == 0)
            //{
            //    list.Add(new SelectListItem { Text = "", Value = "" });
            //}
            return new SelectList(list, "Value", "Text");
        }

        public static string getFavoriteDefault(PatientLogModel db)
        {
            string userID = HubSecurity.getLoggedInUserID();
            var fav = (from f in db.ScheduleFavorites where f.UserID == userID && f.Default == true select f.Name).FirstOrDefault();

            if (fav == null)
            {
                return "[none]";
            }
            else
            {
                return fav.ToString();
            }
        }

        //Returns SelectList of Month Week Day for schedule view
        public static SelectList getMonthWeekDay()
        {
            List<SelectListItem> YesNoList = new List<SelectListItem>();
            YesNoList.Add(new SelectListItem { Text = "Month", Value = "Month" });
            YesNoList.Add(new SelectListItem { Text = "Week", Value = "Week" });
            YesNoList.Add(new SelectListItem { Text = "Day", Value = "Day" });
            return new SelectList(YesNoList, "Value", "Text");
        }
        public static SelectList getHours()
        {
            int interval = 13;
            int i = 0;
            List<SelectListItem> Hours = new List<SelectListItem>();
            for (i = 1; i < interval; i++)
            {
                Hours.Add(new SelectListItem { Text = i.ToString() + " AM", Value = i.ToString() });
            }
            for (i = 1; i < interval; i++)
            {
                Hours.Add(new SelectListItem { Text = i.ToString() + " PM", Value = (i + 12).ToString() });
            }
            return new SelectList(Hours, "Value", "Text");
        }
        public static SelectList getMinutes()
        {
            int i = 0;
            int step = 5;
            List<SelectListItem> Minutes = new List<SelectListItem>();
            for (i = 0; i < 56; i += step)
            {
                if (i < 11)
                {
                    Minutes.Add(new SelectListItem { Text = "0" + i.ToString(), Value = "0" + i.ToString() });
                }
                else
                {
                    Minutes.Add(new SelectListItem { Text = i.ToString(), Value= i.ToString() });
                }           
            }
            return new SelectList(Minutes, "Value", "Text");
        }
        public static SelectList getRepeats()
        {
            List<SelectListItem> Repeats = new List<SelectListItem>();
            Repeats.Add(new SelectListItem { Text = "None", Value = "None" });
            Repeats.Add(new SelectListItem { Text = "Daily", Value = "Daily" });
            Repeats.Add(new SelectListItem { Text = "Weekly", Value = "Weekly" });
            //Repeats.Add(new SelectListItem { Text = "Monthly", Value = "Monthly" });
            return new SelectList(Repeats, "Value", "Text");
        }
        public static SelectList getMassEditCriteria()
        {
            List<SelectListItem> Repeats = new List<SelectListItem>();
            Repeats.Add(new SelectListItem { Text = "Hospital", Value = "Hospital" });
            Repeats.Add(new SelectListItem { Text = "StartTime", Value = "StartTime" });
            Repeats.Add(new SelectListItem { Text = "EndTime", Value = "EndTime" });
            Repeats.Add(new SelectListItem { Text = "ScheduleType", Value = "ScheduleType" });
            Repeats.Add(new SelectListItem { Text = "UserID", Value = "UserID" });
            return new SelectList(Repeats, "Value", "Text");
        }
        public static DataTable getSchedule(PatientLogModel db, DateTime start, DateTime end, List<string> users, List<string> hospitals, List<string> types, bool rounding)
        {
            DataTable returnDT = new DataTable();
            returnDT.Columns.Add("ID");
            returnDT.Columns.Add("StartTime");
            returnDT.Columns.Add("EndTime");
            returnDT.Columns.Add("Title");
            returnDT.Columns.Add("ScheduleType");
            returnDT.Columns.Add("UserID");
            returnDT.Columns.Add("Hospital");

            var query = from s in db.Schedules
                        where s.StartTime >= start && s.EndTime <= end
                        select s;

            if (rounding)
            {
                List<string> round = (from t in db.ScheduleRoundingTypes select t.Event).ToList<string>();
                query = query.Where(s => round.Contains(s.ScheduleType));
            }
            if (users.Any())
            {
                query = query.Where(s => users.Contains(s.UserID));
            }
            if (hospitals.Any())
            {
                query = query.Where(s => hospitals.Contains(s.HospitalShortName));
            }
            if (types.Any())
            {
                query = query.Where(s => types.Contains(s.ScheduleType));
            }

            //Implement Ordering
            //Orders by Day, then Day Nursing, everything else, and finally Scheduled Off at the bottom, alphabetical order of the users for each type
            query = query.OrderBy(s => (s.ScheduleType == "Day-(D)") ? 0 : (s.ScheduleType == "Day Nursing-(DN)") ? 1 : (s.ScheduleType == "Scheduled Off-(O)") ? 100 : 2).ThenBy(s => s.UserID.Substring(1));

            foreach (Schedule sch in query)
            {
                string displayStart = sch.StartTime.Value.ToString("htt");
                string displayEnd = sch.EndTime.Value.ToString("htt");
                DataRow dr = returnDT.NewRow();
                dr["ID"] = sch.ID;
                dr["StartTime"] = sch.StartTime;
                //If time span is overnight, set End Time same as Start to avoid tag spilling into the next day
                if (sch.EndTime.Value.Day == sch.StartTime.Value.Day + 1)
                {
                    dr["EndTime"] = sch.StartTime;
                }
                else
                {
                    dr["EndTime"] = sch.EndTime;
                }
                
                dr["Title"] = displayStart + ' ' + sch.Title + ' ' + displayEnd;
                dr["ScheduleType"] = sch.ScheduleType;
                dr["UserID"] = sch.UserID;
                dr["Hospital"] = sch.HospitalShortName;
                returnDT.Rows.Add(dr);
            }

            return returnDT;
        }

        public static DataTable getScheduleWorkArea(PatientLogModel db, DateTime start, DateTime end, List<string> users, List<string> hospitals, List<string> types, bool rounding)
        {
            DataTable returnDT = new DataTable();
            returnDT.Columns.Add("ID");
            returnDT.Columns.Add("StartTime");
            returnDT.Columns.Add("EndTime");
            returnDT.Columns.Add("Title");
            returnDT.Columns.Add("ScheduleType");
            returnDT.Columns.Add("UserID");
            returnDT.Columns.Add("Hospital");

            var query = from s in db.ScheduleWorkAreas
                        where s.StartTime >= start && s.EndTime <= end
                        select s;

            if (rounding)
            {
                List<string> round = (from t in db.ScheduleRoundingTypes select t.Event).ToList<string>();
                query = query.Where(s => round.Contains(s.ScheduleType));
            }
            if (users.Any())
            {
                query = query.Where(s => users.Contains(s.UserID));
            }
            if (hospitals.Any())
            {
                query = query.Where(s => hospitals.Contains(s.HospitalShortName));
            }
            if (types.Any())
            {
                query = query.Where(s => types.Contains(s.ScheduleType));
            }

            //Implement Ordering
            //Orders by Day, then Day Nursing, everything else, and finally Scheduled Off at the bottom, alphabetical order of the users for each type
            query = query.OrderBy(s => (s.ScheduleType == "Day-(D)") ? 0 : (s.ScheduleType == "Day Nursing-(DN)") ? 1 : (s.ScheduleType == "Scheduled Off-(O)") ? 100 : 2).ThenBy(s => s.UserID.Substring(1));

            foreach (ScheduleWorkArea sch in query)
            {
                string displayStart = sch.StartTime.Value.ToString("htt");
                string displayEnd = sch.EndTime.Value.ToString("htt");
                DataRow dr = returnDT.NewRow();
                dr["ID"] = sch.ID;
                dr["StartTime"] = sch.StartTime;
                //If time span is overnight, set End Time same as Start to avoid tag spilling into the next day
                if (sch.EndTime.Value.Day == sch.StartTime.Value.Day + 1)
                {
                    dr["EndTime"] = sch.StartTime;
                }
                else
                {
                    dr["EndTime"] = sch.EndTime;
                }

                dr["Title"] = displayStart + ' ' + sch.Title + ' ' + displayEnd;
                dr["ScheduleType"] = sch.ScheduleType;
                dr["UserID"] = sch.UserID;
                dr["Hospital"] = sch.HospitalShortName;
                returnDT.Rows.Add(dr);
            }

            return returnDT;
        }

        public static Schedule getSchedule(PatientLogModel db, string id)
        {
            int newID = Convert.ToInt32(id);
            Schedule schedule = db.Schedules.Find(newID);
            return schedule;
        }

        public static ScheduleWorkArea getScheduleWorkArea(PatientLogModel db, string id)
        {
            int newID = Convert.ToInt32(id);
            ScheduleWorkArea schedule = db.ScheduleWorkAreas.Find(newID);
            return schedule;
        }

        public static List<ScheduleType> getScheduleTypeReference(PatientLogModel db)
        {
            IQueryable<ScheduleType> query = from s in db.ScheduleTypes
                                      select s;

            List<ScheduleType> list = query.ToList();
            return list;
        }

        public static SelectList getPatterns(PatientLogModel db)
        {
            IQueryable<SelectListItem> patterns = from p in db.SchedulePatterns
                                                  select new SelectListItem { Text = p.PatternName, Value = p.Pattern } ;

            List<SelectListItem> list = patterns.ToList();
            return new SelectList(list, "Value", "Text");
        }

        //public static DataTable getTally(PatientLogModel db, List<string> users, List<string> hospitals, List<string> types, DateTime start, DateTime end)
        //{
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("UserID");
        //    dt.Columns.Add("HospitalShortName");
        //    dt.Columns.Add("ScheduleType");
        //    dt.Columns.Add("Census");

        //    var query = from s in db.ScheduleWorkAreas
        //                where s.StartTime >= start && s.EndTime <= end
        //                group s by new { s.UserID, s.HospitalShortName }
        //                select s;

        //    if (users.Any())
        //    {
        //        query = query.Where(s => users.Contains(s.UserID));
        //    }
        //    if (hospitals.Any())
        //    {
        //        query = query.Where(s => hospitals.Contains(s.HospitalShortName));
        //    }
        //    if (types.Any())
        //    {
        //        query = query.Where(s => types.Contains(s.ScheduleType));
        //    }

        //    //query.GroupBy(x => new { x.UserID, x.HospitalShortName });

        //    //dt = query.Select(x => new { UserID = x.UserID, HospitalShortName = x.HospitalShortName, ScheduleType = x.ScheduleType, Census = x.ScheduleType.Count() });

        //}
    }
}