using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace AimsHub.ViewModels
{
    public class ScheduleTallyViewModel
    {
        public ScheduleTallyViewModel() { result = new DataTable(); }
        //public Dictionary<string, string[]> ToLookupTally { get; set; }
        public DataTable result { get; set; }
        //public string Users { get; set; }
        //public string Hospitals { get; set; }
        //public string Start { get; set; }
        //public string End { get; set; }
        public string Operation { get; set; }
    }
}