using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AimsHub.ViewModels
{
    public class ScheduleCreateViewModel
    {
        public SelectList Users { get; set; }
        public SelectList Hospitals { get; set; }
        public SelectList Types { get; set; }
        public SelectList Hours { get; set; }
        public SelectList Minutes { get; set; }
        public SelectList SecondHospitals {get; set; }
        public string StartTime { get; set; }
        public DateTime StartDate { get; set; }
        public string StartDateString { get; set; }
        public string EndTime { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public string Comments { get; set; }
        public SelectList Repeat { get; set; }
        public string RepeatInterval { get; set; }
        //public string RepeatUntilTime { get; set; }
        public DateTime RepeatDate { get; set; }
        public string RepeatHour { get; set; }
        public string RepeatMinute { get; set; }
        public SelectList Patterns { get; set; }
        public DateTime PatternUntilDate { get; set; }
        public string SecondHospital { get; set; }
        public string StartHour { get; set; }
        public string EndHour { get; set; }
        public string DefaultType { get; set; }
        public string DefaultUser { get; set; }
        public string DefaultHospital { get; set; }

    }
}