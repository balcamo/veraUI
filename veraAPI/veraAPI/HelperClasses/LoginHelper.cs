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
    public class LoginHelper
    {
        public User CurrentUser { get; private set; }
        public UserSession CurrentSession { get; private set; }

        private readonly string domainName = WebConfigurationManager.AppSettings.Get("LocalDomain");
        private static Scribe log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "LoginHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");

        public LoginHelper()
        {
            CurrentUser = new User();
        }

        public LoginHelper(User user)
        {
            this.CurrentUser = user;
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
                DomainUpn = user.DomainUpn,
                SessionKey = user.Token.SessionKey,
                Authenicated = user.Authenicated
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

        public bool CompareSessionToken()
        {
            log.WriteLogEntry("Starting ConvertSessionToken.");
            bool result = false;
            TokenHandler tokenHandle = new TokenHandler();
            if (tokenHandle.VerifyToken())
            {
                log.WriteLogEntry("Success converting session token.");
                result = true;
            }
            else
                log.WriteLogEntry("Failed converting session token!");
            log.WriteLogEntry("End ConvertSessionToken.");
            return result;
        }

        public bool LoginDomainUser(DomainUser user)
        {
            log.WriteLogEntry("Starting LoginDomainUser...");
            bool result = false;
            string dbServer = WebConfigurationManager.AppSettings.Get("DBServer");
            string dbName = WebConfigurationManager.AppSettings.Get("DBName");

            log.WriteLogEntry("Starting LDAPHandler...");
            LDAPHandler ldapHandle = new LDAPHandler();
            if (ldapHandle.ValidateDomain(domainName))
            {
                if (ldapHandle.AuthenticateDomainUser(user))
                {
                    log.WriteLogEntry("Starting TokenHandler...");
                    TokenHandler tokenHandle = new TokenHandler();
                    if (tokenHandle.GenerateDomainToken(user))
                    {
                        log.WriteLogEntry("Starting UserDataHandler...");
                        UserDataHandler userData = new UserDataHandler(dbServer, dbName);
                        user.UserID = userData.GetUserID(user.DomainUpn);
                        log.WriteLogEntry(string.Format("Current user {0} {1} {2} {3} {4}", user.UserID, user.UserName, user.DomainUpn, user.Token.SessionKey, user.Authenicated));
                        result = true;
                    }
                    else
                        log.WriteLogEntry("FAILED to generate domain session key!");
                }
                else
                    log.WriteLogEntry("FAILED authenticate current user to domain!");
            }
            else
                log.WriteLogEntry("FAILED to validate the domain!");
            log.WriteLogEntry("End LoginDomainUser.");
            return result;
        }

        public bool LoadUserSession(int userID)
        {
            log.WriteLogEntry("Starting LoadUserSession...");
            bool result = false;
            string dbServer = WebConfigurationManager.AppSettings.Get("LoginServer");
            string dbName = WebConfigurationManager.AppSettings.Get("LoginDB");

            log.WriteLogEntry("Starting UserDataHandler...");
            UserSession session = new UserSession(userID);
            UserDataHandler userData = new UserDataHandler(session, dbServer, dbName);
            userData.LoadUserSession(userID);

            log.WriteLogEntry("End LoadUserSession.");
            return result;
        }
    }
}