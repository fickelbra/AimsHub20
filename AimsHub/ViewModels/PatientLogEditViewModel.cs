using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AimsHub.Models;
using System.Web.Mvc;

namespace AimsHub.ViewModels
{
    public class PatientLogEditViewModel
    {

        public PatientLog Patient;

        public SelectList HospitalList;

        public SelectList PCPList;

        public SelectList ServiceTypeList;

        public SelectList PatientClassList;

        public SelectList GenderList;

        public SelectList PhysicianList;

        public bool isFaxable;

        public bool alreadyFaxed;

        public int Indexer;

        public int IndexerDisplay; //Used for display on page of current index status (Avoids showing patient 0 of x)

        public int SafeIndexerPrev; 

        public int SafeIndexerNext;
    }
}