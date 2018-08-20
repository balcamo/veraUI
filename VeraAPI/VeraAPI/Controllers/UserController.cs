using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VeraAPI.Models.Security;
using VeraAPI.Models.DataHandler;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace VeraAPI.Controllers
{
    public class UserController : ApiController
    {
        User CurrentUser;
        LDAPHandler VeraLDAP;
        public PrincipalContext UserContext;
        public UserPrincipal UserAccount;

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
            System.Configuration.Configuration localDomainConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(null);
            System.Configuration.KeyValueConfigurationElement localDomain;
            if (localDomainConfig.AppSettings.Settings.Count > 0)
            {
                localDomain = localDomainConfig.AppSettings.Settings["LocalDomain"];
                if (localDomain != null)
                {
                    userDomain = localDomain.Value;
                    CurrentUser = new User();
                    CurrentUser.AdUpn = userEmail;
                    using (UserContext = new PrincipalContext(ContextType.Domain, userDomain))
                    {
                        UserAccount = new UserPrincipal(UserContext);
                        UserAccount.UserPrincipalName = CurrentUser.AdUpn;
                        using (PrincipalSearcher UserSearch = new PrincipalSearcher())
                        {
                            UserSearch.QueryFilter = UserAccount;
                            using (PrincipalSearchResult<Principal> Psr = UserSearch.FindAll())
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
                                {
                                    CurrentUser.Department = entry.Properties["department"].Value.ToString();
                                }
                                if (entry.Properties.Contains("manager"))
                                    CurrentUser.Department = entry.Properties["manager"].Value.ToString();
                                CurrentUser.Authenicated = true;
                                result = 1;
                            }
                        }
                    }
                    return result;
                }
            }
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
