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
        public User CurrentUser { get; set; }

        private string DomainName = "LocalDomain";
        private string DbServer = WebConfigurationManager.AppSettings.Get("LoginServer");
        private string DbName = WebConfigurationManager.AppSettings.Get("LoginDB");
        private LoginForm loginCredentials;
        private LDAPHandler LDAPHandle;
        private TokenHandler TokenHandle;
        private UserDataHandler UserData;
        private static Scribe log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "LoginHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");

        public LoginHelper()
        {
            CurrentUser = new User();
        }

        public LoginHelper(LoginForm loginCredentials)
        {
            this.loginCredentials = loginCredentials;
            CurrentUser = new User();
        }

        public LoginHelper(LoginForm loginCredentials, User user)
        {
            this.loginCredentials = loginCredentials;
            this.CurrentUser = user;
        }

        public bool AuthenticateDomainCredentials()
        {
            log.WriteLogEntry("Starting AuthenticateDomainCredentials...");
            bool result = false;
            DomainUser user = new DomainUser
            {
                UserName = loginCredentials.UserName,
                UserPwd = loginCredentials.UserPwd
            };
            LDAPHandle = new LDAPHandler(user, DomainName);
            log.WriteLogEntry("Starting LDAPHandler...");
            if (LDAPHandle.ValidateDomain())
            {
                if (LDAPHandle.AuthenticateDomainUser())
                {
                    log.WriteLogEntry(string.Format("Current User {0} {1} {2} {3}", user.FirstName, user.LastName, user.UserName, user.UserEmail));
                    CurrentUser = user;
                    result = true;
                }
                else
                    log.WriteLogEntry("Failed authenticate current user to domain!");
            }
            else
                log.WriteLogEntry("Failed LDAPHandler ValidateDomain!");
            log.WriteLogEntry("End AuthenticateDomainCredentials.");
            return result;
        }

        public bool GetDomainToken()
        {
            log.WriteLogEntry("Starting GetDomainToken...");
            bool result = false;
            if (CurrentUser.GetType() == typeof(DomainUser))
            {
                TokenHandle = new TokenHandler(CurrentUser);
                if (TokenHandle.GenerateDomainToken())
                {
                    log.WriteLogEntry("Success generating domain session token.");
                    log.WriteLogEntry("Session key " + CurrentUser.Token.SessionKey);
                    log.WriteLogEntry("User type " + CurrentUser.Token.UserType);
                    result = true;
                }
            }
            else
                log.WriteLogEntry("Failed current user is not a domain user!");
            log.WriteLogEntry("End GetDomainToken.");
            return result;
        }

        public bool InsertDomainLoginUser()
        {
            log.WriteLogEntry("Begin InsertDomainLoginUser...");
            bool result = false;
            UserData = new UserDataHandler(CurrentUser, DbServer, DbName);
            if (UserData.InsertLoginUser())
            {
                log.WriteLogEntry("Success inserting domain login user.");
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
            TokenHandle = new TokenHandler(CurrentUser);
            if (TokenHandle.VerifyToken())
            {
                log.WriteLogEntry("Success converting session token.");
                result = true;
            }
            else
                log.WriteLogEntry("Failed converting session token!");
            log.WriteLogEntry("End ConvertSessionToken.");
            return result;
        }
    }
}