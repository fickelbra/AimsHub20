using AimsHub.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using AimsHub.Security;
using System.Web.Routing;

namespace AimsHub.DAL
{
    public class DataSubmissions
    {
        //private static string getController()
        //{
        //    try
        //    {
        //        return HttpContext.Current.Request.FilePath.Substring(1, HttpContext.Current.Request.FilePath.LastIndexOf("/") - 1);
        //    }
        //    catch (Exception ex)
        //    {
        //        return "getControllerErrored";
        //    }

        //}

        //private static string getPage()
        //{
        //    try
        //    {
        //        return HttpContext.Current.Request.FilePath.Substring(HttpContext.Current.Request.FilePath.LastIndexOf("/") + 1);
        //    }
        //    catch (Exception ex)
        //    {
        //        return "getPageErrored";
        //    }
        //}
        public static int CreateFavorite(PatientLogModel db, string users, string hospitals, string types, string name, bool isDefault)
        {
            ScheduleFavorite fave = new ScheduleFavorite();
            fave.Users = users;
            fave.Hospital = hospitals;
            fave.Types = types;
            fave.Name = name;
            fave.Default = isDefault;
            fave.UserID = HubSecurity.getLoggedInUserID();
            //If this entry is set to be default, make sure all current ones get default put to false
            if (isDefault)
            {
                List<int> ids = (from f in db.ScheduleFavorites where f.UserID == fave.UserID select f.ID).ToList();
                foreach (int id in ids)
                {
                    ScheduleFavorite f = db.ScheduleFavorites.Find(id);
                    f.Default = false;
                    db.Entry(f).State = EntityState.Modified;
                }
            }
            db.ScheduleFavorites.Add(fave);           
            db.SaveChanges();
            return fave.ID;
        }

        public static void CreatePatient(PatientLogModel db, PatientLog patient)
        {
            patient.DateCreated = DateTime.Now;
            //patientLog.LastUpdated = DateTime.Now;
            
            if (patient.Physician == "" || patient.Physician == null)
            {
                patient.Physician = "Unassigned";
            }
            if (patient.PCP_Practice == "" || patient.PCP_Practice == null)
            {
                patient.PCP_Practice = "No PCP";
            }
            if (patient.ServiceType == "" || patient.ServiceType == null)
            {
                patient.ServiceType = "Assigned";
            }
            db.PatientLogs.Add(patient);
            db.SaveChanges();
        }

        public static void SavePatient(PatientLogModel db, PatientLog patient)
        {
            patient.LastUpdated = DateTime.Now;
            db.Entry(patient).State = EntityState.Modified;


            //db.Audits.Add(new Audit
            //{
            //    Action = "Update",
            //    FunctionUsed = "SavePatient(PatientLog)",
            //    UserID = HubSecurity.getLoggedInUserID(),
            //    TargetIDs = patient.ID.ToString(),
            //    Controller = getController(),
            //    Page = getPage(),
            //    TimeStamp = DateTime.Now
            //});

            db.SaveChanges();
        }

        public static void SavePatients(PatientLogModel db, PatientLog[] patient)
        {
            //string ids = "";
            foreach (PatientLog pat in patient)
            {
                pat.LastUpdated = DateTime.Now;
                db.Entry(pat).State = EntityState.Modified;
                //ids += pat.ID.ToString() + ",";
            }
            //db.Audits.Add(new Audit
            //{
            //    Action = "Update",
            //    FunctionUsed = "SavePatients(PatientLog[])",
            //    UserID = HubSecurity.getLoggedInUserID(),
            //    TargetIDs = ids.Substring(0, ids.Length - 1),
            //    Controller = getController(),
            //    Page = getPage(),
            //    TimeStamp = DateTime.Now
            //});
            db.SaveChanges();
        }

        public static void CopyPatient(PatientLogModel db, int id)
        {
            PatientLog patient = db.PatientLogs.Find(id);

            //Reset values for copied record
            patient.ServiceDate = DateTime.Parse(DateTime.Now.ToShortDateString());
            patient.DateCreated = DateTime.Now;
            patient.LastUpdated = null;
            patient.Notes = null;

            //db.Audits.Add(new Audit
            //{
            //    Action = "Copy",
            //    FunctionUsed = "CopyPatient(int)",
            //    UserID = HubSecurity.getLoggedInUserID(),
            //    TargetIDs = id.ToString(),
            //    Controller = getController(),
            //    Page = getPage(),
            //    TimeStamp = DateTime.Now
            //});

            db.PatientLogs.Add(patient);
            db.SaveChanges();

        }

        public static void CopyPatient(PatientLogModel db, PatientLog patient)
        {
            //Reset values for copied record
            patient.ServiceDate = DateTime.Parse(DateTime.Now.ToShortDateString());
            patient.DateCreated = DateTime.Now;
            patient.LastUpdated = null;
            patient.Notes = null;

            db.PatientLogs.Add(patient);
            db.SaveChanges();
        }

        public static void CopyPatient(PatientLogModel db, string id)
        {
            PatientLog patient = db.PatientLogs.Find(Convert.ToInt32(id));

            //Reset values for copied record
            patient.ServiceDate = DateTime.Parse(DateTime.Now.ToShortDateString());
            patient.DateCreated = DateTime.Now;
            patient.LastUpdated = null;
            patient.Notes = null;

            db.PatientLogs.Add(patient);
            db.SaveChanges();
        }

        public static void CopyPatients(PatientLogModel db, int[] id)
        {
            for (int i = 0; i < id.Length; i++)
            {
                PatientLog patient = db.PatientLogs.Find(id[i]);

                //Reset values for copied record
                patient.ServiceDate = DateTime.Parse(DateTime.Now.ToShortDateString());
                patient.DateCreated = DateTime.Now;
                patient.LastUpdated = null;
                patient.Notes = null;

                db.PatientLogs.Add(patient);
            }
            db.SaveChanges();
        }

        public static void CopyPatients(PatientLogModel db, PatientLog[] patient)
        {
            foreach (PatientLog pat in patient)
            {
                //Reset values for copied record
                pat.ServiceDate = DateTime.Parse(DateTime.Now.ToShortDateString());
                pat.DateCreated = DateTime.Now;
                pat.LastUpdated = null;
                pat.Notes = null;

                db.PatientLogs.Add(pat);
            }
            db.SaveChanges();
        }

        public static void CopyPatients(PatientLogModel db, string id)
        {
            string[] split = id.Split(',');
            for (int i = 0; i < split.Length; i++)
            {
                PatientLog patient = db.PatientLogs.Find(Convert.ToInt32(split[i]));

                //Reset values for copied record
                patient.ServiceDate = DateTime.Parse(DateTime.Now.ToShortDateString());
                patient.DateCreated = DateTime.Now;
                patient.LastUpdated = null;
                patient.Notes = null;

                db.PatientLogs.Add(patient);
            }
            db.SaveChanges();
        }

        public static void CopyPatients(PatientLogModel db, string[] id)
        {
            for (int i = 0; i < id.Length; i++)
            {
                PatientLog patient = db.PatientLogs.Find(Convert.ToInt32(id[i]));

                //Reset values for copied record
                patient.ServiceDate = DateTime.Parse(DateTime.Now.ToShortDateString());
                patient.DateCreated = DateTime.Now;
                patient.LastUpdated = null;
                patient.Notes = null;

                db.PatientLogs.Add(patient);
            }
            db.SaveChanges();
        }

        public static void DeletePatient(PatientLogModel db, int id)
        {
            PatientLog patient = db.PatientLogs.Find(id);
            db.PatientLogTmps.Add(CreateTmpPatient(patient));
            db.PatientLogs.Remove(patient);
            db.SaveChanges();
        }

        public static void DeletePatient(PatientLogModel db, PatientLog patient)
        {
            db.PatientLogTmps.Add(CreateTmpPatient(patient));
            db.PatientLogs.Remove(patient);
            db.SaveChanges();
        }

        public static void DeletePatient(PatientLogModel db, string id)
        {
            PatientLog patient = db.PatientLogs.Find(Convert.ToInt32(id));
            db.PatientLogTmps.Add(CreateTmpPatient(patient));
            db.PatientLogs.Remove(patient);
            db.SaveChanges();
        }

        public static void DeletePatients(PatientLogModel db, int[] id)
        {
            for (int i = 0; i < id.Length; i++)
            {
                PatientLog patient = db.PatientLogs.Find(id[i]);
                db.PatientLogTmps.Add(CreateTmpPatient(patient));
                db.PatientLogs.Remove(patient);
            }
            db.SaveChanges();
        }

        public static void DeletePatients(PatientLogModel db, PatientLog[] patient)
        {
            foreach (PatientLog pat in patient)
            {
                db.PatientLogTmps.Add(CreateTmpPatient(pat));
                db.PatientLogs.Remove(pat);
                db.SaveChanges();
            }
            db.SaveChanges();
        }

        public static void DeletePatients(PatientLogModel db, string id)
        {
            string[] split = id.Split(',');
            for (int i = 0; i < split.Length; i++)
            {
                PatientLog patient = db.PatientLogs.Find(Convert.ToInt32(split[i]));
                db.PatientLogTmps.Add(CreateTmpPatient(patient));
                db.PatientLogs.Remove(patient);
            }
            db.SaveChanges();
        }

        public static void DeletePatients(PatientLogModel db, string[] id)
        {
            for (int i = 0; i < id.Length; i++)
            {
                PatientLog patient = db.PatientLogs.Find(Convert.ToInt32(id[i]));
                db.PatientLogTmps.Add(CreateTmpPatient(patient));
                db.PatientLogs.Remove(patient);
            }
            db.SaveChanges();
        }

        public static string DeleteFavorite(PatientLogModel db, string id)
        {
            int newID = Convert.ToInt32(id);
            string name;
            ScheduleFavorite f = db.ScheduleFavorites.Find(newID);
            name = f.Name;
            db.ScheduleFavorites.Remove(f);
            db.SaveChanges();
            return name;
        }

        private static PatientLogTmp CreateTmpPatient(PatientLog patient)
        {
            PatientLogTmp pt = new PatientLogTmp();
            pt.AIMSComments = patient.AIMSComments;
            pt.Comments = patient.Comments;
            pt.DateCreated = patient.DateCreated;
            pt.DOB = patient.DOB;
            pt.FaceSheet = patient.FaceSheet;
            pt.Gender = patient.Gender;
            pt.Hospital = patient.Hospital;
            pt.LastUpdated = patient.LastUpdated;
            pt.MRN_FIN = patient.MRN_FIN;
            pt.Notes = patient.Notes;
            pt.PatientClass = patient.PatientClass;
            pt.PatientName = patient.PatientName;
            pt.PCP_Practice = patient.PCP_Practice;
            pt.Physician = patient.Physician;
            pt.PLRecord = patient.ID;
            pt.RoomNo = patient.RoomNo;
            pt.ServiceDate = patient.ServiceDate;
            pt.ServiceType = patient.ServiceType;
            return pt;
        }

        public static void CreateBillingEntry(PatientLogModel db, int id)
        {
            Billing bill = new Billing();
            bill.PLRecord = id;
            db.Billings.Add(bill);
            db.SaveChanges();
        }

        public static void SavePreference(PatientLogModel db, string controller, string viewModel, string filterName, string filterValue)
        {
            string userID = HubSecurity.getLoggedInUserID();
            UserPreference preference = (from u in db.UserPreferences where u.Controller == controller && u.ViewModel == viewModel && u.FilterName == filterName && u.UserID == userID select u).FirstOrDefault();

            //If user preference query returns a value, update it. If not, create a new entry in the db
            if (preference != null)
            {
                preference.FilterValue = filterValue;
                db.Entry(preference).State = EntityState.Modified;
            }
            else
            {
                preference.Controller = controller;
                preference.ViewModel = viewModel;
                preference.FilterName = filterName;
                preference.FilterValue = filterValue;
                preference.UserID = userID;
                db.UserPreferences.Add(preference);
            }
            //Decided to leave this outside of the function so that it is only called once, supposed to be more efficient
            //db.SaveChanges();
        }

        public static void SavePreferences(PatientLogModel db, string controller, string viewModel, Dictionary<string, string> values)
        {           

            if (values.Count > 0)
            {
                string userID = HubSecurity.getLoggedInUserID();
                foreach (KeyValuePair<string,string> entry in values)
                {
                    UserPreference preference = (from u in db.UserPreferences where u.Controller == controller && u.ViewModel == viewModel && u.FilterName == entry.Key && u.UserID == userID select u).FirstOrDefault();

                    //If user preference query returns a value, update it. If not, create a new entry in the db
                    if (preference != null)
                    {
                        preference.FilterValue = entry.Value;
                        db.Entry(preference).State = EntityState.Modified;
                    }
                    else
                    {
                        preference = new UserPreference();
                        preference.Controller = controller;
                        preference.ViewModel = viewModel;
                        preference.FilterName = entry.Key;
                        preference.FilterValue = entry.Value;
                        preference.UserID = userID;
                        db.UserPreferences.Add(preference);
                    }
                }
                db.SaveChanges();
            }
        }

        public static void SavePractice(PatientLogModel db, ReferringPractice prac)
        {
            ReferringPractice practice = (from r in db.ReferringPractices where r.PracID == prac.PracID select r).FirstOrDefault();

            //If user preference query returns a value, update it. If not, create a new entry in the db
            if (practice != null)
            {
                db.Entry(practice).State = EntityState.Modified;
            }
            else
            {
                db.ReferringPractices.Add(prac);
            }
            db.SaveChanges();
        }

        //Saves specialties 
        public static void SaveSpecialties(PatientLogModel db, List<RefPracSpecialty> specialties)
        {
            //Zero ID indicates this has never been saved to the database before
            if (specialties[0].ID == 0)
            {
                foreach (RefPracSpecialty spec in specialties)
                {
                    db.RefPracSpecialties.Add(spec);
                }
            }
            else
            {
                foreach (RefPracSpecialty spec in specialties)
                {
                    RefPracSpecialty oldSpec = (from s in db.RefPracSpecialties where s.ID == spec.ID select s).Single();
                    oldSpec.FirstChoice = spec.FirstChoice;
                    oldSpec.Backup = spec.Backup;
                    oldSpec.Comments = spec.Comments;
                    db.Entry(oldSpec).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
        }

        //Saves specialties to all of admins practices
        public static void SaveSpecialties(PatientLogModel db, List<RefPracSpecialty> specialties, string practiceAdmin)
        {
            List<int> ids = new List<int>();
            ids = (from a in db.RefPracAdmins where a.UserID == practiceAdmin select a.PracID).ToList();

            foreach (int id in ids)
            {
                bool exists = (from s in db.RefPracSpecialties where s.PracID == id select s.ID).Any();

                if (exists)
                {
                    foreach (RefPracSpecialty spec in specialties)
                    {
                        RefPracSpecialty oldSpec = (from s in db.RefPracSpecialties where s.PracID == id && s.Specialty == spec.Specialty select s).Single();
                        oldSpec.FirstChoice = spec.FirstChoice;
                        oldSpec.Backup = spec.Backup;
                        oldSpec.Comments = spec.Comments;
                        db.Entry(oldSpec).State = EntityState.Modified;
                    }
                }
                else
                {
                    foreach (RefPracSpecialty spec in specialties)
                    {
                        spec.PracID = id;
                        db.RefPracSpecialties.Add(spec);
                    }
                }
            }
            db.SaveChanges();
        }

        //public static void SchedulePushToDisplay(PatientLogModel db)
        //{
        //    var t = DataCollections.getSchedule()
        //}


        public static void DeleteSchedule(PatientLogModel db, string id)
        {
            int newID = Convert.ToInt32(id);
            Schedule sch = DataCollections.getSchedule(db, id);
            db.Schedules.Remove(sch);
            db.SaveChanges();
        }

        public static void DeleteScheduleWorkArea(PatientLogModel db, string id)
        {
            int newID = Convert.ToInt32(id);
            ScheduleWorkArea sch = DataCollections.getScheduleWorkArea(db, id);
            db.ScheduleWorkAreas.Remove(sch);
            db.SaveChanges();
        }

        public static void CreateScheduleWorkArea(PatientLogModel db, ScheduleWorkArea sch)
        {
            db.ScheduleWorkAreas.Add(sch);
            db.SaveChanges();
        }

        public static void CreateScheduleWorkArea(PatientLogModel db, List<ScheduleWorkArea> sch)
        {
            foreach (ScheduleWorkArea s in sch)
            {
                db.ScheduleWorkAreas.Add(s);
            }           
            db.SaveChanges();
        }

        public static int MassEditSchedule(PatientLogModel db, List<string> users, List<string> hosps, List<string> types, DateTime start, DateTime end, string criteria, string change)
        {
            DateTime newStart = start.Date + new TimeSpan(0, 0, 0);
            DateTime newEnd = end.Date + new TimeSpan(23, 59, 59);
            int count;

            IQueryable<ScheduleWorkArea> toEdit = from w in db.ScheduleWorkAreas
                                                    where w.StartTime >= newStart && w.StartTime <= newEnd
                                                    select w;

            if (users.Any())
            {
                toEdit = toEdit.Where(w => users.Contains(w.UserID));
            }
            if (hosps.Any())
            {
                toEdit = toEdit.Where(w => hosps.Contains(w.HospitalShortName));
            }
            if (types.Any())
            {
                toEdit = toEdit.Where(w => types.Contains(w.ScheduleType));
            }

            count = toEdit.Count();
            foreach (ScheduleWorkArea sch in toEdit)
            {
                string shortHand = sch.ScheduleType.Substring(sch.ScheduleType.IndexOf("(") + 1);
                shortHand = shortHand.Substring(0, shortHand.Count() - 1);
                switch (criteria)
                {
                    case "Hospital":
                        sch.HospitalShortName = change;
                        sch.Title = sch.HospitalShortName + "-" + shortHand + "-" + sch.UserID.Substring(1);
                        break;
                    case "StartTime":
                        sch.StartTime = DateTime.Parse(change);
                        break;
                    case "EndTime":
                        sch.EndTime = DateTime.Parse(change);
                        break;
                    case "ScheduleType":
                        sch.ScheduleType = change;
                        sch.Title = sch.HospitalShortName + "-" + shortHand + "-" + sch.UserID.Substring(1);
                        break;
                    case "UserID":
                        sch.UserID = change;
                        sch.Title = sch.HospitalShortName + "-" + shortHand + "-" + sch.UserID.Substring(1);
                        break;
                }

                db.Entry(sch).State = EntityState.Modified;
            }
            db.SaveChanges();
            return count;
        }

        public static int DeleteSchedule(PatientLogModel db, List<string> users, List<string> hosps, List<string> types, DateTime start, DateTime end)
        {
            DateTime newStart = start.Date + new TimeSpan(0, 0, 0);
            DateTime newEnd = end.Date + new TimeSpan(23, 59, 59);
            int count;

            IQueryable<ScheduleWorkArea> toDelete = from w in db.ScheduleWorkAreas
                                                    where w.StartTime >= newStart && w.StartTime <= newEnd
                                                    select w;

            if (users.Any())
            {
                toDelete = toDelete.Where(w => users.Contains(w.UserID));
            }
            if (hosps.Any())
            {
                toDelete = toDelete.Where(w => hosps.Contains(w.HospitalShortName));
            }
            if (types.Any())
            {
                toDelete = toDelete.Where(w => types.Contains(w.ScheduleType));
            }

            count = toDelete.Count();
            foreach (ScheduleWorkArea sch in toDelete)
            {
                db.ScheduleWorkAreas.Remove(sch);
            }
            db.SaveChanges();
            return count;
        }

        public static int FinalizeSchedule(PatientLogModel db, List<string> users, List<string> hosps, List<string> types, DateTime start, DateTime end)
        {
            DateTime newStart = start.Date + new TimeSpan(0, 0, 0);
            DateTime newEnd = end.Date + new TimeSpan(23, 59, 59);
            int count;

            IQueryable<Schedule> toDelete = from w in db.Schedules
                                                    where w.StartTime >= newStart && w.StartTime <= newEnd
                                                    select w;

            if (users.Any())
            {
                toDelete = toDelete.Where(w => users.Contains(w.UserID));
            }
            if (hosps.Any())
            {
                toDelete = toDelete.Where(w => hosps.Contains(w.HospitalShortName));
            }
            if (types.Any())
            {
                toDelete = toDelete.Where(w => types.Contains(w.ScheduleType));
            }

            //Delete all matching criteria from schedule
            foreach (Schedule sch in toDelete)
            {
                db.Schedules.Remove(sch);
            }
            db.SaveChanges();


            IQueryable<ScheduleWorkArea> toAdd = from w in db.ScheduleWorkAreas
                                                 where w.StartTime >= newStart && w.StartTime <= newEnd
                                                 select w;

            if (users.Any())
            {
                toAdd = toAdd.Where(w => users.Contains(w.UserID));
            }
            if (hosps.Any())
            {
                toAdd = toAdd.Where(w => hosps.Contains(w.HospitalShortName));
            }
            if (types.Any())
            {
                toAdd = toAdd.Where(w => types.Contains(w.ScheduleType));
            }

            //Add all matching ScheduleWorkArea schedules to the schedule table
            count = toAdd.Count();
            foreach (ScheduleWorkArea sch in toAdd)
            {
                Schedule newSchedule = new Schedule();

                newSchedule.Comments = sch.Comments;
                newSchedule.DateCreated = DateTime.Now;
                newSchedule.EndTime = sch.EndTime;
                newSchedule.HospitalShortName = sch.HospitalShortName;
                newSchedule.ScheduleType = sch.ScheduleType;
                newSchedule.StartTime = sch.StartTime;
                newSchedule.Title = sch.Title;
                newSchedule.UserID = sch.UserID;
                db.Schedules.Add(newSchedule);
            }
            db.SaveChanges();

            return count;
        }

        //public static void SetDailyRepeatSchedule(PatientLogModel db, string user, string hospital, string type, DateTime start, DateTime end, int interval, DateTime repeatTo)
        //{

        //}

        public static string SetDefaultFavorite(PatientLogModel db, string id)
        {
            string userid = HubSecurity.getLoggedInUserID();
            string ret;
            int newID = Convert.ToInt32(id);
            List<ScheduleFavorite> query = (from f in db.ScheduleFavorites where f.UserID == userid select f).ToList();
            foreach (ScheduleFavorite f in query)
            {
                f.Default = false;
                db.Entry(f).State = EntityState.Modified;
            }
            ScheduleFavorite newD = db.ScheduleFavorites.Find(newID);
            ret = newD.Name;
            newD.Default = true;
            db.Entry(newD).State = EntityState.Modified;
            db.SaveChanges();
            return ret;
        }
    }
}