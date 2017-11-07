using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AimsHub.Models;
using System.Web.Mvc;

namespace AimsHub.ViewModels
{
    public class PracticeAdminEditPreferencesViewModel
    {
        public PracticeAdminEditPreferencesViewModel() { tabReturn = "1"; }
        public ReferringPractice selectedPrac { get; set; }
        public IEnumerable<RefPracSpecialty> specialties { get; set; }
        public SelectList practices { get; set; }
        public string hidPrac { get; set; }
        public string UpdatePracticeInformation { get; set; }
        public string UpdateCommunicationMethod { get; set; }
        public string UpdateSpecialties { get; set; }
        public string tabReturn { get; set; }
        public string hidAll { get; set; }
    }
}