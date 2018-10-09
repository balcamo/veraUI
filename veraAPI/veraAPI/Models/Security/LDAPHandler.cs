using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using VeraAPI.Models.Tools;

namespace VeraAPI.Models.Security
{
    public class LDAPHandler
    {
        public DomainUser CurrentUser { get; private set; }

        private PrincipalContext UserContext;
        private UserPrincipal UserAccount;
        private static Scribe log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "LDAPHandler_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");

        public bool AuthenticateDomainUser(DomainUser user)
        {
            log.WriteLogEntry("Starting AuthenticateUser...");
            bool result = false;
            string domainName = WebConfigurationManager.AppSettings.Get(user.Domain.DomainName);
            using (UserContext = new PrincipalContext(ContextType.Domain, domainName))
            {
                if (UserContext.ValidateCredentials(user.UserName, user.UserPwd))
                {
                    UserAccount = UserPrincipal.FindByIdentity(UserContext, user.UserName);
                    if (UserAccount != null)
                    {
                        user.DomainUpn = UserAccount.UserPrincipalName;
                        user.Authenicated = true;
                        log.WriteLogEntry(string.Format("Authenticated user {0} {1} {2}", user.UserName, user.DomainUpn, user.Authenicated));
                        result = true;
                    }
                    else
                        log.WriteLogEntry("Failed to authenticate current user to the domain!");
                }
            }
            log.WriteLogEntry("End AuthenticateUser.");
            return result;
        }

        public bool ValidateDomain(string domainName)
        {
            log.WriteLogEntry("Starting ValidateDomain...");
            bool result = false;
            log.WriteLogEntry("Checking domain " + domainName);
            try
            {
                if (new PrincipalContext(ContextType.Domain, domainName) != null)
                    result = true;
                else
                    log.WriteLogEntry("Failed to validate domain!");
            }
            catch (Exception ex)
            {
                log.WriteLogEntry(ex.Message);
            }
            log.WriteLogEntry("End ValidateDomain.");
            return result;
        }

        public bool LoadDomainUser(string domainName, string domainUpn)
        {
            log.WriteLogEntry("Begin LoadUser...");
            bool result = false;
            DomainUser user = new DomainUser();
            log.WriteLogEntry(string.Format("Domain name {0} Domain UPN {1}", domainName, domainUpn));
            using (UserContext = new PrincipalContext(ContextType.Domain, domainName))
            {
                UserAccount = new UserPrincipal(UserContext)
                {
                    UserPrincipalName = domainUpn
                };
                using (PrincipalSearcher UserSearch = new PrincipalSearcher())
                {
                    UserSearch.QueryFilter = UserAccount;
                    using (PrincipalSearchResult<Principal> Psr = UserSearch.FindAll())
                    {
                        UserAccount = (UserPrincipal)Psr.First<Principal>();
                        user.FirstName = UserAccount.GivenName;
                        user.LastName = UserAccount.Surname;
                        user.DomainUserName = UserAccount.SamAccountName;
                        user.DomainUpn = UserAccount.UserPrincipalName;
                        user.UserEmail = UserAccount.EmailAddress;
                        user.EmployeeID = UserAccount.EmployeeId;
                        this.CurrentUser = user;
                        result = true;
                    }
                }
            }
            log.WriteLogEntry("End LoadUser.");
            return result;
        }
    }
}