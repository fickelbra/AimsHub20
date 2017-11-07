using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.DirectoryServices;
using System.Configuration;
using AimsHub.Models;

namespace AimsHub.Security
{
    public class HubSecurity
    {
        public static bool Debug()
        {
            return true;
            //return false;
        }

        public static void loadUserAD(string userID)
        {
            //Connect to AD through LDAP
            string path = ConfigurationManager.ConnectionStrings["ADConnectionString"].ConnectionString;
            DirectoryEntry dEntry = new DirectoryEntry(path);

            //Include memberOf attribute and search for user
            DirectorySearcher dSearch = new DirectorySearcher(dEntry);
            dSearch.PropertiesToLoad.Add("memberOf");
            dSearch.PropertiesToLoad.Add("displayName");
            dSearch.PropertiesToLoad.Add("givenName");
            dSearch.PropertiesToLoad.Add("SN");
            dSearch.PropertiesToLoad.Add("sAMAccountName");
            dSearch.PropertiesToLoad.Add("pwdLastSet");
            dSearch.Filter = "(&(objectClass=user)(sAMAccountName=" + userID + "))";

            //Report error if user is not found
            SearchResult result = dSearch.FindOne();
            if (result == null)
            {
                throw new Exception("Something went wrong with Active Directory. Please contact IT.");
            }

            //Set common properties in session
            //HttpContext.Current.Session.Add("DisplayName", result.Properties["displayName"][0].ToString());
            HttpContext.Current.Session.Add("FirstName", result.Properties["givenName"][0].ToString());
            HttpContext.Current.Session.Add("LastName", result.Properties["SN"][0].ToString());
            HttpContext.Current.Session.Add("UserID", result.Properties["sAMAccountName"][0].ToString());

            HttpContext.Current.Session.Add("isAdmin", false);
            HttpContext.Current.Session.Add("isBilling", false);
            HttpContext.Current.Session.Add("isSiteLeader", false);
            HttpContext.Current.Session.Add("isPhysician", false);
            HttpContext.Current.Session.Add("isPracAdmin", false);
            HttpContext.Current.Session.Add("isPCPManager", false);
            HttpContext.Current.Session.Add("isScheduleAdmin", false);
            HttpContext.Current.Session.Add("isExpired", false);

            //Get domain password expiration policy, and compare it to last time user reset password
            //Flag IsExpired if needed

            //Get root of domain
            //DirectorySearcher ds = new DirectorySearcher(dEntry,"(objectClass=*)",null,SearchScope.Base);
            //SearchResult sr = ds.FindOne();
            //TimeSpan maxPwdAge = TimeSpan.MinValue;
            ////Get maximum password age
            //if (sr.Properties.Contains("maxPwdAge"))
            //{
            //    maxPwdAge = TimeSpan.FromTicks((long)sr.Properties["maxPwdAge"][0]);
            //}
            //TimeSpan test = TimeSpan.MinValue;
            //test = TimeSpan.FromTicks((long)result.Properties["pwdLastSet"][0]);
            //HttpContext.Current.Session.Add("TESTdomainPolicy", maxPwdAge.ToString());
            //HttpContext.Current.Session.Add("TESTlastReset", test.ToString());

            //Set user roles
            int propertyCount;
            propertyCount = result.Properties["memberOf"].Count;
            for (int i = 0; i < propertyCount; i++)
            {
                switch (result.Properties["memberOf"][i].ToString())
                {
                    case "CN=HubAdmin,OU=Security Groups,OU=MyBusiness,DC=AIMS,DC=local":
                        HttpContext.Current.Session["isAdmin"] = true;
                        break;
                    case "CN=HubBilling,OU=Security Groups,OU=MyBusiness,DC=AIMS,DC=local":
                        HttpContext.Current.Session["isBilling"] = true;
                        break;
                    case "CN=HubSiteLeader,OU=Security Groups,OU=MyBusiness,DC=AIMS,DC=local":
                        HttpContext.Current.Session["isSiteLeader"] = true;
                        break;
                    case "CN=HubPhysician,OU=Security Groups,OU=MyBusiness,DC=AIMS,DC=local":
                        HttpContext.Current.Session["isPhysician"] = true;
                        break;
                    case "CN=HubPracAdmin,OU=Security Groups,OU=MyBusiness,DC=AIMS,DC=local":
                        HttpContext.Current.Session["isPracAdmin"] = true;
                        break;
                    case "CN=HubPCPManager,OU=Security Groups,OU=MyBusiness,DC=AIMS,DC=local":
                        HttpContext.Current.Session["isPCPManager"] = true;
                        break;
                    case "CN=ScheduleAdmin,OU=Security Groups,OU=MyBusiness,DC=AIMS,DC=local":
                        HttpContext.Current.Session["isScheduleAdmin"] = true;
                        break;
                }
            }
        }

        public static void ChangePassword(AccountChangeModel model)
        {
            try
            {
                //Connect to AD through LDAP
                string path = ConfigurationManager.ConnectionStrings["ADConnectionString"].ConnectionString;
                //DirectoryEntry dEntry = new DirectoryEntry(path)
                DirectoryEntry dEntry = new DirectoryEntry(path, "AIMS\\" + model.UserID, model.Password,AuthenticationTypes.Secure);

                //Include memberOf attribute and search for user
                var obj = dEntry.NativeObject;
                DirectorySearcher dSearch = new DirectorySearcher(dEntry);
                dSearch.Filter = "(SAMAccountName=" + model.UserID + ")";
                dSearch.PropertiesToLoad.Add("cn");

                SearchResult result = dSearch.FindOne();
                if (result != null)
                {
                    DirectoryEntry userEntry = result.GetDirectoryEntry();

                    if (userEntry != null)
                    {
                        userEntry.Invoke("ChangePassword", new object[] { model.Password, model.newPassword });
                        userEntry.CommitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string getLoggedInFirstName()
        {
            if (Debug())
            {
                return "Rajika";
            }
            else
            {
                return HttpContext.Current.Session["FirstName"].ToString();
            }
        }

        public static string getLoggedInLastName()
        {
            if (Debug())
            {
                return "Munasinghe";
            }
            else
            {
                return HttpContext.Current.Session["LastName"].ToString();
            }
        }

        public static string getLoggedInDisplayName()
        {
            if (Debug())
            {
                return "Rajika Munasinghe";
            }
            else
            {
                return getLoggedInFirstName() + " " + getLoggedInLastName();
            }
        }

        public static string getLoggedInUserID()
        {
            if (Debug())
            {
                return "RMunasinghe";
            }
            else
            {
                return HttpContext.Current.Session["UserID"].ToString();
            }
        }
        public static bool isExpired
        {
            get
            {
                if (Debug())
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean(HttpContext.Current.Session["isExpired"]);
                }
            }
        }

        public static bool isBilling
        {
            get
            {
                if (Debug())
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean(HttpContext.Current.Session["isBilling"]);
                }
            }
        }

        public static bool isPhysician
        {
            get
            {
                if (Debug())
                {
                    return true;
                }
                else
                {
                    return Convert.ToBoolean(HttpContext.Current.Session["isPhysician"]);
                }
            }
        }

        public static bool isAdmin
        {
            get
            {
                if (Debug())
                {
                    return true;
                }
                else
                {
                    return Convert.ToBoolean(HttpContext.Current.Session["isAdmin"]);
                }
            }
        }

        public static bool isSiteLeader
        {
            get
            {
                if (Debug())
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean(HttpContext.Current.Session["isSiteLeader"]);
                }
            }
        }

        public static bool isPracAdmin
        {
            get
            {
                if (Debug())
                {
                    return true;
                }
                else
                {
                    return Convert.ToBoolean(HttpContext.Current.Session["isPracAdmin"]);
                }
            }
        }

        public static bool isPCPManager
        {
            get
            {
                if (Debug())
                {
                    return true;
                }
                else
                {
                    return Convert.ToBoolean(HttpContext.Current.Session["isPCPManager"]);
                }
            }
        }

        public static bool isScheduleAdmin
        {
            get
            {
                if (Debug())
                {
                    return true;
                }
                else
                {
                    return Convert.ToBoolean(HttpContext.Current.Session["isScheduleAdmin"]);
                }
            }
        }
    }

    
}