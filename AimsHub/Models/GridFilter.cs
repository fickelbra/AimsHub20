using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AimsHub.Models
{
    public class GridFilter
    {
        public enum SortDirections
        {
            Ascending,
            Descending
        }

        public enum FilterTypes
        {
            None,
            Equals,
            Contains,
            StartsWith,
            EndsWith
        }
    }
}