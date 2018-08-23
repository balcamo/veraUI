using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using VeraAPI.Models;
using VeraAPI.Models.OfficeHandler;
using VeraAPI.Models.Security;
using VeraAPI.Models.DataHandler;

namespace VeraAPI.HelperClasses
{
    public class EmailHelper
    {
        private string dbServer;
        private string dbName;
        private EmailHandler ExchangeMail;
        private UserDataHandler UserData;
        private LoginUser CurrentUser;
        private Scribe Log;

        public EmailHelper()
        {
            Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UIEmailHelper_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
            dbServer = WebConfigurationManager.AppSettings.Get("DBServer");
            dbName = WebConfigurationManager.AppSettings.Get("DBName");
            UserData = new UserDataHandler(dbServer, dbName);
            ExchangeMail = new EmailHandler();
        }

        public bool SendEmail()
        {
            Log.WriteLogEntry("Begin SendEmail...");
            bool result = false;
            if (CurrentUser != null)
            {
                Log.WriteLogEntry("Email user object ready.");
                ExchangeMail = CurrentUser;
                try
                {
                    if (ExchangeMail.ConnectExchangeService())
                    {
                        Log.WriteLogEntry("Connection to Exchange service successful.");
                        ExchangeMail.RecipientEmailAddress = CurrentUser.DepartmentHeadEmail;
                        ExchangeMail.EmailSubject = "Department Head Test Email";
                        ExchangeMail.EmailBody = "Testing send email to department head.";
                        if (ExchangeMail.SendEmail())
                        {
                            Log.WriteLogEntry("Email sent to " + CurrentUser.DepartmentHeadEmail);
                        }
                        else
                            Log.WriteLogEntry("Failed send email!");
                    }
                    else
                        Log.WriteLogEntry("Failed connect to Exchange service!");
                }
                catch (Exception ex)
                {
                    Log.WriteLogEntry("Program error " + ex.Message);
                }
            }
            else
                Log.WriteLogEntry("User object does not exist!");
            Log.WriteLogEntry("End SendEmail.");
            return result;
        }

        public bool LoadUser(string userEmail)
        {
            Log.WriteLogEntry("Begin EmailHelper LoadUser...");
            bool result = false;
            if (UserData.LoadUser(userEmail))
            {
                Log.WriteLogEntry("Success load email user from database.");
                if (UserData.CurrentUser.Department != null)
                    UserData.FillDepartmentHead();
                UserData.FillGeneralManager();
                CurrentUser = UserData.CurrentUser;
                result = true;
            }
            else
                Log.WriteLogEntry("Failed to load user from database!");
            Log.WriteLogEntry("End EmailHelper LoadUser.");
            return result;
        }
    }
}