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
        private ExchangeHandler emailHandle;
        private Scribe log;

        public EmailHelper()
        {
            log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "EmailHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            dbServer = WebConfigurationManager.AppSettings.Get("LoginServer");
            dbName = WebConfigurationManager.AppSettings.Get("LoginDB");
            CurrentUser = new User();
        }

        public EmailHelper(User user)
        {
            log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "EmailHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            dbServer = WebConfigurationManager.AppSettings.Get("DBServer");
            dbName = WebConfigurationManager.AppSettings.Get("DBName");
            CurrentUser = user;
        }

        public bool ExchangeSendMail()
        {
            log.WriteLogEntry("Begin ExchangeSendMail...");
            bool result = false;

            if (CurrentUser.GetType() == typeof(DomainUser))
            {
                DomainUser user = (DomainUser)CurrentUser;
                emailHandle = new ExchangeHandler(user);
                try
                {
                    if (emailHandle.ConnectExchangeService())
                    {
                        log.WriteLogEntry("Connection to Exchange service successful.");
                        if (emailHandle.SendMail())
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
            }
            log.WriteLogEntry("End ExchangeSendMail.");
            return result;
        }

        public bool NotifyDepartmentHead()
        {
            log.WriteLogEntry("Begin NotifyDepartmentHead...");
            bool result = false;

            if (CurrentUser.GetType() == typeof(DomainUser))
            {
                DomainUser user = (DomainUser)CurrentUser;
                log.WriteLogEntry(string.Format("Current User {0} {1} {2} {3} {4}", user.UserID, user.DomainUpn, user.EmployeeID, user.Department, user.DepartmentHeadEmail));
                emailHandle = new ExchangeHandler(user);
                emailHandle.EmailSubject = "Notify Department Head";
                emailHandle.RecipientEmailAddress = user.DepartmentHeadEmail;
                emailHandle.EmailBody = "<html><body><p>There has been a request to travel</p><p>go <a href=\"https://bermuda.verawp.local\"> here to approve</a></p></body></html>";
                try
                {
                    if (emailHandle.ConnectExchangeService())
                    {
                        log.WriteLogEntry("Connection to Exchange service successful.");
                        if (emailHandle.SendMail())
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
            }
            log.WriteLogEntry("End NotifyDepartmentHead.");
            return result;
        }

        public bool LoadDomainEmailUser(string email)
        {
            log.WriteLogEntry("Starting LoadDomainEmailUser...");
            bool result = false;
            DomainUser user = new DomainUser();
            UserDataHandler userDataHandle = new UserDataHandler(user, dbServer, dbName);
            userDataHandle.LoadLoginUser(email);
            log.WriteLogEntry(string.Format("User loaded {0} {1} {2} {3} {4}", user.UserID, user.DomainUpn, user.EmployeeID, user.Department, user.DepartmentHeadEmail));
            CurrentUser = user;
            log.WriteLogEntry("End LoadDomainEmailUser.");
            return result;
        }
    }
}