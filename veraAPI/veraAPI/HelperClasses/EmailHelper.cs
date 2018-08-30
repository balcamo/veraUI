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
        public User CurrentUser { get; private set; }
        private string dbServer;
        private string dbName;
        private ExchangeHandler ExchangeMail;
        private UserDataHandler UserData;
        private Scribe log;

        public EmailHelper(User user)
        {
            log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UIEmailHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            dbServer = WebConfigurationManager.AppSettings.Get("DBServer");
            dbName = WebConfigurationManager.AppSettings.Get("DBName");
            CurrentUser = user;
            ExchangeMail = new ExchangeHandler();
        }

        public bool ExchangeSendMail()
        {
            log.WriteLogEntry("Begin SendEmail...");
            bool result = false;
            ExchangeMail.CurrentUser = CurrentUser;
            try
            {
                if (ExchangeMail.ConnectExchangeService())
                {
                    log.WriteLogEntry("Connection to Exchange service successful.");
                    if (ExchangeMail.SendMail())
                    {
                        result = true;
                    }
                    else
                        log.WriteLogEntry("Failed send email!");
                }
                else
                    log.WriteLogEntry("Failed connect to Exchange service!");
            }
            catch (Exception ex)
            {
                log.WriteLogEntry("Program error " + ex.Message);
            }
            log.WriteLogEntry("End SendEmail.");
            return result;
        }

        public bool LoadEmailUser()
        {
            log.WriteLogEntry("Begin LoadEmailUser...");
            bool result = false;
            if (UserData.LoadDataUser())
            {
                log.WriteLogEntry("Success load email user from database.");
                CurrentUser = UserData.CurrentUser;
                result = true;
            }
            else
                log.WriteLogEntry("Failed to load user from database!");
            log.WriteLogEntry("End LoadEmailUser.");
            return result;
        }
    }
}