using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AimsHub.ViewModels
{
    public class ScheduleEditUpdate
    {
        public string id { get; set; }
        public string Title { get; set; }
        public string Comments { get; set; }
        public string Repeat { get; set; }
        public int RepeatInterval { get; set; }
        public string RepeatUntilTime { get; set; }
        public string RepeatType { get; set; }
        public string StartHour { get; set; }
        public string StartMinute { get; set; }
        public string txtStartDate { get; set; }
        public string txtEndDate { get; set; }
        public string txtRepeatDate { get; set; }
        public string EndMinute { get; set; }
        public string EndHour { get; set; }
        public string RepeatHour { get; set; }
        public string RepeatMinute { get; set; }
        public string DefaultType { get; set; }
        public string DefaultUser { get; set; }
        public string DefaultHospital { get; set; }
        public string forDelete { get; set; }
        public string Days { get; set; }
        public string RepeatScope { get; set; }
        public string RepeatFirstID { get; set; } //This is used in instances where a repeat schedule was clicked to preserve the original schedule clicked since Edit defaults to the first record of a repeat LONG COMMENT
    }
}