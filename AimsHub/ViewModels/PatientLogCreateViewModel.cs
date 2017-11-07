using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AimsHub.Models;

namespace AimsHub.ViewModels
{
    public class PatientLogCreateViewModel
    {
        public PatientLog Patient { get; set; }

        public SelectList HospitalList;

        public SelectList PCPList;

        public SelectList ServiceTypeList;

        public SelectList PatientClassList;

        public SelectList GenderList;
    }
}