using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using VeraAPI.Models;

namespace VeraAPI.Models.Security
{
    public class LDAPHandler
    {
        public DomainUser CurrentUser { get; set; }
        public List<DomainUser> Users { get; set; }

        private PrincipalContext UserContext;
        private UserPrincipal UserAccount;
        private string domainName;
        private Scribe Log;

        public LDAPHandler(string domainName)
        {
            this.Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "LDAPHandler_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            this.domainName = WebConfigurationManager.AppSettings.Get(domainName);
            CurrentUser = new DomainUser();
        }

        public LDAPHandler(string domainName, Scribe Log)
        {
            this.domainName = WebConfigurationManager.AppSettings.Get(domainName);
            CurrentUser = new DomainUser();
            this.Log = Log;
        }

        public bool AuthenticateUser(string userName, string userPwd)
        {
            Log.WriteLogEntry("Begin LDAPHandler AuthenticateUser...");
            bool result = false;
            using (UserContext = new PrincipalContext(ContextType.Domain, domainName))
            {
                if (UserContext.ValidateCredentials(userName, userPwd))
                {
                    UserAccount = UserPrincipal.FindByIdentity(UserContext, userName);
                    if (UserAccount != null)
                    {
                        DomainUser user = new DomainUser();
                        user.UserName = userName;
                        user.UserPwd = userPwd;
                        user.DomainUpn = UserAccount.UserPrincipalName;
                        user.UserEmail = UserAccount.EmailAddress;
                        user.DomainUserName = UserAccount.SamAccountName;
                        user.FirstName = UserAccount.GivenName;
                        user.LastName = UserAccount.Surname;
                        user.EmployeeID = UserAccount.EmployeeId;
                        DirectoryEntry entry = UserAccount.GetUnderlyingObject() as DirectoryEntry;
                        if (entry.Properties.Contains("department"))
                        {
                            user.Department = entry.Properties["department"].Value.ToString();
                            Log.WriteLogEntry("Department " + user.Department);
                        }
                        else
                            Log.WriteLogEntry("No department found.");
                        user.Authenicated = true;
                        user.UserType = User.DomainUser;
                        result = true;
                    }
                }
            }
            Log.WriteLogEntry("End LDAPHandler AuthenticateUser.");
            return result;
        }

        public bool ValidateDomain()
        {
            Log.WriteLogEntry("Begin LDAPHandler ValidateDomain...");
            bool result = false;
            if (new PrincipalContext(ContextType.Domain, domainName) != null)
                result = true;
            Log.WriteLogEntry("End LDAPHandler ValidateDomain.");
            return result;
        }

        public bool ValidateDomainUpn()
        {
            Log.WriteLogEntry("Begin ValidateDomainUpn...");
            bool result = false;
            using (UserContext = new PrincipalContext(ContextType.Domain, domainName))
            {
                UserAccount = new UserPrincipal(UserContext);
                UserAccount.UserPrincipalName = CurrentUser.DomainUpn;
                Log.WriteLogEntry("User UPN " + UserAccount.UserPrincipalName);
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
            Log.WriteLogEntry("End ValidateDomainUpn.");
            return result;
        }

        public int LoadAllUsers()
        {
            int result = 0;
            Users = new List<DomainUser>();
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
                                user.Department = entry.Properties["department"].Value.ToString();
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

        public bool LoadUser()
        {
            Log.WriteLogEntry("Begin LoadUser...");
            bool result = false;
            using (UserContext = new PrincipalContext(ContextType.Domain, domainName))
            {
                UserAccount = new UserPrincipal(UserContext);
                UserAccount.UserPrincipalName = CurrentUser.DomainUpn;
                Log.WriteLogEntry("User UPN " + UserAccount.UserPrincipalName);
                using (PrincipalSearcher UserSearch = new PrincipalSearcher())
                {
                    UserSearch.QueryFilter = UserAccount;
                    using (PrincipalSearchResult<Principal> Psr = UserSearch.FindAll())
                    {
                        UserAccount = (UserPrincipal)Psr.First<Principal>();
                        CurrentUser.FirstName = UserAccount.GivenName;
                        CurrentUser.LastName = UserAccount.Surname;
                        CurrentUser.DomainUserName = UserAccount.SamAccountName;
                        CurrentUser.DomainUpn = UserAccount.UserPrincipalName;
                        CurrentUser.UserEmail = UserAccount.EmailAddress;
                        CurrentUser.EmployeeID = UserAccount.EmployeeId;
                        DirectoryEntry entry = UserAccount.GetUnderlyingObject() as DirectoryEntry;
                        if (entry.Properties.Contains("department"))
                        {
                            CurrentUser.Department = entry.Properties["department"].Value.ToString();
                            Log.WriteLogEntry("Department " + CurrentUser.Department);
                        }
                        else
                            Log.WriteLogEntry("No department found.");
                        if (entry.Properties.Contains("manager"))
                            CurrentUser.Department = entry.Properties["manager"].Value.ToString();
                        result = true;
                    }
                }
            }
            Log.WriteLogEntry("End LoadUser.");
            return result;
        }
    }
}