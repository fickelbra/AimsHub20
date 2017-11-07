using System.Collections.Generic;
using static AimsHub.Models.GridFilter;
using AimsHub.Models;
using System.Web.Mvc;

namespace AimsHub.ViewModels
{
    public class PatientAssignmentIndexViewModel
    {
        public PatientAssignmentIndexViewModel()
        {
            SortDirection = SortDirections.Ascending;
            SortColumn = "PatientName";
        }
        public IEnumerable<PatientLog> Patients { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public SelectList PhysicianList { get; set; }
        public SelectList HospitalList { get; set; }
        public SelectList ServiceList { get; set; }
        public SelectList ImportColumns { get; set; }
        public List<string> SelectedPhysicians { get; set; }
        public string SelectedHospitals { get; set; }
        public List<string> SelectedServices { get; set; }
        public string hidPhysicians { get; set; }
        public string hidHospitals { get; set; }
        public string hidServices { get; set; }
        public SortDirections SortDirection { get; set; }
        public string SortColumn { get; set; }
        public Dictionary<string, string[]> ToLookupTally { get; set; }
        public string CopyButton { get; set; }
        public string DeleteButton { get; set; }
        public string hidSelectedIDs { get; set; }
    }
}