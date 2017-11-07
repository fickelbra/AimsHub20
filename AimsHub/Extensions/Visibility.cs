using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AimsHub.Extensions
{
    public static class Visibility
    {
        public static MvcHtmlString YesNo(this HtmlHelper htmlHelper, bool yesNo)
        {
            var text = yesNo ? "Yes" : "No";
            return new MvcHtmlString(text);
        }
    }
}