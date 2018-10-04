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
        public User CurrentUser { get; set; }

        private PrincipalContext UserContext;
        private UserPrincipal UserAccount;
        private static Scribe log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "LDAPHandler_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");

        public LDAPHandler(User user)
        {
            CurrentUser = user;
        }

        public bool AuthenticateDomainUser()
        {
            log.WriteLogEntry("Starting AuthenticateUser...");
            bool result = false;
            if (CurrentUser.GetType() == typeof(DomainUser))
            {
                DomainUser user = (DomainUser)CurrentUser;
                string domainName = WebConfigurationManager.AppSettings.Get(user.Domain.DomainName);
                using (UserContext = new PrincipalContext(ContextType.Domain, domainName))
                {
                    if (UserContext.ValidateCredentials(user.UserName, user.UserPwd))
                    {
                        UserAccount = UserPrincipal.FindByIdentity(UserContext, user.UserName);
                        if (UserAccount != null)
                        {
                            log.WriteLogEntry("Success authenticating current user to the domain.");
                            user.EmployeeID = UserAccount.EmployeeId;
                            user.FirstName = UserAccount.GivenName;
                            user.LastName = UserAccount.Surname;
                            user.UserEmail = UserAccount.EmailAddress;
                            user.DomainUpn = UserAccount.UserPrincipalName;
                            user.DomainUserName = UserAccount.SamAccountName;
                            user.Authenicated = true;
                            log.WriteLogEntry(string.Format("Authenticated user {0} {1} {2} {3} {4} {5}", user.FirstName, user.LastName, user.EmployeeID, user.UserName, user.DomainUpn, user.UserEmail));
                            result = true;
                        }
                        else
                            log.WriteLogEntry("Failed to authenticate current user to the domain!");
                    }
                }
            }
            else
                log.WriteLogEntry("Failed current user is not a domain user!");
            log.WriteLogEntry("End AuthenticateUser.");
            return result;
        }

        public bool ValidateDomain(string domainName)
        {
            log.WriteLogEntry("Starting ValidateDomain...");
            bool result = false;
            if (new PrincipalContext(ContextType.Domain, domainName) != null)
                result = true;
            else
                log.WriteLogEntry("Failed to validate domain!");
            log.WriteLogEntry("End ValidateDomain.");
            return result;
        }

        public bool ValidateDomainUpn(string domainName)
        {
            log.WriteLogEntry("Begin ValidateDomainUpn...");
            bool result = false;
            if (CurrentUser.GetType() == typeof(DomainUser))
            {
                DomainUser user = (DomainUser)CurrentUser;
                using (UserContext = new PrincipalContext(ContextType.Domain, domainName))
                {
                    UserAccount = new UserPrincipal(UserContext);
                    UserAccount.UserPrincipalName = user.DomainUpn;
                    log.WriteLogEntry("User UPN " + UserAccount.UserPrincipalName);
                    using (PrincipalSearcher UserSearch = new PrincipalSearcher())
                    {
                        UserSearch.QueryFilter = UserAccount;
                        using (PrincipalSearchResult<Principal> Psr = UserSearch.FindAll())
                        {
                            if (Psr.Count<Principal>() > 0)
                                result = true;
                        }
                    }
                }
            }
            log.WriteLogEntry("End ValidateDomainUpn.");
            return result;
        }

        public int LoadAllDomainUsers(string domainName)
        {
            int result = 0;
            List<DomainUser> Users = new List<DomainUser>();
            using (UserContext = new PrincipalContext(ContextType.Domain, domainName))
            {
                using (PrincipalSearcher UserSearch = new PrincipalSearcher(new UserPrincipal(UserContext)))
                {
                    using (PrincipalSearchResult<Principal> SearchResult = UserSearch.FindAll())
                    {
                        foreach (UserPrincipal ADUser in SearchResult)
                        {
                            DomainUser user = new DomainUser();
                            user.FirstName = ADUser.GivenName;
                            user.LastName = ADUser.Surname;
                            user.DomainUserName = ADUser.SamAccountName;
                            user.DomainUpn = ADUser.UserPrincipalName;
                            user.UserEmail = ADUser.EmailAddress;
                            user.EmployeeID = ADUser.EmployeeId;
                            DirectoryEntry entry = ADUser.GetUnderlyingObject() as DirectoryEntry;
                            if (entry.Properties.Contains("department"))
                                user.Department.DeptName = entry.Properties["department"].Value.ToString();
                            if (entry.Properties.Contains("manager"))
                            {
                                string supName = entry.Properties["manager"].Value.ToString();
                                string[] fields = supName.Split(',');
                                foreach (string field in fields)
                                {
                                    if (field.Substring(0, 3).ToUpper() == "CN=")
                                        user.SupervisorName = field.Substring(3);
                                }
                            }
                            Users.Add(user);
                        }
                    }
                }
            }
            result = Users.Count;
            return result;
        }

        public bool LoadDomainUser(string domainName, string domainUpn)
        {
            log.WriteLogEntry("Begin LoadUser...");
            bool result = false;
            if (CurrentUser.GetType() == typeof(DomainUser))
            {
                DomainUser user = (DomainUser)CurrentUser;
                using (UserContext = new PrincipalContext(ContextType.Domain, domainName))
                {
                    UserAccount = new UserPrincipal(UserContext);
                    UserAccount.UserPrincipalName = domainUpn;
                    log.WriteLogEntry("User UPN " + domainUpn);
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
                            DirectoryEntry entry = UserAccount.GetUnderlyingObject() as DirectoryEntry;
                            if (entry.Properties.Contains("department"))
                            {
                                user.Department.DeptName = entry.Properties["department"].Value.ToString();
                                log.WriteLogEntry("Department " + user.Department);
                            }
                            else
                                log.WriteLogEntry("No department found.");
                            result = true;
                        }
                    }
                }
            }
            log.WriteLogEntry("End LoadUser.");
            return result;
        }
    }
}