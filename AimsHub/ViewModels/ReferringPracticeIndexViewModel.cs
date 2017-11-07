using AimsHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AimsHub.ViewModels
{
    public class ReferringPracticeIndexViewModel
    {
        public IEnumerable<ReferringPractice> ReferringPractices;

        public string filterPracticeName;

        public string filterAddressLine1;

        public string filterCity;

        public string filterFax;

        public SelectList HospitalList;

        public SelectList RefPhyList;

        public List<string> SelectedHospitals;

        public List<string> SelectedRefPhy;

        public string hidHospitals;

        public string hidRefPhy;    
    }
}