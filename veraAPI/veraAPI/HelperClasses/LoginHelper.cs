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

        private readonly string domainName = WebConfigurationManager.AppSettings.Get("LocalDomain");
        private readonly string dbServer = WebConfigurationManager.AppSettings.Get("LoginServer");
        private readonly string dbName = WebConfigurationManager.AppSettings.Get("LoginDB");
        private static Scribe log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "LoginHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
        private LDAPHandler LDAPHandle;
        private TokenHandler TokenHandle;
        private UserDataHandler UserData;

        public LoginHelper()
        {
            CurrentUser = new User();
        }

        public LoginHelper(User user)
        {
            this.CurrentUser = user;
        }

        public bool InsertDomainLoginUser()
        {
            log.WriteLogEntry("Begin InsertDomainLoginUser...");
            bool result = false;
            if (CurrentUser.GetType() == typeof(DomainUser))
            {
                DomainUser user = (DomainUser)CurrentUser;
                UserData = new UserDataHandler(user, dbServer, dbName);
                if (UserData.InsertLoginUser())
                {
                    result = true;
                }
                else
                    log.WriteLogEntry("Failed inserting domain login user!");
            }
            else
                log.WriteLogEntry("FAILED not a domain user!");
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

        public bool LoginDomainUser(string userName, string userPwd)
        {
            log.WriteLogEntry("Starting LoginDomainUser...");
            bool result = false;
            if (CurrentUser.GetType() == typeof(DomainUser))
            {
                DomainUser user = (DomainUser)CurrentUser;
                user.UserName = userName;
                user.UserPwd = userPwd;
                log.WriteLogEntry("Login username " + userName);
                LDAPHandle = new LDAPHandler(user);
                log.WriteLogEntry("Starting LDAPHandler...");
                if (LDAPHandle.ValidateDomain(domainName))
                {
                    user.Domain.DomainName = domainName;
                    if (LDAPHandle.AuthenticateDomainUser())
                    {
                        TokenHandle = new TokenHandler(user);
                        log.WriteLogEntry("Starting TokenHandler...");
                        if (TokenHandle.GenerateDomainToken())
                        {
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
                log.WriteLogEntry(string.Format("Current User {0} {1} {2} {3} {4}", user.FirstName, user.LastName, user.UserName, user.UserEmail, user.Token.SessionKey));
            }
            else
                log.WriteLogEntry("FAILED not a domain user!");
            log.WriteLogEntry("End LoginDomainUser.");
            return result;
        }

        public bool LoadDomainLoginUser()
        {
            log.WriteLogEntry("Begin LoadDomainLoginUser...");
            bool result = false;
            if (CurrentUser.GetType() == typeof(DomainUser))
            {
                DomainUser user = (DomainUser)CurrentUser;
                UserData = new UserDataHandler(user, dbServer, dbName);
                if (UserData.InsertLoginUser())
                {
                    result = true;
                }
                else
                    log.WriteLogEntry("Failed loading domain login user!");
            }
            else
                log.WriteLogEntry("FAILED not a domain user!");
            log.WriteLogEntry("End LoadDomainLoginUser.");
            return result;
        }
    }
}