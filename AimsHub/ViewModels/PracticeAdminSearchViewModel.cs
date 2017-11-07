using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AimsHub.Models;

namespace AimsHub.ViewModels
{
    public class PracticeAdminSearchViewModel
    {
        public IEnumerable<PatientLog> Patients { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string PatientSearch { get; set; }
    }
}