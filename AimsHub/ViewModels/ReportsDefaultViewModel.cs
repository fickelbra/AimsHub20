using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static AimsHub.Models.GridFilter;

namespace AimsHub.ViewModels
{
    public class ReportsDefaultViewModel
    {
        public string ReportName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string DefaultHospital { get; set; }
        public string DefaultUser { get; set; }
        public SelectList Users { get; set; }
        public SelectList Hospitals { get; set; }
        public List<string> Columns { get; set; }
        public DataTable Report { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
    }
}