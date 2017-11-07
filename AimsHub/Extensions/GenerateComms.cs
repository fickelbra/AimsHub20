using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using iTextSharp.text.pdf;
using System.Net.Mail;
using AimsHub.Models;
using AimsHub.Security;

namespace AimsHub.Extensions
{
    public class GenerateComms
    {
        private const string FAX_COVER = "AIMSFaxCoverGeneric.cov";
        private const string MACHINE_PATH = "\\AIMSppse05\\C$";
        private const string MAIL_SERVER = "192.168.140.23";
        private const int MAIL_PORT = 25;

        public void SendFax(string strRepType, Microsoft.Reporting.WebForms.ReportViewer AIMSReportViewer, string strFaxFolder, string strFaxNumber, string strHospital,
                            string strUserID, string strPcpID, string strPcpName, int intPtLogRecord, string strPatientName,
                            string strComment, string strGeneralHeading, string strNotification, string strDOB, string password, string strSub, string strTo, string additonalComments)
        {
            string strReportPath;
            string strFileName;
            string strFilePath;
            string strCompleteFilePath;
            string sourceFile;

            string timeoffax = DateTime.Now.ToString().Substring(10, 7).Replace(":", "") + DateTime.Now.Millisecond;
            strFaxNumber = strFaxNumber.Substring(0, 3) + strFaxNumber.Substring(4, 3) + strFaxNumber.Substring(8, 4);
            strReportPath = "\\AdDisNotice\\" + strRepType + strHospital + ".rdlc";
            strFileName = strRepType + strHospital + strUserID + "_" + intPtLogRecord.ToString() + "_" + timeoffax + ".pdf";
            strFilePath = strFaxFolder + strRepType + "\\";
            strCompleteFilePath = strFilePath + strFileName;
            sourceFile = strFilePath;

            CreatePDFDocument(strCompleteFilePath, AIMSReportViewer, strCompleteFilePath);

            if (strFilePath.Substring(1,1) == ":")
            {
                strFilePath = MACHINE_PATH + strFilePath.Substring(2);
            }

            bool isFax = false;
            bool isEmail = false;

            if (strNotification == "")
            {
                isFax = true;
            }
            else
            {
                string[] arrNotification = strNotification.Split(new char[] { ';' });

                if (arrNotification.Contains("Fax"))
                {
                    isFax = true;
                }
                if (arrNotification.Contains("Email"))
                {
                    isEmail = true;
                }
            }

            if (strDOB == "N/A") {
                strDOB = "";
            }

            PCPCommunication comm = new PCPCommunication();
            comm.AdditionalComments = additonalComments;
            comm.Comments = strComment;
            comm.CommStatus = "Created";
            //Try to convert DOB to DateTime, revert to default if fails
            try
            {
                comm.DOB = DateTime.Parse(strDOB);
            }
            catch
            {
                comm.DOB = DateTime.Parse("1/1/1900");
            }
            comm.DocumentName = strFileName;
            comm.DocumentPath = strFilePath;
            comm.DocumentType = strRepType;
            //comm.EmailID = 
            comm.FaxCover = FAX_COVER;
            comm.FaxNo = strFaxNumber;
            comm.GenComHeading = strGeneralHeading;
            comm.Hospital = strHospital;
            //comm.Pages = 
            comm.PatientName = strPatientName;
            comm.PLRecord = intPtLogRecord;
            comm.ToUserID = strPcpID;
            comm.UserID = strUserID;
                       
            if (isFax)
            {
                comm.CommType = "Fax";
                UpdateCommTable(comm);
            }

            if (isEmail)
            {
                comm.CommType = "Email";
                if (strTo != "")
                {
                    if (password == "")
                    {
                        password = "aims";
                    }
                }
                string[] ret = CreateEncryptedPDF(sourceFile, strFileName, password);
                string outputFile = ret[0];
                int pages = Convert.ToInt32(ret[1]);
                string strMsg = DateTime.Now.ToString();
                UpdateCommTable(comm);

                try
                {
                    if (HubSecurity.Debug() == true)
                    {
                        strTo = "techsupport@aims.us.com";
                    }
                    SendMail("fax@aims.us.com", strTo, strSub, strMsg, outputFile, "fax", "aimsfx2345", null);
                    UpdateStatus(comm, "Mail Forwarded");
                    strMsg = "<html><body><b>" + strSub + "</b> has been sent to <br/><br/><table><tr><td> <b>PCP</b>: </td><td>" + strPcpName + "</td></tr>" +
                            "<tr><td><b>Mail Id: </b></td><td>" + strTo + "</td></tr>" +
                            "<tr><td><b>Time: </b></td><td>" + DateTime.Now.ToString() + "</td></tr></table><br/><br/> Please access AIMS Hub to view details.</body></html>";

                    string strToCC = AimsHub.Security.HubSecurity.getLoggedInUserID() + "@aims.us.com";
                    SendMail("fax@aims.us.com", strToCC, strSub, strMsg, null, "fax", "aimsfx2345", null);
                }
                catch
                {
                    UpdateStatus(comm, "Send Failed");
                }
                File.Delete(outputFile);
            }
        }

        private void CreatePDFDocument(string strReportPath, Microsoft.Reporting.WebForms.ReportViewer AIMSReport, string strDestinationFilePath)
        {
            Microsoft.Reporting.WebForms.Warning[] warnings = null;
            string[] streamids = null;
            string mimeType = null;
            string encoding = null;
            string extension = null;
            string deviceInfo = null;
            byte[] bytes;
            Microsoft.Reporting.WebForms.LocalReport lr = new Microsoft.Reporting.WebForms.LocalReport();

            lr.ReportPath = strReportPath;
            deviceInfo = "<DeviceInfo><SimplePageHeaders>True</SimplePageHeaders></DeviceInfo>";
            bytes = AIMSReport.LocalReport.Render("PDF", deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
            WriteToFile(strDestinationFilePath, bytes);
        }

        private void WriteToFile(string strPath, byte[] buffer)
        {
            FileStream newFile = new FileStream(strPath, FileMode.Create);
            newFile.Write(buffer, 0, buffer.Length);
            newFile.Close();
        }
        
        private void UpdateCommTable(PCPCommunication comm)
        {
            comm.ScheduledTime = DateTime.Now;
            PatientLogModel db = new PatientLogModel();
            db.PCPCommunications.Add(comm);
            db.SaveChanges();
            db.Dispose();
        }

        private void UpdateStatus(PCPCommunication comm, string status)
        {
            PatientLogModel db = new PatientLogModel();
            comm.CommStatus = status;
            db.Entry(comm).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            db.Dispose();
        }

        private string[] CreateEncryptedPDF(string sourceFile, string strFileName, string password)
        {
            string outputFile = "C:\\Data\\PDFTemp\\";
            if (Directory.Exists(outputFile) == false)
            {
                Directory.CreateDirectory(outputFile);
            }

            outputFile += strFileName;

            PdfReader pReader = new PdfReader(sourceFile);
            int pages = pReader.NumberOfPages;
            PdfEncryptor.Encrypt(pReader, new FileStream(outputFile, FileMode.Append), PdfWriter.ENCRYPTION_AES_128, password, null, PdfWriter.AllowPrinting);
            string[] ret = new string[] { outputFile, pages.ToString() };
            return ret;
        }

        public void SendMail(string from, string to, string subject, string body, string pdfpath, string authusername, string authpassword, string cc)
        {
            //int mailport = 25;
            SmtpClient client = new SmtpClient(MAIL_SERVER, MAIL_PORT);

            MailMessage message = new MailMessage();
            message.From = new MailAddress(from);
            if (to.Contains(";"))
            {
                string[] arrayRecipient = to.Split(new char[] { ';' });
                for (int i = 0; i < arrayRecipient.Length - 1; i++)
                {
                    message.To.Add((arrayRecipient[i]).Trim());
                }
            }
            else
            {
                message.To.Add(to);
            }
            message.Body = body;
            message.Subject = subject;
            if (cc != null && cc != "")
            {
                message.CC.Add(new MailAddress(cc));
            }

            message.IsBodyHtml = true;

            if (pdfpath != null && pdfpath != "")
            {
                message.Attachments.Add(new Attachment(pdfpath));
            }

            if ((authusername != null && authusername != "") && (authpassword != null && authpassword != ""))
            {
                client.Credentials = new System.Net.NetworkCredential(authusername, authpassword);
            }

            client.Send(message);
            message.Dispose();
        }

    }
}