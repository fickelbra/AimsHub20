using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AimsHub.ViewModels
{
    public class ScheduleCreateUpdate
    {
        public string Title { get; set; }
        public string Comments { get; set; }
        
        public string txtStartDate { get; set; }
        public string StartHour { get; set; }
        public string StartMinute { get; set; }

        public string txtEndDate { get; set; }
        public string EndMinute { get; set; }
        public string EndHour { get; set; }

        public string DefaultType { get; set; }
        public string DefaultUser { get; set; }
        public string DefaultHospital { get; set; }

        public string Patterns { get; set; }
        public string txtDoUntil { get; set; }
        public string SecondHospital { get; set; }
        public string chkPattern { get; set; }

        public string Repeat { get; set; }
        public string RepeatInterval { get; set; }
        public string chkSunday { get; set; }
        public string chkMonday { get; set; }
        public string chkTuesday { get; set; }
        public string chkWednesday { get; set; }
        public string chkThursday { get; set; }
        public string chkFriday { get; set; }
        public string chkSaturday { get; set; }
        public string txtRepeatDate { get; set; }
        public string RepeatHour { get; set; }
        public string RepeatMinute { get; set; }
    }
}