using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AimsHub.Models;

namespace AimsHub.ViewModels
{
    public class PCPCommunicationManageViewModel
    {
        public IEnumerable<PCPCommunicationManagePatient> Patients { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public SelectList HospitalList { get; set; }
        public List<string> SelectedHospitals { get; set; }
        public string hidHospitals { get; set; }

        public SelectList PCPList;
        public List<string> SelectedPCPs { get; set; }
        public string hidPCPs { get; set; }

        public List<string> Arcturus { get; set; }
        public List<string> Envision { get; set; }
        public List<string> HFMG { get; set; }
        public List<string> Reliance { get; set; }

        public BasedOn basedOn;

        public enum BasedOn
        {
            ServiceDate,
            DateCreated
        }
    }
}