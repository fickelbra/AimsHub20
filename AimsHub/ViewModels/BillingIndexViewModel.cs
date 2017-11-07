using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AimsHub.Models;
using static AimsHub.Models.GridFilter;

namespace AimsHub.ViewModels
{
    public class BillingIndexViewModel
    {
        public BillingIndexViewModel()
        {
            SortDirection = SortDirections.Ascending;
            SortColumn = "PatientName";
        }
        public IEnumerable<BillingIndexPatient> Patients { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public SelectList PhysicianList { get; set; }
        public SelectList HospitalList { get; set; }
        public SelectList ServiceList { get; set; }
        public SelectList LastNameFilterList { get; set; }
        public SelectList YesNo { get; set; }
        public List<string> SelectedPhysicians { get; set; }
        public List<string> SelectedHospitals { get; set; }
        public List<string> SelectedServices { get; set; }
        public List<string> SelectedLastNameFilters { get; set; }
        public string hidPhysicians { get; set; }
        public string hidHospitals { get; set; }
        public string hidServices { get; set; }
        public string hidLastNameFilters { get; set; }
        //Billing filters
        public bool? SelectedNotesCompleted { get; set; }
        public bool? SelectedNotesCopied { get; set; }
        public bool? SelectedFaceSheetEntered { get; set; }
        public bool? SelectedCodingCompleted { get; set; }
        public bool? SelectedChargePosted { get; set; }
        public string hidNotesCompleted { get; set; }
        public string hidNotesCopied { get; set; }
        public string hidFaceSheetEntered { get; set; }
        public string hidCodingCompleted { get; set; }
        public string hidChargePosted { get; set; }

        public SortDirections SortDirection { get; set; }
        public string SortColumn { get; set; }
    }
}