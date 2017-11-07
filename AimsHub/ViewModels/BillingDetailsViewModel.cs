using AimsHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AimsHub.ViewModels
{
    public class BillingDetailsViewModel
    {

        public PatientLog Patient;

        public Billing BillingRecord;

        public int Indexer;

        public int IndexerDisplay; //Used for display on page of current index status (Avoids showing patient 0 of x)

        public int SafeIndexerPrev;

        public int SafeIndexerNext;

        public SelectList DXCodesList;

        public SelectList MODCodesList;

        public SelectList POSCodesList;

        public SelectList CPTCodesList;
    }

    
}