using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using VeraAPI.Models.DataHandler;
using VeraAPI.Models.Security;
using VeraAPI.Models.Forms;
using VeraAPI.Models.Tools;

namespace VeraAPI.HelperClasses
{
    public class UserHelper
    {
        public User CurrentUser { get; private set; }

        private readonly string dbServer = WebConfigurationManager.AppSettings.Get("DBServer");
        private readonly string dbName = WebConfigurationManager.AppSettings.Get("DBName");
        private static Scribe log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UserHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
        private UserDataHandler userData;

        public UserHelper()
        {
            this.CurrentUser = new User();
        }

        public UserHelper(User user)
        {
            this.CurrentUser = user;
        }

        public bool LoadPublicUser(int userID)
        {
            log.WriteLogEntry("Begin LoadUserData...");
            bool result = false;

            // Code goes here

            log.WriteLogEntry("End LoadUserData.");
            return result;
        }

        public bool LoadDomainUser(User user)
        {
            log.WriteLogEntry("Begin LoadDomainUser...");
            bool result = false;
            if (user.GetType() == typeof(DomainUser))
            {
                DomainUser domainUser = (DomainUser)user;
                log.WriteLogEntry("Starting UserDataHandler...");
                userData = new UserDataHandler(dbServer, dbName);
                domainUser.UserID = userData.GetUserID(domainUser.DomainUpn);
                if (userData.LoadUserData(domainUser.UserID))
                {
                    if (userData.LoadCompany())
                    {
                        if (userData.LoadDepartment())
                        {
                            if (userData.LoadPosition())
                            {
                                if (userData.LoadSecurityRoles() > 0)
                                {
                                    domainUser.Token.UserID = domainUser.UserID;
                                    domainUser.Token.AccessKey[0] = domainUser.CompanyNumber;
                                    domainUser.Token.AccessKey[1] = domainUser.DepartmentNumber;
                                    domainUser.Token.AccessKey[2] = domainUser.PositionNumber;
                                    domainUser.Token.AccessKey[3] = domainUser.SecurityRoles.FirstOrDefault().RoleNumber;
                                    domainUser.Token.AccessKey[4] = domainUser.SecurityAccess.FirstOrDefault().AccessNumber;
                                    log.WriteLogEntry(string.Format("User access key array values {0} {1} {2} {3} {4}", domainUser.CompanyNumber, domainUser.DepartmentNumber, domainUser.PositionNumber, domainUser.SecurityRoles.FirstOrDefault().RoleNumber, domainUser.SecurityAccess.FirstOrDefault().AccessNumber));
                                    log.WriteLogEntry(string.Format("User token access key {0}", string.Join(",", domainUser.Token.AccessKey)));
                                    result = true;
                                }
                                else
                                    log.WriteLogEntry("FAILED to load security roles!");
                            }
                            else
                                log.WriteLogEntry("FAILED to load position!");
                        }
                        else
                            log.WriteLogEntry("FAILED to load department!");
                    }
                    else
                        log.WriteLogEntry("FAILED to load company!");
                }
                else
                    log.WriteLogEntry("FAILED loading user data!");
            }
            else
                log.WriteLogEntry("FAILED not a domain user!");
            log.WriteLogEntry("End LoadDomainUser.");
            return result;
        }

        public bool LoadUserSession(UserSession session)
        {
            log.WriteLogEntry("Starting LoadUserSession...");
            bool result = false;
            string dbServer = WebConfigurationManager.AppSettings.Get("LoginServer");
            string dbName = WebConfigurationManager.AppSettings.Get("LoginDB");

            log.WriteLogEntry("Starting UserDataHandler...");
            UserDataHandler userData = new UserDataHandler(dbServer, dbName);
            if (userData.LoadUserSession(session))
                result = true;
            else
                log.WriteLogEntry("FAILED to load the current user session!");
            log.WriteLogEntry("End LoadUserSession.");
            return result;
        }

        public bool InsertDomainUserSession(DomainUser user)
        {
            log.WriteLogEntry("Begin InsertDomainLoginUser...");
            bool result = false;
            string dbServer = WebConfigurationManager.AppSettings.Get("LoginServer");
            string dbName = WebConfigurationManager.AppSettings.Get("LoginDB");

            UserSession session = new UserSession(user.UserID)
            {
                CompanyNumber = user.CompanyNumber,
                DeptNumber = user.DepartmentNumber,
                PositionNumber = user.PositionNumber,
                DomainNumber = user.CompanyNumber,
                RoleNumber = user.SecurityRoles.FirstOrDefault().RoleNumber,
                AccessLevel = user.SecurityAccess.FirstOrDefault().AccessNumber,
                UserName = user.UserName,
                UserEmail = user.UserEmail,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmployeeID = user.EmployeeID,
                DeptName = user.Department.DeptName,
                DeptHeadName = user.Department.DeptHeadName,
                DeptHeadEmail = user.Department.DeptHeadEmail,
                DomainUserName = user.DomainUserName,
                DomainUpn = user.DomainUpn,
                Authenicated = user.Authenicated,
                StartTime = DateTime.Now
            };

            log.WriteLogEntry("Starting UserDataHandler...");
            UserDataHandler userData = new UserDataHandler(dbServer, dbName);
            if (userData.InsertUserSession(session))
            {
                result = true;
            }
            else
                log.WriteLogEntry("Failed inserting domain login user!");
            log.WriteLogEntry("End InsertDomainLoginUser.");
            return result;
        }
    }
}