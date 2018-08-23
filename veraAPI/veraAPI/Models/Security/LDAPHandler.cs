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
            domainName = WebConfigurationManager.AppSettings.Get(domainName);
            CurrentUser = new DomainUser();
        }

        public LDAPHandler(string domainName, Scribe Log)
        {
            domainName = WebConfigurationManager.AppSettings.Get(domainName);
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
                        result = true;
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
                UserAccount.UserPrincipalName = CurrentUser.AdUpn;
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

        public bool LoginDomain()
        {
            int result = 0;
                Log.WriteLogEntry("localDomain value = " + userDomain);
                CurrentUser = new LoginUser();
                CurrentUser.AdUpn = userEmail.Substring(1, userEmail.Length - 2);
                Log.WriteLogEntry("User UPN = " + CurrentUser.AdUpn);
                using (UserContext = new PrincipalContext(ContextType.Domain, userDomain))
                {
                    UserAccount = new UserPrincipal(UserContext);
                    UserAccount.UserPrincipalName = CurrentUser.AdUpn;
                    using (PrincipalSearcher UserSearch = new PrincipalSearcher())
                    {
                        UserSearch.QueryFilter = UserAccount;
                        using (PrincipalSearchResult<Principal> Psr = UserSearch.FindAll())
                        {
                            Log.WriteLogEntry("Principal search result " + Psr.Count<Principal>());
                            if (Psr.Count<Principal>() > 0)
                            {
                                UserAccount = (UserPrincipal)Psr.First<Principal>();
                                CurrentUser.FirstName = UserAccount.GivenName;
                                CurrentUser.LastName = UserAccount.Surname;
                                CurrentUser.AdSam = UserAccount.SamAccountName;
                                CurrentUser.AdUpn = UserAccount.UserPrincipalName;
                                CurrentUser.UserEmail = UserAccount.EmailAddress;
                                CurrentUser.EmployeeID = UserAccount.EmployeeId;
                                DirectoryEntry entry = UserAccount.GetUnderlyingObject() as DirectoryEntry;
                                if (entry.Properties.Contains("department"))
                                    CurrentUser.Department = entry.Properties["department"].Value.ToString();
                                if (entry.Properties.Contains("manager"))
                                    CurrentUser.Department = entry.Properties["manager"].Value.ToString();
                                CurrentUser.Authenicated = true;
                                Log.WriteLogEntry(string.Format("{0} {1} {2} {3}", CurrentUser.FirstName, CurrentUser.LastName, CurrentUser.EmployeeID, CurrentUser.Department));
                                result = 1;
                            }
                            else
                                Log.WriteLogEntry("User not found in directory!");
                        }
                    }
                }
            }
            else
                Log.WriteLogEntry("localDomain Appsetting not found!");
            Log.WriteLogEntry("Return Result " + result);
            return result;
        }

        public int LoadAllUsers()
        {
            int result = 0;
            Users = new List<LoginUser>();
            using (UserContext = new PrincipalContext(ContextType.Domain, userDomain))
            {
                using (PrincipalSearcher UserSearch = new PrincipalSearcher(new UserPrincipal(UserContext)))
                {
                    using (PrincipalSearchResult<Principal> SearchResult = UserSearch.FindAll())
                    {
                        foreach (UserPrincipal ADUser in SearchResult)
                        {
                            LoginUser user = new LoginUser();
                            user.FirstName = ADUser.GivenName;
                            user.LastName = ADUser.Surname;
                            user.AdSam = ADUser.SamAccountName;
                            user.AdUpn = ADUser.UserPrincipalName;
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
                UserAccount.UserPrincipalName = CurrentUser.AdUpn;
                Log.WriteLogEntry("User UPN " + UserAccount.UserPrincipalName);
                using (PrincipalSearcher UserSearch = new PrincipalSearcher())
                {
                    UserSearch.QueryFilter = UserAccount;
                    using (PrincipalSearchResult<Principal> Psr = UserSearch.FindAll())
                    {
                        UserAccount = (UserPrincipal)Psr.First<Principal>();
                        CurrentUser.FirstName = UserAccount.GivenName;
                        Log.WriteLogEntry("First Name " + CurrentUser.FirstName);
                        CurrentUser.LastName = UserAccount.Surname;
                        Log.WriteLogEntry("Last Name " + CurrentUser.LastName);
                        CurrentUser.AdSam = UserAccount.SamAccountName;
                        CurrentUser.AdUpn = UserAccount.UserPrincipalName;
                        CurrentUser.UserEmail = UserAccount.EmailAddress;
                        CurrentUser.EmployeeID = UserAccount.EmployeeId;
                        Log.WriteLogEntry("Employee ID " + CurrentUser.EmployeeID);
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