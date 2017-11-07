using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AimsHub.Models;

namespace AimsHub.ViewModels
{
    public class PCPCommunicationIndexViewModel
    {
        public PCPCommunicationIndexViewModel()
        {
            //Selected = new List<int>();
        }

        public IEnumerable<PatientLog> Patients { get; set; }

        public string hidIDs { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

    }
}