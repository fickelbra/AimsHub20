using AimsHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AimsHub.ViewModels
{
    public class BillingCorrectionViewModel
    {
        public IEnumerable<BillingIndexPatient> Patients { get; set; }
        //public PatientLog selectedPatient { get; set; }
        public SelectList PhysicianList { get; set; }
        public SelectList HospitalList { get; set; }
        public SelectList GenderList { get; set; }
        public SelectList PCPList { get; set; }
        public SelectList ServiceList { get; set; }
        public List<string> SelectedHospitals { get; set; }
        public List<string> SelectedServices { get; set; }
        public string hidHospitals { get; set; }
        public string hidServices { get; set; }
        public string SearchPatientName { get; set; }
        public string SearchServiceDate { get; set; }

    }
}