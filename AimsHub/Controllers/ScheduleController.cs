using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AimsHub.ViewModels;
using AimsHub.DAL;
using AimsHub.Models;
using DayPilot.Web.Mvc.Events.Calendar;
using DayPilot.Web.Mvc;
using AimsHub.Extensions;
using DayPilot.Web.Mvc.Enums;
using DayPilot.Web.Mvc.Data;
using System.Data;
using AimsHub.Security;
using System.Text;
using DayPilot.Web.Mvc.Events.Month;
using static AimsHub.Extensions.EventManager;
using System.Globalization;

namespace AimsHub.Controllers
{

    public class ScheduleController : Controller
    {
        private PatientLogModel db = new PatientLogModel();

        public ActionResult DisplayArea()
        {
            ScheduleDisplayAreaViewModel viewM = new ScheduleDisplayAreaViewModel();
            string[] split;
            viewM.UserList = DataCollections.getScheduleUsers(db);
            viewM.HospitalList = DataCollections.getAllLocationsByType(db, false);
            viewM.ScheduleList = DataCollections.getScheduleTypes(db);
            //viewM.ScheduleRoundingList = DataCollections.getScheduleRoundingTypes(db);
            viewM.FavoriteList = DataCollections.getFavorites(db);
            //viewM.ViewList = DataCollections.getMonthWeekDay();

            //Determines hospital and NH hidden lists without hitting the DB again
            byte[] ascii;
            byte[] asciiOld = Encoding.ASCII.GetBytes("A");
            bool doingHosp = true;
            bool doingNH = false;
            foreach (SelectListItem hosp in viewM.HospitalList)
            {
                ascii = Encoding.ASCII.GetBytes(hosp.Value.Substring(0));
                if (ascii[0] < asciiOld[0])
                {
                    if (doingHosp)
                    {
                        doingHosp = false;
                        doingNH = true;
                    }
                    else if (doingNH)
                    {
                        doingNH = false;
                    }
                }
                if (doingHosp)
                {
                    viewM.hidHospitalList += hosp.Value + ",";
                }
                if (doingNH)
                {
                    viewM.hidNHList += hosp.Value + ",";
                }
                asciiOld[0] = ascii[0];
            }
            viewM.hidHospitalList = viewM.hidHospitalList.Substring(0, viewM.hidHospitalList.Count() - 1);
            viewM.hidNHList = viewM.hidNHList.Substring(0, viewM.hidNHList.Count() - 1);

            if (Session["scheduleUsers"] != null)
            {
                //split = (List<string>)Session["scheduleUsers"].ToString().Split(new char[] { ',' });
                viewM.SelectedUsers = (List<string>)Session["scheduleUsers"];
            }
            else
            {
                viewM.SelectedUsers.Add(HubSecurity.getLoggedInUserID());
            }
            if (Session["scheduleHosps"] != null)
            {
                //split = Session["scheduleHosps"].ToString().Split(new char[] { ',' });
                viewM.SelectedHospitals = (List<string>)Session["scheduleHospitals"];
            }
            if (Session["scheduleTypes"] != null)
            {
                //split = Session["scheduleTypes"].ToString().Split(new char[] { ',' });
                viewM.SelectedSchedules = (List<string>)Session["scheduleTypes"];
            }
            if (Session["scheduleDate"] != null)
            {
                viewM.SelectedDate = DateTime.Parse(Session["scheduleDate"].ToString());
            }
            else
            {
                viewM.SelectedDate = DateTime.Now;
            }
            //if (Session["scheduleRounding"] != null)
            //{
            //    viewM.ShowRounding = (bool)Session["scheduleRounding"];
            //}
            //viewM.SelectedView = "Month";
            viewM.DateString = viewM.SelectedDate.ToString("MMMM") + ", " + viewM.SelectedDate.Year;

            Session["scheduleUsers"] = viewM.SelectedUsers;
            Session["scheduleHospitals"] = viewM.SelectedHospitals;
            Session["scheduleTypes"] = viewM.SelectedSchedules;
            Session["scheduleDate"] = viewM.SelectedDate;
            //Session["scheduleRounding"] = viewM.ShowRounding;
            ViewData["UserList"] = viewM.UserList;
            ViewData["HospitalList"] = viewM.HospitalList;
            ViewData["ScheduleList"] = viewM.ScheduleList;
            //ViewData["ScheduleRoundingList"] = viewM.ScheduleRoundingList;
            if (Session["ScheduleTypeReference"] == null)
            {
                Session["ScheduleTypeReference"] = DataCollections.getScheduleTypeReference(db); //Only loads schedule types once to prevent the event tags from needing DB access 
            }

            return View(viewM);
        }

        [HttpPost]
        public ActionResult DisplayArea(ScheduleDisplayAreaViewModel viewM)
        {
            ScheduleDisplayAreaViewModel returnM = new ScheduleDisplayAreaViewModel();
            string[] split;
            if (ViewData["UserList"] != null)
            {
                returnM.UserList = (SelectList)ViewData["UserList"];
            }
            else
            {
                returnM.UserList = DataCollections.getScheduleUsers(db);
            }
            if (ViewData["HospitalList"] != null)
            {
                returnM.HospitalList = (SelectList)ViewData["HospitalList"];
            }
            else
            {
                returnM.HospitalList = DataCollections.getAllLocationsByType(db, false);
            }
            if (ViewData["ScheduleList"] != null)
            {
                returnM.ScheduleList = (SelectList)ViewData["ScheduleList"];
            }
            else
            {
                returnM.ScheduleList = DataCollections.getScheduleTypes(db);
            }
            //if (ViewData["ScheduleRoundingList"] != null)
            //{
            //    returnM.ScheduleRoundingList = (SelectList)ViewData["ScheduleRoundingList"];
            //}
            //else
            //{
            //    returnM.ScheduleRoundingList = DataCollections.getScheduleRoundingTypes(db);
            //}

            if (viewM.FavoritePostback != null)
            {
                returnM.FavoriteSelected = viewM.FavoriteSelected;
                //Make sure value isn't null to prevent database error
                if (returnM.FavoriteSelected != null)
                {
                    ScheduleFavorite fave = DataCollections.getFavorite(db, viewM.FavoriteSelected);
                    if (fave.Users != null && fave.Users != "")
                    {
                        split = fave.Users.Split(new char[] { ',' });
                        returnM.SelectedUsers = split.ToList<string>();
                    }
                    if (fave.Hospital != null && fave.Hospital != "")
                    {
                        split = fave.Hospital.Split(new char[] { ',' });
                        returnM.SelectedHospitals = split.ToList<string>();
                    }
                    if (fave.Types != null && fave.Types != "")
                    {
                        split = fave.Types.Split(new char[] { ',' });
                        returnM.SelectedSchedules = split.ToList<string>();
                    }
                    Session["scheduleUsers"] = returnM.SelectedUsers;
                    Session["scheduleHospitals"] = returnM.SelectedHospitals;
                    Session["scheduleTypes"] = returnM.SelectedSchedules;
                }
                else
                {
                    if (viewM.hidUsers != null && viewM.hidUsers != "")
                    {
                        split = viewM.hidUsers.Split(new char[] { ',' });
                        returnM.SelectedUsers = split.ToList<string>();
                    }
                    if (viewM.hidHospitals != null && viewM.hidHospitals != "")
                    {
                        split = viewM.hidHospitals.Split(new char[] { ',' });
                        returnM.SelectedHospitals = split.ToList<string>();
                    }
                    if (viewM.hidTypes != null && viewM.hidTypes != "")
                    {
                        split = viewM.hidTypes.Split(new char[] { ',' });
                        returnM.SelectedSchedules = split.ToList<string>();
                    }

                    Session["scheduleUsers"] = returnM.SelectedUsers;
                    Session["scheduleHospitals"] = returnM.SelectedHospitals;
                    Session["scheduleTypes"] = returnM.SelectedSchedules;
                }
            }
            else
            {
                if (viewM.hidUsers != null && viewM.hidUsers != "")
                {
                    split = viewM.hidUsers.Split(new char[] { ',' });
                    returnM.SelectedUsers = split.ToList<string>();
                }
                if (viewM.hidHospitals != null && viewM.hidHospitals != "")
                {
                    split = viewM.hidHospitals.Split(new char[] { ',' });
                    returnM.SelectedHospitals = split.ToList<string>();
                }
                if (viewM.hidTypes != null && viewM.hidTypes != "")
                {
                    split = viewM.hidTypes.Split(new char[] { ',' });
                    returnM.SelectedSchedules = split.ToList<string>();
                }
            }
            //Check to see if rounding schedule was selected
            //if (viewM.ShowRounding)
            //{
            //    //If selected, need to make sure if this was just selected or if it already was by checking session
            //    //This is to prevent the wrong types being saved into session and messing up queries
            //    //If just selected, pass null to schedule type so that all are selected
            //    if ((bool)Session["scheduleRounding"])
            //    {
            //        if (viewM.hidTypes != null && viewM.hidTypes != "")
            //        {
            //            split = viewM.hidTypes.Split(new char[] { ',' });
            //            returnM.SelectedSchedules = split.ToList<string>();
            //        }
            //    }
            //    else
            //    {
            //        //Leave null
            //    }
            //}
            //else
            //{
            //    //If true, this means we are leaving rounding mode
            //    if ((bool)Session["scheduleRounding"])
            //    {
            //        //Leave null
            //    }
            //    else
            //    {
            //        if (viewM.hidTypes != null && viewM.hidTypes != "")
            //        {
            //            split = viewM.hidTypes.Split(new char[] { ',' });
            //            returnM.SelectedSchedules = split.ToList<string>();
            //        }
            //    }
            //}
            //returnM.ShowRounding = viewM.ShowRounding;

            //Add schedule favorite if necessary
            if (viewM.FavoriteAddPostback == "yes")
            {
                int retID;
                retID = DataSubmissions.CreateFavorite(db, viewM.hidUsers, viewM.hidHospitals, viewM.hidTypes, viewM.FavoriteNewName, viewM.FavoriteMakeDefault);
                returnM.FavoriteSelected = retID.ToString(); 
            }

            //Set date
            if (viewM.PreviousButton == "clicked")
            {
                returnM.SelectedDate = viewM.SelectedDate.AddMonths(-1);
            }
            else if (viewM.NextButton == "clicked")
            {
                returnM.SelectedDate = viewM.SelectedDate.AddMonths(1);
            }
            else
            {
                returnM.SelectedDate = viewM.SelectedDate;
            }

            returnM.DateString = returnM.SelectedDate.ToString("MMMM") + ", " + returnM.SelectedDate.Year;
            returnM.FavoriteList = DataCollections.getFavorites(db);
            returnM.hidHospitalList = viewM.hidHospitalList;
            returnM.hidNHList = viewM.hidNHList;

            Session["scheduleUsers"] = returnM.SelectedUsers;
            Session["scheduleHospitals"] = returnM.SelectedHospitals;
            Session["scheduleTypes"] = returnM.SelectedSchedules;
            Session["scheduleDate"] = returnM.SelectedDate;
            //Session["scheduleRounding"] = returnM.ShowRounding;
            ViewData["UserList"] = returnM.UserList;
            ViewData["HospitalList"] = returnM.HospitalList;
            ViewData["ScheduleList"] = returnM.ScheduleList;
            //ViewData["ScheduleRoundingList"] = returnM.ScheduleRoundingList;
            if (Session["ScheduleTypeReference"] == null)
            {
                Session["ScheduleTypeReference"] = DataCollections.getScheduleTypeReference(db);
            }


            return View(returnM);
        }

        public ActionResult WorkArea()
        {
            ScheduleWorkAreaViewModel viewM = new ScheduleWorkAreaViewModel();
            //string[] split;
            viewM.UserList = DataCollections.getScheduleUsers(db);
            viewM.HospitalList = DataCollections.getAllLocationsByType(db, false);
            viewM.ScheduleList = DataCollections.getScheduleTypes(db);
            //viewM.ScheduleRoundingList = DataCollections.getScheduleRoundingTypes(db);
            viewM.FavoriteList = DataCollections.getFavorites(db);
            viewM.MassEditCriteria = DataCollections.getMassEditCriteria();
            viewM.Hours = DataCollections.getHours();
            //viewM.ViewList = DataCollections.getMonthWeekDay();

            //Determines hospital and NH hidden lists without hitting the DB again
            byte[] ascii;
            byte[] asciiOld = Encoding.ASCII.GetBytes("A");
            bool doingHosp = true;
            bool doingNH = false;
            //Loops through locations, first identifying hospitals. When alphabet resets, switches to NH, and finally stops once alphabet resets again.
            foreach (SelectListItem hosp in viewM.HospitalList)
            {
                ascii = Encoding.ASCII.GetBytes(hosp.Value.Substring(0));
                if (ascii[0] < asciiOld[0])
                {
                    if (doingHosp)
                    {
                        doingHosp = false;
                        doingNH = true;
                    }
                    else if (doingNH)
                    {
                        doingNH = false;
                    }
                }
                if (doingHosp)
                {
                    viewM.hidHospitalList += hosp.Value + ",";
                }
                if (doingNH)
                {
                    viewM.hidNHList += hosp.Value + ",";
                }
                asciiOld[0] = ascii[0];
            }
            viewM.hidHospitalList = viewM.hidHospitalList.Substring(0, viewM.hidHospitalList.Count() - 1);
            viewM.hidNHList = viewM.hidNHList.Substring(0, viewM.hidNHList.Count() - 1);

            if (Session["scheduleUsers"] != null)
            {
                //split = (List<string>)Session["scheduleUsers"].ToString().Split(new char[] { ',' });
                viewM.SelectedUsers = (List<string>)Session["scheduleUsers"];
            }
            else
            {
                viewM.SelectedUsers.Add(HubSecurity.getLoggedInUserID());
            }
            if (Session["scheduleHosps"] != null)
            {
                //split = Session["scheduleHosps"].ToString().Split(new char[] { ',' });
                viewM.SelectedHospitals = (List<string>)Session["scheduleHospitals"];
            }
            if (Session["scheduleTypes"] != null)
            {
                //split = Session["scheduleTypes"].ToString().Split(new char[] { ',' });
                viewM.SelectedSchedules = (List<string>)Session["scheduleTypes"];
            }
            if (Session["scheduleDate"] != null)
            {
                viewM.SelectedDate = DateTime.Parse(Session["scheduleDate"].ToString());
            }
            else
            {
                viewM.SelectedDate = DateTime.Now;
            }
            //viewM.SelectedView = "Month";
            viewM.DateString = viewM.SelectedDate.ToString("MMMM") + ", " + viewM.SelectedDate.Year;
            viewM.OperationStartDate = DateTime.Parse(viewM.SelectedDate.Month.ToString() + "/1/" + viewM.SelectedDate.Year.ToString());
            viewM.OperationEndDate = DateTime.Parse(viewM.SelectedDate.Month.ToString() + "/" +  DateTime.DaysInMonth(viewM.SelectedDate.Year, viewM.SelectedDate.Month) + "/" + viewM.SelectedDate.Year.ToString());

            Session["scheduleUsers"] = viewM.SelectedUsers;
            Session["scheduleHospitals"] = viewM.SelectedHospitals;
            Session["scheduleTypes"] = viewM.SelectedSchedules;
            Session["scheduleDate"] = viewM.SelectedDate;
            ViewData["UserList"] = viewM.UserList;
            ViewData["HospitalList"] = viewM.HospitalList;
            ViewData["ScheduleList"] = viewM.ScheduleList;
            if (Session["ScheduleTypeReference"] == null)
            {
                Session["ScheduleTypeReference"] = DataCollections.getScheduleTypeReference(db); //Only loads schedule types once to prevent the event tags from needing DB access 
            }

            return View(viewM);
        }

        [HttpPost]
        public ActionResult WorkArea(ScheduleWorkAreaViewModel viewM)
        {
            ScheduleWorkAreaViewModel returnM = new ScheduleWorkAreaViewModel();
            string[] split;
            if (ViewData["UserList"] != null)
            {
                returnM.UserList = (SelectList)ViewData["UserList"];
            }
            else
            {
                returnM.UserList = DataCollections.getScheduleUsers(db);
            }
            if (ViewData["HospitalList"] != null)
            {
                returnM.HospitalList = (SelectList)ViewData["HospitalList"];
            }
            else
            {
                returnM.HospitalList = DataCollections.getAllLocationsByType(db, false);
            }
            if (ViewData["ScheduleList"] != null)
            {
                returnM.ScheduleList = (SelectList)ViewData["ScheduleList"];
            }
            else
            {
                returnM.ScheduleList = DataCollections.getScheduleTypes(db);
            }
            returnM.MassEditCriteria = DataCollections.getMassEditCriteria();

            //If drpFavorite caused postback, set selected favorite as search parameters
            if (viewM.FavoritePostback == "yes")
            {
                returnM.FavoriteSelected = viewM.FavoriteSelected;
                //Make sure value isn't null to prevent database error
                if (returnM.FavoriteSelected != null)
                {
                    ScheduleFavorite fave = DataCollections.getFavorite(db, viewM.FavoriteSelected);
                    if (fave.Users != null && fave.Users != "")
                    {
                        split = fave.Users.Split(new char[] { ',' });
                        returnM.SelectedUsers = split.ToList<string>();
                    }
                    if (fave.Hospital != null && fave.Hospital != "")
                    {
                        split = fave.Hospital.Split(new char[] { ',' });
                        returnM.SelectedHospitals = split.ToList<string>();
                    }
                    if (fave.Types != null && fave.Types != "")
                    {
                        split = fave.Types.Split(new char[] { ',' });
                        returnM.SelectedSchedules = split.ToList<string>();
                    }
                    Session["scheduleUsers"] = returnM.SelectedUsers;
                    Session["scheduleHospitals"] = returnM.SelectedHospitals;
                    Session["scheduleTypes"] = returnM.SelectedSchedules;
                }
                else
                {
                    if (viewM.hidUsers != null && viewM.hidUsers != "")
                    {
                        split = viewM.hidUsers.Split(new char[] { ',' });
                        returnM.SelectedUsers = split.ToList<string>();
                    }
                    if (viewM.hidHospitals != null && viewM.hidHospitals != "")
                    {
                        split = viewM.hidHospitals.Split(new char[] { ',' });
                        returnM.SelectedHospitals = split.ToList<string>();
                    }
                    if (viewM.hidTypes != null && viewM.hidTypes != "")
                    {
                        split = viewM.hidTypes.Split(new char[] { ',' });
                        returnM.SelectedSchedules = split.ToList<string>();
                    }

                    Session["scheduleUsers"] = returnM.SelectedUsers;
                    Session["scheduleHospitals"] = returnM.SelectedHospitals;
                    Session["scheduleTypes"] = returnM.SelectedSchedules;
                }
                
            }
            else
            {
                //Only update the search parameters and save to session if this is not an operation postback
                if (viewM.OperationAction != null)
                {
                    //Collect the parameters returned in the view
                    List<string> users = new List<string>();
                    List<string> hospitals = new List<string>();
                    List<string> types = new List<string>();

                    if (viewM.hidUsers != null && viewM.hidUsers != "")
                    {
                        split = viewM.hidUsers.Split(new char[] { ',' });
                        users = split.ToList<string>();
                    }
                    if (viewM.hidHospitals != null && viewM.hidHospitals != "")
                    {
                        split = viewM.hidHospitals.Split(new char[] { ',' });
                        hospitals = split.ToList<string>();
                    }
                    if (viewM.hidTypes != null && viewM.hidTypes != "")
                    {
                        split = viewM.hidTypes.Split(new char[] { ',' });
                        types = split.ToList<string>();
                    }

                    switch (viewM.OperationAction)
                    {
                        case "MassEdit":
                            returnM.OperationResult = DataSubmissions.MassEditSchedule(db, users, hospitals, types, viewM.OperationStartDate, viewM.OperationEndDate, viewM.OperationCriteria, viewM.OperationChangeTo);
                            break;
                        case "Delete":
                            returnM.OperationResult = DataSubmissions.DeleteSchedule(db, users, hospitals, types, viewM.OperationStartDate, viewM.OperationEndDate);
                            break;
                        case "Finalize":
                            returnM.OperationResult = DataSubmissions.FinalizeSchedule(db, users, hospitals, types, viewM.OperationStartDate, viewM.OperationEndDate);
                            break;
                        case "Tally":

                            break;
                    }

                    returnM.OperationCompleted = true;


                    //Revert selected filters to those stored in session
                    if (Session["scheduleUsers"] != null)
                    {
                        //split = (List<string>)Session["scheduleUsers"].ToString().Split(new char[] { ',' });
                        returnM.SelectedUsers = (List<string>)Session["scheduleUsers"];
                    }
                    else
                    {
                        returnM.SelectedUsers.Add(HubSecurity.getLoggedInUserID());
                    }
                    if (Session["scheduleHosps"] != null)
                    {
                        //split = Session["scheduleHosps"].ToString().Split(new char[] { ',' });
                        returnM.SelectedHospitals = (List<string>)Session["scheduleHospitals"];
                    }
                    if (Session["scheduleTypes"] != null)
                    {
                        //split = Session["scheduleTypes"].ToString().Split(new char[] { ',' });
                        returnM.SelectedSchedules = (List<string>)Session["scheduleTypes"];
                    }

                }
                else
                {
                    if (viewM.hidUsers != null && viewM.hidUsers != "")
                    {
                        split = viewM.hidUsers.Split(new char[] { ',' });
                        returnM.SelectedUsers = split.ToList<string>();
                    }
                    if (viewM.hidHospitals != null && viewM.hidHospitals != "")
                    {
                        split = viewM.hidHospitals.Split(new char[] { ',' });
                        returnM.SelectedHospitals = split.ToList<string>();
                    }
                    if (viewM.hidTypes != null && viewM.hidTypes != "")
                    {
                        split = viewM.hidTypes.Split(new char[] { ',' });
                        returnM.SelectedSchedules = split.ToList<string>();
                    }

                    Session["scheduleUsers"] = returnM.SelectedUsers;
                    Session["scheduleHospitals"] = returnM.SelectedHospitals;
                    Session["scheduleTypes"] = returnM.SelectedSchedules;
                }
            }

            //Add schedule favorite if necessary
            if (viewM.FavoriteAddPostback == "yes")
            {
                int retID;
                retID = DataSubmissions.CreateFavorite(db, viewM.hidUsers, viewM.hidHospitals, viewM.hidTypes, viewM.FavoriteNewName, viewM.FavoriteMakeDefault);
                returnM.FavoriteSelected = retID.ToString();
            }

            //Set date
            if (viewM.PreviousButton == "clicked")
            {
                returnM.SelectedDate = viewM.SelectedDate.AddMonths(-1);
            }
            else if (viewM.NextButton == "clicked")
            {
                returnM.SelectedDate = viewM.SelectedDate.AddMonths(1);
            }
            else
            {
                returnM.SelectedDate = viewM.SelectedDate;
            }
            
            returnM.DateString = returnM.SelectedDate.ToString("MMMM") + ", " + returnM.SelectedDate.Year;
            returnM.OperationStartDate = viewM.OperationStartDate;
            returnM.OperationEndDate = viewM.OperationEndDate;
            returnM.OperationAction = null;
            returnM.Hours = DataCollections.getHours();
            returnM.FavoriteList = DataCollections.getFavorites(db);
            returnM.hidHospitalList = viewM.hidHospitalList;
            returnM.hidNHList = viewM.hidNHList;

            
            Session["scheduleDate"] = returnM.SelectedDate;
            ViewData["UserList"] = returnM.UserList;
            ViewData["HospitalList"] = returnM.HospitalList;
            ViewData["ScheduleList"] = returnM.ScheduleList;
            if (Session["ScheduleTypeReference"] == null)
            {
                Session["ScheduleTypeReference"] = DataCollections.getScheduleTypeReference(db);
            }

            return View(returnM);
        }

        public ActionResult Create(string date, string user, string hosp)
        {
            ScheduleCreateViewModel viewM = new ScheduleCreateViewModel();
            viewM.Users = DataCollections.getScheduleUsers(db);
            viewM.Hospitals = DataCollections.getAllLocationsByType(db, false);
            viewM.Types = DataCollections.getScheduleTypes(db);
            viewM.Repeat = DataCollections.getRepeats();
            viewM.Hours = DataCollections.getHours();
            viewM.Minutes = DataCollections.getMinutes();
            viewM.Patterns = DataCollections.getPatterns(db);
            viewM.PatternUntilDate = DateTime.Now.AddDays(3);
            viewM.SecondHospitals = DataCollections.getAllLocationsByType(db, true);
            viewM.SecondHospital = "none";
            viewM.StartDate = DateTime.Parse(date);
            viewM.EndDate = viewM.StartDate;
            viewM.StartHour = "8";
            viewM.EndHour = "17";
            viewM.RepeatDate = viewM.StartDate.AddDays(2);
            viewM.RepeatHour = "0";
            viewM.RepeatMinute = "0";
            viewM.RepeatInterval = "1";
            viewM.DefaultType = "Day-(D)";
            viewM.DefaultHospital = hosp;
            viewM.DefaultUser = user;
            return View(viewM);
        }

        [HttpPost]
        public ActionResult Create(ScheduleCreateUpdate viewM)
        {
            try
            {
                //Check if pattern is toggled first, and perform pattern creation logic if true
                if (viewM.chkPattern == "Use Pattern")
                {
                    string[] pattern = viewM.Patterns.Split(new char[] { ',' });
                    string hosp = viewM.DefaultHospital;
                    string hosp2 = viewM.SecondHospital;
                    string user = viewM.DefaultUser;
                    int iteration;
                    int stop;
                    int dayIteration;
                    int daySpan;
                    string dayType;

                    DateTime repeater = DateTime.Parse(viewM.txtStartDate);
                    DateTime ender = DateTime.Parse(viewM.txtDoUntil);

                    iteration = 0;
                    dayIteration = 0;
                    stop = pattern.Length / 2;
                    daySpan = Convert.ToInt32(pattern[0]);
                    dayType = pattern[1];
                    

                    while (repeater <= ender)
                    {
                        ScheduleWorkArea sch = new ScheduleWorkArea();
                        DateTime start;
                        DateTime end;
                        DateTime start2 = new DateTime();
                        DateTime end2 = new DateTime();
                        if (viewM.SecondHospital == "none")
                        {
                            start = repeater.Date + new TimeSpan(8, 0, 0);
                            end = repeater.Date + new TimeSpan(17, 0, 0);
                        }
                        else
                        {
                            start = repeater.Date + new TimeSpan(8, 0, 0);
                            end = repeater.Date + new TimeSpan(13, 0, 0);
                            start2 = repeater.Date + new TimeSpan(13, 0, 0);
                            end2 = repeater.Date + new TimeSpan(17, 0, 0);
                        }
                        if (dayType == "on")
                        {
                            bool weekend = false;
                            if (repeater.DayOfWeek.ToString() == "Saturday" || repeater.DayOfWeek.ToString() == "Sunday")
                            {
                                weekend = true;
                            }
                            string type = weekend ? "Weekend-(W)" : "Day-(D)";

                            sch.DateCreated = DateTime.Now;
                            sch.EndTime = end;
                            sch.StartTime = start;
                            sch.UserID = user;
                            sch.HospitalShortName = hosp;
                            sch.ScheduleType = type;
                            int st = type.IndexOf("(");
                            int en = type.Length - 1;
                            string typeShort = type.Substring(st + 1).Replace(")", "");
                            sch.Title = hosp + "-" + typeShort + "-" + user.Substring(1);
                            db.ScheduleWorkAreas.Add(sch);
                            db.SaveChanges();

                            if (viewM.SecondHospital != "none")
                            {
                                sch.HospitalShortName = viewM.SecondHospital;
                                sch.StartTime = start2;
                                sch.EndTime = end2;
                                sch.Title = viewM.SecondHospital + "-" + typeShort + "-" + user.Substring(1);
                                db.ScheduleWorkAreas.Add(sch);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            DateTime offEnd = repeater.Date + new TimeSpan(17, 0, 0);

                            string type = "Scheduled Off-(O)";

                            sch.DateCreated = DateTime.Now;
                            sch.EndTime = offEnd;
                            sch.StartTime = start;
                            sch.UserID = user;
                            sch.HospitalShortName = hosp;
                            sch.ScheduleType = type;
                            int st = type.IndexOf("(");
                            int en = type.Length - 1;
                            string typeShort = type.Substring(st + 1).Replace(")", "");
                            sch.Title = hosp + "-" + typeShort + "-" + user.Substring(1);
                            db.ScheduleWorkAreas.Add(sch);
                            db.SaveChanges();
                        }
                        dayIteration += 1;
                        if (dayIteration == daySpan)
                        {
                            dayIteration = 0;
                            iteration += 2;
                            if (iteration >= pattern.Length)
                            {
                                iteration = 0;
                            }
                            daySpan = Convert.ToInt32(pattern[iteration]);
                            dayType = pattern[iteration + 1];
                        }
                        repeater = repeater.AddDays(1);
                    }
                }
                else
                {
                    //Create basic object first, every operation regardless of repeat selected will need these things done
                    ScheduleWorkArea schedule = new ScheduleWorkArea();
                    schedule.Comments = viewM.Comments;
                    schedule.DateCreated = DateTime.Now;
                    schedule.StartTime = DateTime.Parse(viewM.txtStartDate) + new TimeSpan(Convert.ToInt32(viewM.StartHour), Convert.ToInt32(viewM.StartMinute), 0);
                    schedule.EndTime = DateTime.Parse(viewM.txtEndDate) + new TimeSpan(Convert.ToInt32(viewM.EndHour), Convert.ToInt32(viewM.EndMinute), 0);
                    schedule.HospitalShortName = viewM.DefaultHospital;
                    schedule.ScheduleType = viewM.DefaultType;
                    schedule.Title = viewM.Title;
                    schedule.UserID = viewM.DefaultUser;

                    //Create basic entry
                    if (viewM.Repeat == "None")
                    {
                        DataSubmissions.CreateScheduleWorkArea(db, schedule);
                    }
                    else if (viewM.Repeat == "Daily")
                    {
                        //Create recurrent pattern string
                        string recurrentPattern;
                        TimeSpan t = new TimeSpan(Convert.ToInt32(viewM.RepeatHour), Convert.ToInt32(viewM.RepeatMinute), 0);
                        DateTime temp = DateTime.Now.Date + t;
                        string time = temp.ToString("HH:mm:ss tt", CultureInfo.InvariantCulture);
                        recurrentPattern = viewM.Repeat + "," + viewM.RepeatInterval + "," + viewM.txtRepeatDate + " " + time;

                        //Loop parameters
                        DateTime repeater = DateTime.Parse(viewM.txtStartDate);
                        repeater = repeater.AddDays(Convert.ToInt32(viewM.RepeatInterval)); //Set repeat to start at next interval since we are already saving the original prior to loop
                        DateTime ender = DateTime.Parse(viewM.txtRepeatDate);

                        //This list stores the recurrent IDs being generated so they can be appended after insert
                        List<string> IDs = new List<string>();

                        //Save original
                        schedule.RecurrentPattern = recurrentPattern;
                        db.ScheduleWorkAreas.Add(schedule);
                        db.SaveChanges();
                        IDs.Add(schedule.ID.ToString());

                        //Create additional
                        while (repeater <= ender)
                        {
                            ScheduleWorkArea nextSchedule = new ScheduleWorkArea();
                            nextSchedule = schedule; //Reuse most of original schedule object
                                                     //nextSchedule.ID = 0;
                            nextSchedule.StartTime = repeater + new TimeSpan(Convert.ToInt32(viewM.StartHour), Convert.ToInt32(viewM.EndHour), 0);
                            nextSchedule.EndTime = repeater + new TimeSpan(Convert.ToInt32(viewM.EndHour), Convert.ToInt32(viewM.EndMinute), 0);
                            db.ScheduleWorkAreas.Add(nextSchedule);
                            db.SaveChanges();
                            IDs.Add(nextSchedule.ID.ToString());

                            repeater = repeater.AddDays(Convert.ToInt32(viewM.RepeatInterval));
                        }

                        foreach (string id in IDs)
                        {
                            ScheduleWorkArea append = db.ScheduleWorkAreas.Find(Convert.ToInt32(id));
                            append.RecurrentIDs = String.Join(",", IDs.ToArray());
                            db.Entry(append).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else if (viewM.Repeat == "Weekly")
                    {
                        //Create recurrent pattern string
                        string recurrentPattern;
                        TimeSpan t = new TimeSpan(Convert.ToInt32(viewM.RepeatHour), Convert.ToInt32(viewM.RepeatMinute), 0);
                        DateTime temp = DateTime.Now.Date + t;
                        string time = temp.ToString("HH:mm:ss tt", CultureInfo.InvariantCulture);
                        //recurrentPattern = viewM.Repeat + "," + viewM.RepeatInterval + "," + viewM.txtRepeatDate + " " + time;

                        //Loop parameters
                        //schedule.RecurrentPattern = recurrentPattern;
                        DateTime repeater = DateTime.Parse(viewM.txtStartDate);
                        //repeater = repeater.AddDays(Convert.ToInt32(viewM.RepeatInterval)); //Set repeat to start at next interval since we are already saving the original prior to loop
                        DateTime ender = DateTime.Parse(viewM.txtRepeatDate);

                        //This list stores the recurrent IDs being generated so they can be appended after insert
                        List<string> IDs = new List<string>();
                        List<string> days = new List<string>();

                        //Collect the days of the week to be used
                        if (viewM.chkSunday == "Sun")
                        {
                            days.Add(viewM.chkSunday);
                        }
                        if (viewM.chkMonday == "Mon")
                        {
                            days.Add(viewM.chkMonday);
                        }
                        if (viewM.chkTuesday == "Tue")
                        {
                            days.Add(viewM.chkTuesday);
                        }
                        if (viewM.chkWednesday == "Wed")
                        {
                            days.Add(viewM.chkWednesday);
                        }
                        if (viewM.chkThursday == "Thu")
                        {
                            days.Add(viewM.chkThursday);
                        }
                        if (viewM.chkFriday == "Fri")
                        {
                            days.Add(viewM.chkFriday);
                        }
                        if (viewM.chkSaturday == "Sat")
                        {
                            days.Add(viewM.chkSaturday);
                        }
                        recurrentPattern = viewM.Repeat + "," + viewM.RepeatInterval + "," + string.Join(":", days) + "," + viewM.txtRepeatDate + " " + time;
                        schedule.RecurrentPattern = recurrentPattern;

                        int week = 1;
                        int threshold = 0;
                        if (viewM.RepeatInterval != "1")
                        {
                            threshold = (Convert.ToInt32(viewM.RepeatInterval) - 1) * 7;
                        }

                        //Create additional
                        while (repeater <= ender)
                        {
                            if (days.Contains(repeater.DayOfWeek.ToString().Substring(0,3)))
                            {
                                ScheduleWorkArea nextSchedule = new ScheduleWorkArea();
                                nextSchedule = schedule; //Reuse most of original schedule object
                                                         //nextSchedule.ID = 0;
                                nextSchedule.StartTime = repeater + new TimeSpan(Convert.ToInt32(viewM.StartHour), Convert.ToInt32(viewM.EndHour), 0);
                                nextSchedule.EndTime = repeater + new TimeSpan(Convert.ToInt32(viewM.EndHour), Convert.ToInt32(viewM.EndMinute), 0);
                                db.ScheduleWorkAreas.Add(nextSchedule);
                                db.SaveChanges();
                                IDs.Add(nextSchedule.ID.ToString());
                            }
                            repeater = repeater.AddDays(1);
                            if (viewM.RepeatInterval != "1")
                            {
                                week += 1;
                                if (week == 7)
                                {
                                    repeater = repeater.AddDays(threshold + 1);
                                    week = 1;
                                }
                            }                            
                        }
                        //Append ids to all tags 
                        foreach (string id in IDs)
                        {
                            ScheduleWorkArea append = db.ScheduleWorkAreas.Find(Convert.ToInt32(id));
                            append.RecurrentIDs = String.Join(",", IDs.ToArray());
                            db.Entry(append).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }                   
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            return Content("success");
        }

        public ActionResult ShowView(string id)
        {
            ScheduleViewViewHehModel viewM = new ScheduleViewViewHehModel();
            Schedule sch = db.Schedules.Find(Convert.ToInt32(id));
            viewM.UserID = DataValidations.getFullName(db, sch.UserID);
            viewM.Comments = sch.Comments;
            viewM.EndString = sch.EndTime.ToString();
            viewM.Hospital = sch.HospitalShortName;
            viewM.StartString = sch.StartTime.ToString();
            viewM.Title = sch.Title;
            viewM.Type = sch.ScheduleType;
            ScheduleType schType = (from t in db.ScheduleTypes where t.Event == viewM.Type select t).Single();
            viewM.BackColor = schType.BackColor;
            viewM.ForeColor = schType.ForeColor;
            return View(viewM);
        }

        public ActionResult Legend()
        {
            ScheduleLegendViewModel viewM = new ScheduleLegendViewModel();
            viewM.types = DataCollections.getScheduleTypeReference(db);
            return View(viewM);
        }

        [HttpGet]
        public ActionResult Edit()
        {
            ScheduleEditViewModel viewM = new ScheduleEditViewModel();
            string id = Request.QueryString["id"];
            viewM.Users = DataCollections.getScheduleUsers(db);
            viewM.Hospitals = DataCollections.getAllLocationsByType(db, false);
            viewM.Types = DataCollections.getScheduleTypes(db);
            viewM.Hours = DataCollections.getHours();
            viewM.Minutes = DataCollections.getMinutes();
            viewM.Repeat = DataCollections.getRepeats();

            //Check if target schedule belongs to recurrent pattern
            //If so, load first schedule of recurrent pattern rather than the clicked schedule
            viewM.TargetSchedule = DataCollections.getScheduleWorkArea(db, id);
            if (viewM.TargetSchedule.RecurrentIDs != null && viewM.TargetSchedule.RecurrentIDs != "")
            {
                string firstID = viewM.TargetSchedule.RecurrentIDs.Substring(0, viewM.TargetSchedule.RecurrentIDs.IndexOf(','));
                viewM.RepeatFirstID = firstID;
                viewM.TargetSchedule = DataCollections.getScheduleWorkArea(db, firstID);

                string[] split;
                split = viewM.TargetSchedule.RecurrentPattern.Split(new char[] { ',' });
                viewM.ShowRepeat = true;
                viewM.RepeatType = split[0];
                viewM.RepeatInterval = Convert.ToInt32(split[1]);
                viewM.RepeatHour = viewM.RepeatUntilDate.Hour.ToString();
                viewM.RepeatMinute = viewM.RepeatUntilDate.Minute.ToString();

                if (viewM.RepeatType == "Daily")
                {
                    viewM.RepeatUntilDate = DateTime.Parse(split[2]);
                }
                //Set checkbox values
                if (viewM.RepeatType == "Weekly")
                {
                    viewM.Days = split[2];
                    viewM.RepeatUntilDate = DateTime.Parse(split[3]);
                }
            }
            else
            {
                viewM.RepeatType = "None";
            }

            //Set the rest of the values of the schedule
            viewM.StartDate = viewM.TargetSchedule.StartTime ?? DateTime.Now;
            viewM.EndDate = viewM.TargetSchedule.EndTime ?? DateTime.Now;
            viewM.StartHour = viewM.TargetSchedule.StartTime.Value.Hour.ToString();
            viewM.EndHour = viewM.TargetSchedule.EndTime.Value.Hour.ToString();
            viewM.StartMinute = viewM.TargetSchedule.StartTime.Value.Minute.ToString();
            viewM.EndMinute = viewM.TargetSchedule.EndTime.Value.Minute.ToString();
            viewM.DefaultType = viewM.TargetSchedule.ScheduleType;
            viewM.DefaultHospital = viewM.TargetSchedule.HospitalShortName;
            viewM.DefaultUser = viewM.TargetSchedule.UserID;
            viewM.Title = viewM.TargetSchedule.Title;
            viewM.Comments = viewM.TargetSchedule.Comments;

            return View(viewM);
        }

        [HttpPost]
        public ActionResult Edit(ScheduleEditUpdate viewM)
        {
            try
            {
                if (viewM.forDelete == "yes")
                {
                    if (viewM.RepeatType != "None")
                    {
                        //List<ScheduleWorkArea> list = new List<ScheduleWorkArea>();
                        ScheduleWorkArea org = DataCollections.getScheduleWorkArea(db, viewM.id);
                        string[] ids = org.RecurrentIDs.Split(new char[] { ',' });
                        db.ScheduleWorkAreas.Remove(org);
                        //Collect entire series from ID and delete them all
                        if (viewM.RepeatScope == "WholeSeries")
                        {
                            foreach (string id in ids)
                            {
                                ScheduleWorkArea others = DataCollections.getScheduleWorkArea(db, id);
                                db.ScheduleWorkAreas.Remove(others);
                            }
                        }
                        //Decouple this id from all in series
                        else
                        {
                            string newRecurrent = string.Join(",", ids);
                            if (newRecurrent.Contains(viewM.id + ","))
                            {
                                newRecurrent = newRecurrent.Replace(viewM.id + ",", "");
                            }
                            else if (newRecurrent.Contains(viewM.id))
                            {
                                newRecurrent = newRecurrent.Replace(viewM.id, "");
                            }
                            else
                            {
                                throw new Exception("Old ID was not found. This shouldn't happen, notify IT :)");
                            }

                            foreach (string id in ids)
                            {
                                ScheduleWorkArea others = DataCollections.getScheduleWorkArea(db, id);
                                others.RecurrentIDs = newRecurrent;
                                db.Entry(others).State = System.Data.Entity.EntityState.Modified;
                            }
                        }
                        db.SaveChanges();
                        return Content("success");
                    }
                    else
                    {
                        DataSubmissions.DeleteScheduleWorkArea(db, viewM.id);
                        return Content("success");
                    }
                }
                else
                {
                    ScheduleWorkArea edit = DataCollections.getScheduleWorkArea(db, viewM.id);
                    edit.Comments = viewM.Comments;
                    edit.HospitalShortName = viewM.DefaultHospital;
                    edit.LastUpdated = DateTime.Now;
                    edit.ScheduleType = viewM.DefaultType;
                    edit.Title = viewM.Title;
                    edit.StartTime = DateTime.Parse(viewM.txtStartDate) + new TimeSpan(Convert.ToInt32(viewM.StartHour), Convert.ToInt32(viewM.StartMinute), 0);
                    edit.EndTime = DateTime.Parse(viewM.txtEndDate) + new TimeSpan(Convert.ToInt32(viewM.EndHour), Convert.ToInt32(viewM.EndMinute), 0);
                    if (viewM.RepeatType == "None")
                    {
                        edit.RecurrentIDs = null;
                        edit.RecurrentPattern = null;
                        db.Entry(edit).State = System.Data.Entity.EntityState.Modified;
                    }
                    //Delete Entire series and recreate based on parameters
                    else if (viewM.RepeatType == "Daily")
                    {
                        if (viewM.RepeatScope == "WholeSeries")
                        {
                            //Create recurrent pattern string
                            string recurrentPattern;
                            TimeSpan t = new TimeSpan(Convert.ToInt32(viewM.RepeatHour), Convert.ToInt32(viewM.RepeatMinute), 0);
                            DateTime temp = DateTime.Now.Date + t;
                            string time = temp.ToString("HH:mm:ss tt", CultureInfo.InvariantCulture);
                            recurrentPattern = viewM.RepeatType + "," + viewM.RepeatInterval + "," + viewM.txtRepeatDate + " " + time;

                            //Loop parameters
                            DateTime repeater = DateTime.Parse(viewM.txtStartDate);
                            //repeater = repeater.AddDays(Convert.ToInt32(viewM.RepeatInterval)); //Set repeat to start at next interval since we are already saving the original prior to loop
                            DateTime ender = DateTime.Parse(viewM.txtRepeatDate);

                            //This list stores the recurrent IDs being generated so they can be appended after insert
                            List<string> IDs = new List<string>();

                            //Set originals for deletion
                            //ScheduleWorkArea original = DataCollections.getScheduleWorkArea(db, viewM.id);
                            string[] ids = edit.RecurrentIDs.Split(new char[] { ',' });
                            foreach (string id in ids)
                            {
                                DataSubmissions.DeleteScheduleWorkArea(db, id);
                            }

                            //Loop and create repeat schedules
                            while (repeater <= ender)
                            {
                                ScheduleWorkArea nextSchedule = new ScheduleWorkArea();
                                nextSchedule = edit; //Reuse most of original schedule object
                                nextSchedule.RecurrentPattern = recurrentPattern;
                                nextSchedule.StartTime = repeater + new TimeSpan(Convert.ToInt32(viewM.StartHour), Convert.ToInt32(viewM.EndHour), 0);
                                nextSchedule.EndTime = repeater + new TimeSpan(Convert.ToInt32(viewM.EndHour), Convert.ToInt32(viewM.EndMinute), 0);
                                db.ScheduleWorkAreas.Add(nextSchedule);
                                db.SaveChanges();
                                IDs.Add(nextSchedule.ID.ToString());

                                repeater = repeater.AddDays(Convert.ToInt32(viewM.RepeatInterval));
                            }

                            //Loop through added records and append recurrentIDs
                            foreach (string id in IDs)
                            {
                                ScheduleWorkArea append = db.ScheduleWorkAreas.Find(Convert.ToInt32(id));
                                append.RecurrentIDs = String.Join(",", IDs.ToArray());
                                db.Entry(append).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }
                        }

                        //Update only this record and remove it from recurrent IDs of all other tags it was associated with
                        else if (viewM.RepeatScope == "ThisOnly")
                        {
                            //Remove id from other schedules
                            ScheduleWorkArea original = DataCollections.getScheduleWorkArea(db, viewM.id);
                            List<string> listID = original.RecurrentIDs.Split(new char[] { ',' }).ToList();
                            listID.Remove(viewM.id);
                            string newRecurrent = string.Join(",", listID);

                            foreach (string id in listID)
                            {
                                ScheduleWorkArea thisonly = DataCollections.getScheduleWorkArea(db, id);
                                thisonly.RecurrentIDs = newRecurrent;
                                db.Entry(thisonly).State = System.Data.Entity.EntityState.Modified;
                            }

                            edit.RecurrentIDs = null;
                            edit.RecurrentPattern = null;
                            db.Entry(edit).State = System.Data.Entity.EntityState.Modified;

                            //db.SaveChanges();
                        }
                    }
                    //Delete Entire series and recreate based on parameters
                    else if (viewM.RepeatType == "Weekly")
                    {
                        //Set recurrent values
                        ScheduleWorkArea org = DataCollections.getScheduleWorkArea(db, viewM.id);
                        if (viewM.RepeatScope == "WholeSeries")
                        {
                            //Create recurrent pattern string
                            string recurrentPattern;
                            TimeSpan t = new TimeSpan(Convert.ToInt32(viewM.RepeatHour), Convert.ToInt32(viewM.RepeatMinute), 0);
                            DateTime temp = DateTime.Now.Date + t;
                            string time = temp.ToString("HH:mm:ss tt", CultureInfo.InvariantCulture);
                            recurrentPattern = viewM.RepeatType + "," + viewM.RepeatInterval + "," + viewM.Days + "," + viewM.txtRepeatDate + " " + time;

                            //Loop parameters
                            DateTime repeater = DateTime.Parse(viewM.txtStartDate);
                            DateTime ender = DateTime.Parse(viewM.txtRepeatDate);

                            //Remove all old entries from series
                            string[] ids = edit.RecurrentIDs.Split(new char[] { ',' });
                            foreach (string id in ids)
                            {
                                DataSubmissions.DeleteScheduleWorkArea(db, id);
                            }

                            //This list stores the recurrent IDs being generated so they can be appended after insert
                            List<string> IDs = new List<string>();

                            int week = 1;
                            int threshold = 0;
                            if (viewM.RepeatInterval != 1)
                            {
                                threshold = (viewM.RepeatInterval - 1) * 7;
                            }

                            //Create additional
                            while (repeater <= ender)
                            {
                                //UNTESTED
                                if (viewM.Days.Contains(repeater.DayOfWeek.ToString().Substring(0, 3)))
                                {
                                    ScheduleWorkArea nextSchedule = new ScheduleWorkArea();
                                    nextSchedule = edit; //Reuse most of original schedule object
                                    nextSchedule.RecurrentPattern = recurrentPattern;
                                    nextSchedule.StartTime = repeater + new TimeSpan(Convert.ToInt32(viewM.StartHour), Convert.ToInt32(viewM.EndHour), 0);
                                    nextSchedule.EndTime = repeater + new TimeSpan(Convert.ToInt32(viewM.EndHour), Convert.ToInt32(viewM.EndMinute), 0);
                                    db.ScheduleWorkAreas.Add(nextSchedule);
                                    db.SaveChanges();
                                    IDs.Add(nextSchedule.ID.ToString());
                                }
                                repeater = repeater.AddDays(1);
                                if (viewM.RepeatInterval != 1)
                                {
                                    week += 1;
                                    if (week == 7)
                                    {
                                        repeater = repeater.AddDays(threshold + 1);
                                        week = 1;
                                    }
                                }
                            }
                            //Append ids to all tags 
                            foreach (string id in IDs)
                            {
                                ScheduleWorkArea append = db.ScheduleWorkAreas.Find(Convert.ToInt32(id));
                                append.RecurrentIDs = String.Join(",", IDs.ToArray());
                                db.Entry(append).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            //Remove id from other schedules
                            ScheduleWorkArea original = DataCollections.getScheduleWorkArea(db, viewM.id);
                            List<string> listID = original.RecurrentIDs.Split(new char[] { ',' }).ToList();
                            listID.Remove(viewM.id);
                            string newRecurrent = string.Join(",", listID);

                            foreach (string id in listID)
                            {
                                ScheduleWorkArea thisonly = DataCollections.getScheduleWorkArea(db, id);
                                thisonly.RecurrentIDs = newRecurrent;
                                db.Entry(thisonly).State = System.Data.Entity.EntityState.Modified;
                            }

                            edit.RecurrentIDs = null;
                            edit.RecurrentPattern = null;
                            db.Entry(edit).State = System.Data.Entity.EntityState.Modified;
                        }
                        
                    }
                    db.SaveChanges();
                    return Content("success");
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }           
        }

        
        public ActionResult Tally(string users, string hospitals, string start, string end, string operation)
        {
            ScheduleTallyViewModel viewM = new ScheduleTallyViewModel();
            string[] splitList;
            DateTime st = DateTime.Parse(start);
            DateTime et = DateTime.Parse(end);
            viewM.Operation = operation;
            //Setup lookup table
            IQueryable<ScheduleWorkArea> query = from s in db.ScheduleWorkAreas
                                                 where s.StartTime >= st && s.EndTime <= et
                                                 select s;

            if (users != "")
            {
                splitList = users.Split(new char[] { ',' });
                List<string> lst = splitList.ToList();
                query = query.Where(s => lst.Contains(s.UserID));
            }
            if (hospitals != "")
            {
                splitList = hospitals.Split(new char[] { ',' });
                List<string> lst = splitList.ToList();
                query = query.Where(s => lst.Contains(s.HospitalShortName));
            }

            switch (operation.Replace("Schedule", ""))
            {
                case "Day":
                    query = query.Where(s => s.ScheduleType == "Day-(D)");
                    break;
                case "Night":
                    query = query.Where(s => s.ScheduleType == "Night-(N)");
                    break;
                case "Weekend":
                    query = query.Where(s => s.ScheduleType == "Weekend-(W)");
                    break;
                case "TimeOff":
                    query = query.Where(s => s.ScheduleType == "CME-(CME)" ||  s.ScheduleType == "Vacation/Sick/Personal-(VSP)" || s.ScheduleType == "Vacation-Requested(VR)");
                    break;
                case "Moonlighting":
                    query = query.Where(s => s.ScheduleType == "Moonlighting-(ML)");
                    break;
                case "Call":
                    query = query.Where(s => s.ScheduleType == "AIMS Call Schedule-(ACS)" || s.ScheduleType == "Admin On Call-(AOC)" || s.ScheduleType == "Clinic Call-(CC)" || s.ScheduleType == "ED Staff Call-(EDSC)"  );
                    break;
            }
            //query = query.GroupBy(s => new { s.UserID, s.HospitalShortName, s.ScheduleType });
            

            return View(viewM);
        }

        public ActionResult Month()
        {
            Dpm returnDpm = new Dpm();

            //returnDpm.StartDate = (DateTime)Session["scheduleDate"];
            //returnDpm.VisibleStart = viewM.SelectedDate.
            //if (viewM.SelectedView == "Week")
            //{
            //    returnDpm.ViewType = DayPilot.Web.Mvc.Enums.Month.ViewType.Weeks;
            //}
            //return new Dpm().CallBack(this);
            return returnDpm.CallBack(this);
        }

        public ActionResult MonthWork()
        {
            DpmWork returnDpm = new DpmWork();
            return returnDpm.CallBack(this);
        }

        //Returns a list of favorites for the specified user
        [HttpGet]
        public ActionResult jsonGetFavorites(string user)
        {
            var query = DataCollections.getFavorites(db);
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult jsonGetDefault(string user)
        {
            var query = DataCollections.getFavoriteDefault(db);
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult jsonSetDefault(string id)
        {
            try
            {
                string name = DataSubmissions.SetDefaultFavorite(db, id);
                return Content(name);
            }
            catch (Exception ex)
            {

                return Content(ex.Message);
            }        
        }

        [HttpPost]
        public ActionResult jsonDeleteFavorite(string id)
        {
            try
            {
                string name = DataSubmissions.DeleteFavorite(db, id);
                return Content(name);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult jsonDeleteRecord(string id)
        {
            try
            {
                ScheduleWorkArea sch = DataCollections.getScheduleWorkArea(db, id);
                //Decouple this entry from it's recurrent buddies
                if (sch.RecurrentIDs != null && sch.RecurrentIDs != "")
                {
                    string[] ids = sch.RecurrentIDs.Split(new char[] { ',' });
                    string newRecurrent = string.Join(",", ids);
                    if (newRecurrent.Contains(sch.ID + ","))
                    {
                        newRecurrent = newRecurrent.Replace(sch.ID + ",", "");
                    }
                    else if (newRecurrent.Contains(sch.ID.ToString()))
                    {
                        newRecurrent = newRecurrent.Replace(sch.ID.ToString(), "");
                    }
                    else
                    {
                        throw new Exception("Old ID was not found. This shouldn't happen, notify IT :)");
                    }

                    foreach (string i in ids)
                    {
                        ScheduleWorkArea others = DataCollections.getScheduleWorkArea(db, i);
                        others.RecurrentIDs = newRecurrent;
                        db.Entry(others).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                DataSubmissions.DeleteScheduleWorkArea(db, id); //db.SaveChanges() gets called in here, yay spaghetti!
                return Json("yay");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        class Dpm : DayPilotMonth
        {
            private PatientLogModel db = new PatientLogModel();

            protected override void OnInit(DayPilot.Web.Mvc.Events.Month.InitArgs e)
            {
                StartDate = (DateTime)System.Web.HttpContext.Current.Session["scheduleDate"];
                ViewType = DayPilot.Web.Mvc.Enums.Month.ViewType.Month;
                Update(CallBackUpdateType.Full);
            }

            protected override void OnEventMove(DayPilot.Web.Mvc.Events.Month.EventMoveArgs e)
            {
                new EventManager().EventMove(e.Id, e.NewStart, e.NewEnd);
                Update();
            }

            protected override void OnBeforeCellRender(DayPilot.Web.Mvc.Events.Month.BeforeCellRenderArgs e)
            {
                //e.CssClass = e.Start.ToShortDateString(); 
            }

            protected override void OnBeforeEventRender(DayPilot.Web.Mvc.Events.Month.BeforeEventRenderArgs e)
            {
                int index = 0;
                string schType = e.DataItem["ScheduleType"].ToString();
                List<ScheduleType> types = (List<ScheduleType>)System.Web.HttpContext.Current.Session["ScheduleTypeReference"];
                index = types.FindIndex(type => type.Event == schType);
                e.BackgroundColor = types[index].BackColor;
                if (types[index].ForeColor.ToUpper() == "WHITE")
                {
                    e.CssClass = "whiteText";
                }
                string user = e.DataItem["UserID"].ToString();
                user = user.Substring(1);
                e.ToolTip = user + "&#013;" +
                            e.DataItem["Hospital"].ToString() + "&#013;" +
                            e.DataItem["StartTime"].ToString() + "&#013;" +
                            e.DataItem["EndTime"].ToString();
            }

            protected override void OnCommand(DayPilot.Web.Mvc.Events.Month.CommandArgs e)
            {
                switch (e.Command)
                {
                    case "navigate":
                        StartDate = (DateTime)e.Data["day"];
                        Update(CallBackUpdateType.Full);
                        break;
                    case "refresh":
                        Update(CallBackUpdateType.EventsOnly);
                        break;
                    case "delete":
                        new EventManager().EventDelete((string)e.Data["e"]["id"]);
                        Update(CallBackUpdateType.EventsOnly);
                        break;
                }
            }

            protected override void OnFinish()
            {
                if (UpdateType == CallBackUpdateType.None)
                {
                    return;
                }
                List<string> users = new List<string>();
                List<string> hospitals = new List<string>();
                List<string> types = new List<string>();
                bool rounding = false;

                if (System.Web.HttpContext.Current.Session["scheduleUsers"] != null)
                {
                    List<string> list = (List<string>)System.Web.HttpContext.Current.Session["scheduleUsers"];
                    if (list.Count == 1)
                    {
                        users.Add(list[0]);
                    }
                    else
                    {
                        users = list;
                    }

                }
                if (System.Web.HttpContext.Current.Session["scheduleHospitals"] != null)
                {
                    hospitals = (List<string>)System.Web.HttpContext.Current.Session["scheduleHospitals"];
                }
                if (System.Web.HttpContext.Current.Session["scheduleTypes"] != null)
                {
                    types = (List<string>)System.Web.HttpContext.Current.Session["scheduleTypes"];
                }
                if (System.Web.HttpContext.Current.Session["scheduleRounding"] != null)
                {
                    rounding = (bool)System.Web.HttpContext.Current.Session["scheduleRounding"];
                }

                Events = new EventManager().FilteredData(db, VisibleStart, VisibleEnd, users, hospitals, types, rounding).AsEnumerable();


                DataIdField = "ID";
                DataTextField = "Title";
                DataStartField = "StartTime";
                DataEndField = "EndTime";

                db.Dispose();
            }
        }


        class DpmWork : DayPilotMonth
        {
            private PatientLogModel db = new PatientLogModel();

            protected override void OnInit(DayPilot.Web.Mvc.Events.Month.InitArgs e)
            {
                StartDate = (DateTime)System.Web.HttpContext.Current.Session["scheduleDate"];
                ViewType = DayPilot.Web.Mvc.Enums.Month.ViewType.Month;
                Update(CallBackUpdateType.Full);
            }

            //protected override void OnEventMove(DayPilot.Web.Mvc.Events.Month.EventMoveArgs e)
            //{
            //    new EventManager().EventMove(e.Id, e.NewStart, e.NewEnd);
            //    Update();
            //}

            protected override void OnBeforeCellRender(DayPilot.Web.Mvc.Events.Month.BeforeCellRenderArgs e)
            {
                //e.CssClass = e.Start.ToShortDateString(); 
            }

            protected override void OnBeforeEventRender(DayPilot.Web.Mvc.Events.Month.BeforeEventRenderArgs e)
            {
                int index = 0;
                string schType = e.DataItem["ScheduleType"].ToString();
                List<ScheduleType> types = (List<ScheduleType>)System.Web.HttpContext.Current.Session["ScheduleTypeReference"];
                index = types.FindIndex(type => type.Event == schType);
                e.BackgroundColor = types[index].BackColor;
                if (types[index].ForeColor.ToUpper() == "WHITE")
                {
                    e.CssClass = "whiteText";
                }
                string user = e.DataItem["UserID"].ToString();
                user = user.Substring(1);
                e.ToolTip = user + "&#013;" +
                            e.DataItem["Hospital"].ToString() + "&#013;" +
                            e.DataItem["StartTime"].ToString() + "&#013;" +
                            e.DataItem["EndTime"].ToString();
            }

            protected override void OnCommand(DayPilot.Web.Mvc.Events.Month.CommandArgs e)
            {
                switch (e.Command)
                {
                    case "navigate":
                        StartDate = (DateTime)e.Data["day"];
                        Update(CallBackUpdateType.Full);
                        break;
                    case "refresh":
                        Update(CallBackUpdateType.EventsOnly);
                        break;
                    case "delete":
                        new EventManager().EventDelete((string)e.Data["e"]["id"]);
                        Update(CallBackUpdateType.EventsOnly);
                        break;
                }
            }

            protected override void OnFinish()
            {
                if (UpdateType == CallBackUpdateType.None)
                {
                    return;
                }
                List<string> users = new List<string>();
                List<string> hospitals = new List<string>();
                List<string> types = new List<string>();
                bool rounding = false;

                if (System.Web.HttpContext.Current.Session["scheduleUsers"] != null)
                {
                    List<string> list = (List<string>)System.Web.HttpContext.Current.Session["scheduleUsers"];
                    if (list.Count == 1)
                    {
                        users.Add(list[0]);
                    }
                    else
                    {
                        users = list;
                    }

                }
                if (System.Web.HttpContext.Current.Session["scheduleHospitals"] != null)
                {
                    hospitals = (List<string>)System.Web.HttpContext.Current.Session["scheduleHospitals"];
                }
                if (System.Web.HttpContext.Current.Session["scheduleTypes"] != null)
                {
                    types = (List<string>)System.Web.HttpContext.Current.Session["scheduleTypes"];
                }
                if (System.Web.HttpContext.Current.Session["scheduleRounding"] != null)
                {
                    rounding = (bool)System.Web.HttpContext.Current.Session["scheduleRounding"];
                }

                Events = new EventManagerWork().FilteredData(db, VisibleStart, VisibleEnd, users, hospitals, types, rounding).AsEnumerable();


                DataIdField = "ID";
                DataTextField = "Title";
                DataStartField = "StartTime";
                DataEndField = "EndTime";

                db.Dispose();
            }
        }
    }
}
