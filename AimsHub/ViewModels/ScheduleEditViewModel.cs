using AimsHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AimsHub.ViewModels
{
    public class ScheduleEditViewModel
    {
        public ScheduleEditViewModel()
        {
            ShowRepeat = false;
        }
        public ScheduleWorkArea TargetSchedule { get; set; }
        public string RepeatFirstID { get; set; }
        public SelectList Users { get; set; }
        public SelectList Hospitals { get; set; }
        public SelectList Types { get; set; }
        public SelectList Hours { get; set; }
        public SelectList Minutes { get; set; }
        public string StartTime { get; set; }
        public DateTime StartDate { get; set; }
        public string StartDateString { get; set; }
        public string EndTime { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public string Comments { get; set; }
        public SelectList Repeat { get; set; }
        public int RepeatInterval { get; set; }
        public string RepeatUntilTime { get; set; }
        public DateTime RepeatUntilDate { get; set; }
        public string RepeatType { get; set; }
        public string RepeatScope { get; set; }
        public string StartHour { get; set; }
        public string StartMinute { get; set; }
        public string EndHour { get; set; }
        public string EndMinute { get; set; }
        public string RepeatHour { get; set; }
        public string RepeatMinute { get; set; }
        public string DefaultType { get; set; }
        public string DefaultUser { get; set; }
        public string DefaultHospital { get; set; }
        public bool ShowRepeat { get; set; }
        public string Days { get; set; }
    }
}