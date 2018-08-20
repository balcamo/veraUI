using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using VeraAPI.Models;

namespace VeraAPI.Models.Security
{
    public class LDAPHandler
    {
        private string userDomain;
        public User CurrentUser { get; set; } = new User();
        public List<User> Users { get; set; }
        public PrincipalContext UserContext { get; private set; }
        public UserPrincipal UserAccount { get; private set; }
        private Scribe Log;

        public LDAPHandler(string userDomain)
        {
            this.userDomain = userDomain;
            this.Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "LDAPHandler_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
        }

        public LDAPHandler(string userDomain, Scribe Log)
        {
            this.userDomain = userDomain;
            this.Log = Log;
        }

        public bool ValidateUser(string userName, string userPwd)
        {
            bool result = false;
            using (UserContext = new PrincipalContext(ContextType.Domain, userDomain))
            {
                if (UserContext.ValidateCredentials(userName, userPwd))
                {
                    UserAccount = UserPrincipal.FindByIdentity(UserContext, userName);
                    if (UserAccount != null)
                        result = true;
                }
            }
            return result;
        }

        public int LoadAllADUsers()
        {
            int result = 0;
            Users = new List<User>();
            using (UserContext = new PrincipalContext(ContextType.Domain, userDomain))
            {
                using (PrincipalSearcher UserSearch = new PrincipalSearcher(new UserPrincipal(UserContext)))
                {
                    using (PrincipalSearchResult<Principal> SearchResult = UserSearch.FindAll())
                    {
                        foreach (UserPrincipal ADUser in SearchResult)
                        {
                            User user = new User();
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

        public bool LoadUser(string userPrincipalName)
        {
            Log.WriteLogEntry("Begin LoadUser...");
            bool result = false;
            using (UserContext = new PrincipalContext(ContextType.Domain, userDomain))
            {
                UserAccount = new UserPrincipal(UserContext);
                UserAccount.UserPrincipalName = userPrincipalName;
                Log.WriteLogEntry("User UPN " + UserAccount.UserPrincipalName);
                using (PrincipalSearcher UserSearch = new PrincipalSearcher())
                {
                    UserSearch.QueryFilter = UserAccount;
                    using (PrincipalSearchResult<Principal> Psr = UserSearch.FindAll())
                    {
                        UserAccount = (UserPrincipal)Psr.First<Principal>();
                        CurrentUser = new User();
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