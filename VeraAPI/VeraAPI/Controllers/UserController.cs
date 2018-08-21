using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using VeraAPI.Models;
using VeraAPI.Models.Security;
using VeraAPI.Models.DataHandler;

namespace VeraAPI.Controllers
{
    public class UserController : ApiController
    {
        private User CurrentUser;
        private LDAPHandler VeraLDAP;
        private PrincipalContext UserContext;
        private UserPrincipal UserAccount;
        private Scribe Log;

        public UserController()
        {
            this.Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UIUserController_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
        }

        // GET: api/User
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/User/5
        public int Get(string userEmail)
        {
            int result = 0;
            string userDomain;
            userDomain = WebConfigurationManager.AppSettings.Get("LocalDomain");
            if (userDomain != null)
            {
                Log.WriteLogEntry("localDomain value = " + userDomain);
                CurrentUser = new User();
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

        public int Get(string userName, string userPwd)
        {
            int result = 0;
            string userDomain;
            System.Configuration.Configuration localDomainConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(null);
            System.Configuration.KeyValueConfigurationElement localDomain;
            if (localDomainConfig.AppSettings.Settings.Count > 0)
            {
                localDomain = localDomainConfig.AppSettings.Settings["LocalDomain"];
                if (localDomain != null)
                {
                    userDomain = localDomain.Value;
                    CurrentUser = new User();
                    VeraLDAP = new LDAPHandler(userDomain);
                    if (VeraLDAP.ValidateUser(CurrentUser.UserName, CurrentUser.UserPwd))
                    {
                        CurrentUser.Authenicated = true;
                        result = 1;
                    }
                    else
                    {
                        CurrentUser.Authenicated = false;
                    }
                }
            }
            return result;
        }

        // POST: api/User
        public void Post([FromBody]string value)
        {

        }

        // PUT: api/User/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/User/5
        public void Delete(int id)
        {
        }
    }
}
