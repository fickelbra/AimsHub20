using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace AimsHub.Models
{
    public class SmallButtonModel
    {
        public string Action { get; set; }
        public string Text { get; set; }
        public string Glyph { get; set; }
        public string ButtonType { get; set; }
        public int? Id { get; set; }
        public string Name { get; set; }
        public string ToolTip { get; set; }
        public string ActionParameters
        {
            get
            {
                var param = new StringBuilder("?");
                if (Id != null && Id > 0)
                    param.Append(String.Format("{0}={1}&", "id", Id));

                return param.ToString().Substring(0, param.Length - 1);
            }
        }
    }
}