using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AimsHub.Security
{
    public class ControllerAuthentication
    {
        public class PatientLogAuthorization : AuthorizeAttribute
        {
            public PatientLogAuthorization()
            {
                View = "~/Login/Authorize";
                Master = String.Empty;
            }

            public String View { get; set; }
            public String Master { get; set; }

            public override void OnAuthorization(AuthorizationContext filterContext)
            {
                base.OnAuthorization(filterContext);
                CheckIfUserIsAuthenticated(filterContext);
            }

            private void CheckIfUserIsAuthenticated(AuthorizationContext filterContext)
            {
                //Return nothing if allowed to view these modules
                if (HubSecurity.isPhysician || HubSecurity.isSiteLeader || HubSecurity.isAdmin)
                {
                    return;
                }

                // If here, you're getting an HTTP 401 status code
                var result = new ViewResult { ViewName = View, MasterName = Master };
                filterContext.Result = result;               
            }
        }


    }
}