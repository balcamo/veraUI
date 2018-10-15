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

        public bool NotifyFinance()
        {
            log.WriteLogEntry("Begin NotifyFinance...");
            bool result = false;
            // Get finance department email
            UserDataHandler userData = new UserDataHandler(dbServer, dbName);
            string financeEmail = userData.GetDepartment(Department.FinanceDept).DeptEmail;
            // Get finance notification


            ExchangeHandler emailHandle = new ExchangeHandler
            {
                EmailSubject = "Notify Department Head",
                RecipientEmailAddress = financeEmail,
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
            log.WriteLogEntry("End NotifyFinance.");
            return result;
        }

        public bool NotifySubmitter(User user)
        {
            log.WriteLogEntry("Begin NotifyDepartmentHead...");
            bool result = false;

            if (user.GetType() == typeof(DomainUser))
            {
                DomainUser domainUser = (DomainUser)user;
                log.WriteLogEntry(string.Format("Current User {0} {1} {2} {3} {4}", user.UserID, domainUser.DomainUpn, domainUser.EmployeeID, domainUser.Department, domainUser.Department.DeptHeadEmail));
                ExchangeHandler emailHandle = new ExchangeHandler
                {
                    EmailSubject = "Notify Submitter",
                    RecipientEmailAddress = domainUser.UserEmail,
                    EmailBody = "<html><body><p>Your request to travel has been approved.</p></body></html>"
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

        public bool NotifyGeneralManager(User user)
        {
            log.WriteLogEntry("Begin NotifyDepartmentHead...");
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
            log.WriteLogEntry("End NotifyDepartmentHead.");
            return result;
        }
    }
}