using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AimsHub.Extensions;
using AimsHub.Models;
using AimsHub.Security;

namespace AimsHub.Views.PatientLog
{
    public partial class CustomFax : System.Web.UI.Page
    {
        private PatientLogModel db = new PatientLogModel();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
               // string faxType = drpFaxTypes.SelectedValue;
               // drpFaxTypes.Visible = false;
               // string std = Request.QueryString["patid"];
               // int id = Convert.ToInt32(std);
               // AimsHub.Models.PatientLog pat = new AimsHub.Models.PatientLog();
               // pat = db.PatientLogs.Find(id);

               // ReportViewer1.Visible = true;
               // ReportViewer1.Reset();
               // ReportViewer1.ProcessingMode = ProcessingMode.Local;
               // btnEdit.Visible = true;
               // btnSend.Visible = true;
               // if (faxType.Contains("Notice"))
               // {
               //     ReportViewer1.LocalReport.ReportPath = "AdDisNotice/" + faxType + pat.Hospital + ".rdlc";
               // }
               // else if (faxType == "DischargeSummary")
               // {
               //     ReportViewer1.LocalReport.ReportPath = "AdDisNotice/DischargeSummary.rdlc";
               // }
               // else if (faxType == "GeneralCommunication")
               // {
               //     ReportViewer1.LocalReport.ReportPath = "AdDisNotice/GeneralCommunication.rdlc";
               // }
               // else
               // {
               //     ReportViewer1.Visible = false;
               //     btnEdit.Visible = false;
               //     btnSend.Visible = false;
               //     return;
               // }

               // //Break apart PCP name
               // string[] splitName;
               // splitName = pat.PCP_Practice.Split(new char[] { ',' });
               // string pcpName = splitName[1] + " " + splitName[0];
               // string firstname = splitName[1].Trim();
               // string lastname = splitName[0].Trim();
               // string pcpID = (from u in db.Users where (u.FirstName == firstname && u.LastName == lastname) select u.UserID).Single();
                

               // //Get practice info of PCP
               // //ReferringPractice prac = new ReferringPractice();
               // //prac = (from p in db.ReferringPractices
               // //        join r in db.RefPracUsers on p.PracID equals r.PracID
               // //        where r.UserID == pcpID
               // //        select p).Single();

               // //Cannot perform ToShortDateString on pat.ServiceDate because of DateTime? type
               // DateTime thed = new DateTime();
               // thed = Convert.ToDateTime(pat.ServiceDate);
               // DateTime birth = new DateTime();
               // birth = Convert.ToDateTime(pat.DOB);

               // ReportParameter dateparam = new ReportParameter("Date", thed.ToShortDateString());
               // //ReportParameter faxparam = new ReportParameter("Fax", prac.Fax);
               // ReportParameter faxparam = new ReportParameter("Fax", "313-867-5309");
               // ReportParameter faxtoparam = new ReportParameter("FaxTo", pcpName);
               // ReportParameter dischargeparam = new ReportParameter("DischargeDate", thed.ToShortDateString());
               // ReportParameter patientparam = new ReportParameter("Fax", pat.PatientName);
               // ReportParameter treatmentparam = new ReportParameter("Treatment", pat.Comments);
               // ReportParameter mrnparam = new ReportParameter("MRN", pat.MRN_FIN);
               // ReportParameter physicianparam = new ReportParameter("Physician", HubSecurity.getLoggedInDisplayName());
               // ReportParameter dobparam = new ReportParameter("DOB", birth.ToShortDateString());

               // ReportParameterCollection theparams = new ReportParameterCollection() { dateparam, faxparam, faxtoparam, dischargeparam, patientparam, treatmentparam, mrnparam, physicianparam, dobparam };
               //ReportViewer1.LocalReport.SetParameters(theparams);

            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                string faxType = drpFaxTypes.SelectedValue;
                drpFaxTypes.Visible = false;
                string std = Request.QueryString["patid"];
                int id = Convert.ToInt32(std);
                AimsHub.Models.PatientLog pat = new AimsHub.Models.PatientLog();
                pat = db.PatientLogs.Find(id);

                ReportViewer1.Visible = true;
                ReportViewer1.Reset();
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                btnEdit.Visible = true;
                btnSend.Visible = true;
                lblMessage.Visible = false;
                if (faxType.Contains("Notice"))
                {
                    ReportViewer1.LocalReport.ReportPath = "AdDisNotice/" + faxType + pat.Hospital + ".rdlc";
                }
                else if (faxType == "DischargeSummary")
                {
                    ReportViewer1.LocalReport.ReportPath = "AdDisNotice/DischargeSummary.rdlc";
                }
                else if (faxType == "GeneralCommunication")
                {
                    ReportViewer1.LocalReport.ReportPath = "AdDisNotice/GeneralCommunication.rdlc";
                }
                else
                {
                    ReportViewer1.Visible = false;
                    btnEdit.Visible = false;
                    btnSend.Visible = false;
                    lblMessage.Visible = true;
                    return;
                }

                //Break apart PCP name
                string[] splitName;
                splitName = pat.PCP_Practice.Split(new char[] { ',' });
                string pcpName = splitName[1] + " " + splitName[0];
                string firstname = splitName[1].Trim();
                string lastname = splitName[0].Trim();
                string pcpID = (from u in db.Users where (u.FirstName == firstname && u.LastName == lastname) select u.UserID).Single();


                //Get practice info of PCP
                //ReferringPractice prac = new ReferringPractice();
                //prac = (from p in db.ReferringPractices
                //        join r in db.RefPracUsers on p.PracID equals r.PracID
                //        where r.UserID == pcpID
                //        select p).Single();

                //Cannot perform ToShortDateString on pat.ServiceDate because of DateTime? type
                DateTime thed = new DateTime();
                thed = Convert.ToDateTime(pat.ServiceDate);
                DateTime birth = new DateTime();
                birth = Convert.ToDateTime(pat.DOB);

                ReportParameter dateparam = new ReportParameter("Date", thed.ToShortDateString());
                //ReportParameter faxparam = new ReportParameter("Fax", prac.Fax);
                ReportParameter faxparam = new ReportParameter("Fax", "313-867-5309");
                ReportParameter faxtoparam = new ReportParameter("FaxTo", pcpName);
                ReportParameter dischargeparam = new ReportParameter("DischargeDate", thed.ToShortDateString());
                ReportParameter patientparam = new ReportParameter("Fax", pat.PatientName);
                ReportParameter treatmentparam = new ReportParameter("Treatment", pat.Comments);
                ReportParameter mrnparam = new ReportParameter("MRN", pat.MRN_FIN);
                ReportParameter physicianparam = new ReportParameter("Physician", HubSecurity.getLoggedInDisplayName());
                ReportParameter dobparam = new ReportParameter("DOB", birth.ToShortDateString());

                ReportParameterCollection theparams = new ReportParameterCollection() { dateparam, faxparam, faxtoparam, dischargeparam, patientparam, treatmentparam, mrnparam, physicianparam, dobparam };
                ReportViewer1.LocalReport.SetParameters(theparams);
            }
        }

        protected void btnFax_Clicked(object sender, EventArgs e)
        {
            GenerateComms gcomm = new GenerateComms();
            //gcomm.SendFax()
            
        }


    }
}