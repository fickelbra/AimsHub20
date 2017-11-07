using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AimsHub.ViewModels
{
    public class PracticeAdminSupportViewModel
    {
        public PracticeAdminSupportViewModel() { MessageSent = false; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string ReturnEmail { get; set; }
        public SelectList Reasons { get; set; }
        public string Reason { get; set; }
        public bool MessageSent { get; set; }
        public string ErrorMessage { get; set; }
    }
}