using System.Collections.Generic;
using AimsHub.Models;
using System.Web.Mvc;
using static AimsHub.Models.GridFilter;

namespace AimsHub.ViewModels
{
    public class PatientLogIndexViewModel
    {
        public PatientLogIndexViewModel()
        {
                SortDirection = SortDirections.Ascending;
                SortColumn = "PatientName";
        }
        public IEnumerable<PatientLog> Patients { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        //public SelectList PhysicianList { get; set; }
        public SelectList HospitalList { get; set; }
        public SelectList ServiceList { get; set; }
        //public List<string> SelectedPhysicians { get; set; }
        public List<string> SelectedHospitals { get; set; }
        public List<string> SelectedServices { get; set; }
        //public string hidPhysicians { get; set; }
        public string hidHospitals { get; set; }
        public string hidServices { get; set; }
        public SortDirections SortDirection { get; set; }
        public string SortColumn { get; set; }
        public string FilterColumn { get; set; }
        public FilterTypes FilterType { get; set; }
        public string FilterValue { get; set; }
        public bool Assigned { get; set; }
        public Dictionary<string, string> RiskFactor { get; set; }

    }
}