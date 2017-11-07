using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AimsHub.ViewModels
{
    public class ScheduleWorkAreaViewModel
    {
        public ScheduleWorkAreaViewModel()
        {
            ShowRounding = false;
            OperationCompleted = false;
            OperationResult = 0;
            SelectedUsers = new List<string>();
            SelectedHospitals = new List<string>();
            SelectedSchedules = new List<string>();
            //FavoriteList = new List<string>();
            //ScheduleRoundingList = new List<string>();
            FavoriteMakeDefault = false;
        }
        public DateTime SelectedDate { get; set; }
        public SelectList UserList { get; set; }
        public SelectList HospitalList { get; set; }
        public SelectList ScheduleList { get; set; }
        public SelectList FavoriteList { get; set; }
        public SelectList ScheduleRoundingList { get; set; }
        //public SelectList ViewList { get; set; }
        public List<string> SelectedUsers { get; set; }
        public List<string> SelectedHospitals { get; set; }
        public List<string> SelectedSchedules { get; set; }
        public string hidUsers { get; set; }
        public string hidHospitals { get; set; }
        public string hidTypes { get; set; }
        public string hidHospitalList { get; set; } //Used by javascript to filter out only hospitals
        public string hidNHList { get; set; } //Used by javascript to filter out only NH
        //public string SelectedView { get; set; }
        public bool ShowRounding { get; set; }
        public string DateString { get; set; }
        public DateTime OperationStartDate { get; set; }
        public DateTime OperationEndDate { get; set; }
        public SelectList MassEditCriteria { get; set; }
        public string PreviousButton { get; set; }
        public string NextButton { get; set; }
        public SelectList Hours { get; set; }
        public string OperationAction { get; set; }
        public string OperationCriteria { get; set; }
        public string OperationChangeTo { get; set; }
        public bool OperationCompleted { get; set; }
        public int OperationResult { get; set; }
        public string FavoritePostback { get; set; }
        public string FavoriteSelected { get; set; }
        public string FavoriteNewName { get; set; }
        public bool FavoriteMakeDefault { get; set; }
        public string FavoriteAddPostback { get; set; }
    }
}