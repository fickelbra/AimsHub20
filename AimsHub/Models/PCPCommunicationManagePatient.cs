using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AimsHub.Models
{
    public class PCPCommunicationManagePatient
    {
        PatientLog Patient { get; set; }
        //[Key]
        //public int ID { get; set; }

        //[StringLength(255)]
        //[DisplayName("Physician")]
        //[DefaultValue("Unassigned")]
        //public string Physician { get; set; }

        //[StringLength(255)]
        //[DisplayName("Hospital")]
        //public string Hospital { get; set; }

        //[StringLength(255)]
        //[DisplayName("PCP")]
        //public string PCP_Practice { get; set; }

        //[StringLength(50)]
        //[DisplayName("MRN")]
        //public string MRN_FIN { get; set; }

        //[StringLength(255)]
        //[DisplayName("Patient")]
        //[DisplayFormat(NullDisplayText = "")]
        //public string PatientName { get; set; }

        //[DisplayName("ServiceType")]
        //[StringLength(255)]
        //[DefaultValue("Unassigned")]
        //public string ServiceType { get; set; }

        //[DisplayName("ServiceDate")]
        //[DataType(DataType.DateTime)]
        //[DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, DataFormatString = "{0:MM/dd/yyyy}", NullDisplayText = "")]
        //public DateTime? ServiceDate { get; set; }

        //[DisplayName("RoomNo")]
        //[StringLength(255)]
        //public string RoomNo { get; set; }

        public string CommStatus { get; set; }

    }
}