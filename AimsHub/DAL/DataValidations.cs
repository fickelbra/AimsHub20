using AimsHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AimsHub.DAL
{
    public class DataValidations
    {
        //This function is meant for Patient Assignment
        //Takes common input from hospital imports and attempts to match the user to an AIMSPhy in our system
        //Expects <lastname>, <firstname> <title?> OR <firstname> <lastname> <title?>
        //Returns their UserID or null otherwise
        public string checkAIMSPhy(PatientLogModel db, string id)
        {
            string firstName = "";
            string lastName = "";
            string[] splitName;

            //Trim any spaces from id first
            id = id.Trim();

            //Get rid of any MD or DO titles
            if (id.Contains(" M.D."))
            {
                id = id.Replace(" M.D.", "");
            }
            if (id.Contains(" MD"))
            {
                id = id.Replace(" MD", "");
            }
            if (id.Contains(" D.O."))
            {
                id = id.Replace(" D.O.", "");
            }
            if (id.Contains(" DO"))
            {
                id = id.Replace(" DO", "");
            }

            //Determine order of name
            if (id.Contains(","))
            {
                splitName = id.Split(new char[] { ',' });
                lastName = splitName[0].Trim();
                firstName = splitName[1].Trim();
            }
            else if (id.Contains(" "))
            {
                splitName = id.Split(new char[] { ' ' });
                lastName = splitName[1].Trim();
                firstName = splitName[0].Trim();
            }

            try
            {
                string userID = (from u in db.Users
                                 join ud in db.UserDetails on u.UserID equals ud.UserID
                                 where ud.UserType == "AIMSPhy" && (u.FirstName.Trim() == firstName && u.LastName.Trim() == lastName)
                                 select u.UserID).First();
                return userID;
            }
            catch
            {
                return null;
            }        
        }

        //This function is meant for Patient Assignment
        //Takes common input from hospital imports and attempts to match the user to an AIMSPhy in our system that has privileges at the hospital provided
        //Expects <lastname>, <firstname> <title?> OR <firstname> <lastname> <title?>
        //Returns their UserID or null otherwise
        public string checkAIMSPhyAndHosp(PatientLogModel db, string id, string hosp)
        {
            string firstName = "";
            string lastName = "";
            string[] splitName;

            //Trim any spaces from id first
            id = id.Trim();

            //Get rid of any MD or DO titles
            if (id.Contains(" M.D."))
            {
                id = id.Replace(" M.D.", "");
            }
            if (id.Contains(" MD"))
            {
                id = id.Replace(" MD", "");
            }
            if (id.Contains(" D.O."))
            {
                id = id.Replace(" D.O.", "");
            }
            if (id.Contains(" DO"))
            {
                id = id.Replace(" DO", "");
            }

            //Determine order of name
            if (id.Contains(","))
            {
                splitName = id.Split(new char[] { ',' });
                lastName = splitName[0].Trim();
                firstName = splitName[1].Trim();
            }
            else if (id.Contains(" "))
            {
                splitName = id.Split(new char[] { ' ' });
                lastName = splitName[1].Trim();
                firstName = splitName[0].Trim();
            }

            try
            {
                string userID = (from u in db.Users
                                 join ud in db.UserDetails on u.UserID equals ud.UserID
                                 where ud.UserType == "AIMSPhy" && (u.FirstName.Trim() == firstName && u.LastName.Trim() == lastName)
                                 select u.UserID).First();
                return userID;
            }
            catch
            {
                return null;
            }
        }

        //This function checks to see if the PCP is a valid PCP in our system.
        //It does not check based on site
        //Expects <lastname>, <firstname> <title?> OR <firstname> <lastname> <title?>
        //Returns their UserID or null otherwise
        public string checkPCP(PatientLogModel db, string id)
        {
            string firstName = "";
            string lastName = "";
            string[] splitName;

            //Trim any spaces from id first
            id = id.Trim();

            //Get rid of any MD or DO titles
            if (id.Contains(" M.D."))
            {
                id = id.Replace(" M.D.", "");
            }
            if (id.Contains(" MD"))
            {
                id = id.Replace(" MD", "");
            }
            if (id.Contains(" D.O."))
            {
                id = id.Replace(" D.O.", "");
            }
            if (id.Contains(" DO"))
            {
                id = id.Replace(" DO", "");
            }

            //Determine order of name
            if (id.Contains(","))
            {
                splitName = id.Split(new char[] { ',' });
                lastName = splitName[0].Trim();
                firstName = splitName[1].Trim();
            }
            else if (id.Contains(" "))
            {
                splitName = id.Split(new char[] { ' ' });
                lastName = splitName[1].Trim();
                firstName = splitName[0].Trim();
            }

            try
            {
                string userID = (from u in db.Users
                                 join ud in db.UserDetails on u.UserID equals ud.UserID
                                 where ud.UserType == "RefPhy" && (u.FirstName.Trim() == firstName && u.LastName.Trim() == lastName)
                                 select u.UserID).First();
                return userID;
            }
            catch
            {
                return null;
            }
        }

        //This function checks to see if the PCP is a valid PCP in our system and also assigned to the hospital location provided
        //Expects <lastname>, <firstname> <title?> OR <firstname> <lastname> <title?>
        //Returns their UserID or null otherwise
        public string checkPCPandHosp(PatientLogModel db, string id, string hosp)
        {
            string firstName = "";
            string lastName = "";
            string[] splitName;

            //Trim any spaces from id first
            id = id.Trim();

            //Get rid of any MD or DO titles
            if (id.Contains(" M.D."))
            {
                id = id.Replace(" M.D.", "");
            }
            if (id.Contains(" MD"))
            {
                id = id.Replace(" MD", "");
            }
            if (id.Contains(" D.O."))
            {
                id = id.Replace(" D.O.", "");
            }
            if (id.Contains(" DO"))
            {
                id = id.Replace(" DO", "");
            }

            //Determine order of name
            if (id.Contains(","))
            {
                splitName = id.Split(new char[] { ',' });
                lastName = splitName[0].Trim();
                firstName = splitName[1].Trim();
            }
            else if (id.Contains(" "))
            {
                splitName = id.Split(new char[] { ' ' });
                lastName = splitName[1].Trim();
                firstName = splitName[0].Trim();
            }

            try
            {
                string userID = (from u in db.Users
                                 join ud in db.UserDetails on u.UserID equals ud.UserID
                                 where ud.UserType == "RefPhy" && ud.DefaultHospital == hosp && (u.FirstName.Trim() == firstName && u.LastName.Trim() == lastName)
                                 select u.UserID).First();
                return userID;
            }
            catch
            {
                return null;
            }
        }

        //Takes gender input and returns a valid format
        public string checkGender(PatientLogModel db, string gender)
        {

            string returner;
            try
            {
                returner = (from g in db.Genders
                            where g.Gender1 == gender
                            select g).Single().ToString();
            }
            catch
            {
                if (gender == "M" || gender == "m")
                {
                    returner = "Male";
                }
                else if (gender == "F" || gender == "f")
                {
                    returner = "Female";
                }
                else
                {
                    returner = "Male";
                }
            }

            return returner;
        }

        public static string getFullName(PatientLogModel db, string userID)
        {
            string name;
            try
            {
                var getter = (from u in db.Users where u.UserID == userID select u).Single();
                name = getter.FirstName + ' ' + getter.LastName;
            }
            catch
            {
                name = "<Error retrieving valid fullname>";
            }
            return name;
        }

    }
}