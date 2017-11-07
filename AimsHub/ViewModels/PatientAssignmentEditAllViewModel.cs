using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AimsHub.Models;
using System.Web.Mvc;
using static AimsHub.Models.GridFilter;

namespace AimsHub.ViewModels
{
    public class PatientAssignmentEditAllViewModel
    {
        public PatientAssignmentEditAllViewModel()
        {
            Patients = new List<PatientLogEditViewModel>();
        }

        public List<PatientLogEditViewModel> Patients;

        public string FromDate;

        public string ToDate;

        public bool isAdmin;
    }
}