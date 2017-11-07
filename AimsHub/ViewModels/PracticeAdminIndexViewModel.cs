using AimsHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static AimsHub.Models.GridFilter;

namespace AimsHub.ViewModels
{
    public class PracticeAdminIndexViewModel
    {
        public PracticeAdminIndexViewModel()
        {
            dateList = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                if (i != 0)
                {
                    dateList.Add(i.ToString() + " " + "Days");
                }
                else
                {
                    dateList.Add(i.ToString() + " " + "Day");
                }
            }
        }
        public IEnumerable<PatientLog> Patients { get; set; }
        public DateTime dateFallback { get; set; }
        public List<string> dateList { get; set; }
        public SelectList HospitalList { get; set; }
        public SelectList ServiceList { get; set; }
        public List<string> SelectedHospitals { get; set; }
        public List<string> SelectedServices { get; set; }
        public string hidHospitals { get; set; }
        public string hidServices { get; set; }
        public SortDirections SortDirection { get; set; }
        public string SortColumn { get; set; }
    }
}