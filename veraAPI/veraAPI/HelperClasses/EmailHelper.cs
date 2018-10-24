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
        private readonly string dbServer = WebConfigurationManager.AppSettings.Get("DBServer");
        private readonly string dbName = WebConfigurationManager.AppSettings.Get("DBName");
        private Scribe log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "EmailHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");

        public bool NotifyDepartmentHead(User user)
        {
            log.WriteLogEntry("Begin NotifyDepartmentHead...");
            bool result = false;

            if (user.GetType() == typeof(DomainUser))
            {
                DomainUser domainUser = (DomainUser)user;
                log.WriteLogEntry(string.Format("Current User {0} {1} {2} {3} {4}", user.UserID, domainUser.DomainUpn, domainUser.EmployeeID, domainUser.Department, domainUser.Department.DeptHeadEmail));
                ExchangeHandler emailHandle = new ExchangeHandler
                {
                    EmailSubject = "Notify Department Head",
                    RecipientEmailAddress = domainUser.Department.DeptHeadEmail,
                    EmailBody = "<html><body><p>There has been a request to travel</p><p>go <a href=\"https://bermuda.verawp.local/?route=travel\"> here to approve</a></p></body></html>"
                };
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

        public bool NotifyFinance(int notification)
        {
            log.WriteLogEntry("Begin NotifyFinance...");
            bool result = false;
            string emailBody = string.Empty;
            switch (notification)
            {
                case 0:
                    emailBody = "<html><body><p>Notifying Finance there is a new travel advance.</p></body></html>";
                    break;
                case 1:
                    emailBody = "<html><body><p>Notifying Finance there is a new travel recap.</p></body></html>";
                    break;
            }
            UserDataHandler userData = new UserDataHandler(dbServer, dbName);
            string financeEmail = userData.GetDepartment(Department.FinanceDept).DeptEmail;
            ExchangeHandler emailHandle = new ExchangeHandler
            {
                EmailSubject = "Notify Finance",
                RecipientEmailAddress = financeEmail,
                EmailBody = emailBody
            };
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
            log.WriteLogEntry("End NotifyFinance.");
            return result;
        }

        public bool NotifySubmitter(string email, int notification)
        {
            log.WriteLogEntry("Begin NotifySubmitter...");
            bool result = false;
            string emailBody = string.Empty;
            switch (notification)
            {
                case 0:
                    {
                        emailBody = "<html><body><p>Your request to travel has been <b>DENIED.</b></p></body></html>";
                        break;
                    }
                case 1:
                    {
                        emailBody = "<html><body><p>Your request to travel has been <b>APPROVED.</b></p></body></html>";
                        break;
                    }
                case 2:
                    {
                        emailBody = "<html><body><p>Your travel advance is being processed by Finance.</p></body></html>";
                        break;
                    }
                case 3:
                    {
                        emailBody = "<html><body><p>Your travel recap is being processed by Finance.</p></body></html>";
                        break;
                    }
            }
            ExchangeHandler emailHandle = new ExchangeHandler
            {
                EmailSubject = "Notify Submitter",
                RecipientEmailAddress = email,
                EmailBody = emailBody
            };
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
            return result;
        }

        public bool NotifyGeneralManager(User user)
        {
            log.WriteLogEntry("Begin NotifyGeneralManager...");
            bool result = false;

            if (user.GetType() == typeof(DomainUser))
            {
                DomainUser domainUser = (DomainUser)user;
                log.WriteLogEntry(string.Format("Current User {0} {1} {2} {3} {4}", user.UserID, domainUser.DomainUpn, domainUser.EmployeeID, domainUser.Department, domainUser.Department.DeptHeadEmail));
                ExchangeHandler emailHandle = new ExchangeHandler
                {
                    EmailSubject = "Notify General Manager",
                    RecipientEmailAddress = domainUser.Company.GeneralManagerEmail,
                    EmailBody = "<html><body><p>A department head has approved a request to travel</p><p>go <a href=\"https://bermuda.verawp.local/?route=travel\"> here to approve</a></p></body></html>"
                };
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
            log.WriteLogEntry("End NotifyGeneralManager.");
            return result;
        }
    }
}