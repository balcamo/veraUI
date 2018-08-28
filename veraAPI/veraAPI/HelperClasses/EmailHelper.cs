using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using VeraAPI.Models.Tools;
using VeraAPI.Models.OfficeHandler;
using VeraAPI.Models.Security;
using VeraAPI.Models.DataHandler;

namespace VeraAPI.HelperClasses
{
    public class EmailHelper
    {
        private string dbServer;
        private string dbName;
        private ExchangeHandler ExchangeMail;
        private UserDataHandler UserData;
        private User CurrentUser;
        private Scribe Log;

        public EmailHelper()
        {
            Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UIEmailHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            dbServer = WebConfigurationManager.AppSettings.Get("DBServer");
            dbName = WebConfigurationManager.AppSettings.Get("DBName");
            UserData = new UserDataHandler(dbServer, dbName);
            ExchangeMail = new ExchangeHandler();
        }

        public bool ExchangeSendMail()
        {
            Log.WriteLogEntry("Begin SendEmail...");
            bool result = false;
            ExchangeMail.CurrentUser = CurrentUser;
            try
            {
                if (ExchangeMail.ConnectExchangeService())
                {
                    Log.WriteLogEntry("Connection to Exchange service successful.");
                    if (ExchangeMail.SendMail())
                    {
                        result = true;
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
            Log.WriteLogEntry("End SendEmail.");
            return result;
        }

        public bool LoadEmailUser()
        {
            Log.WriteLogEntry("Begin LoadEmailUser...");
            bool result = false;
            if (UserData.LoadDataUser())
            {
                Log.WriteLogEntry("Success load email user from database.");
                CurrentUser = UserData.CurrentUser;
                result = true;
            }
            else
                Log.WriteLogEntry("Failed to load user from database!");
            Log.WriteLogEntry("End LoadEmailUser.");
            return result;
        }
    }
}