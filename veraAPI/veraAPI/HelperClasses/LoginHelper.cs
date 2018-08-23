using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.IdentityModel.Tokens.Jwt;
using VeraAPI.Models;
using VeraAPI.Models.DataHandler;
using VeraAPI.Models.Security;

namespace VeraAPI.HelperClasses
{
    public class LoginHelper
    {
        public LoginUser CurrentUser { get; set; }

        private string dbServer;
        private string dbName;
        private string domainName;
        private UserDataHandler UserData;
        private LDAPHandler UserDomain;
        private Scribe Log;

        public LoginHelper()
        {
            Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UIUserHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            dbServer = WebConfigurationManager.AppSettings.Get("DBServer");
            dbName = WebConfigurationManager.AppSettings.Get("DBName");
            domainName = "LocalDomain";
            CurrentUser = new LoginUser();
        }

        public bool LoadUserData(string userEmail)
        {
            Log.WriteLogEntry("Begin UserHelper LoadUserData...");
            bool result = false;
            UserData = new UserDataHandler(dbServer, dbName);
            if (UserData.LoadUser(userEmail))
            {
                Log.WriteLogEntry("Success load user from database.");
                if (UserData.CurrentUser.Department != null)
                    UserData.FillDepartmentHead();
                UserData.FillGeneralManager();
                CurrentUser = UserData.CurrentUser;
                result = true;
            }
            else
                Log.WriteLogEntry("Failed to load user from database!");
            Log.WriteLogEntry("End UserHelper LoadUserData.");
            return result;
        }

        public bool LoginDomainUser()
        {
            Log.WriteLogEntry("Begin UserHelper LoginUser...");
            bool result = false;
            UserDomain = new LDAPHandler(domainName);
            DomainUser user = null;
            if (UserDomain.ValidateDomain())
            {
                if (CurrentUser.GetType() == typeof(DomainUser))
                {
                    user = (DomainUser)CurrentUser;
                    user.DomainUpn = CurrentUser.UserEmail.Substring(1, CurrentUser.UserEmail.Length - 2);
                    Log.WriteLogEntry("User UPN = " + user.DomainUpn);
                    if (UserDomain.LoginDomain())
                    {

                    }
                }
            }
            else
                Log.WriteLogEntry("localDomain Appsetting not found!");
            Log.WriteLogEntry("Return Result " + result);
            return result;
        }

        public bool ValidateDomainUser(string userEmail)
        {
            Log.WriteLogEntry("Begin UserHelper ValidateDomainUser...");
            bool result = false;
            UserDomain = new LDAPHandler(domainName);
            UserDomain.CurrentUser.DomainUpn = userEmail;
            if (UserDomain.ValidateDomainUpn())
            {
                result = true;
            }
            Log.WriteLogEntry("End UserHelper ValidateDomainUser.");
            return result;
        }

        public void HandleJwtToken()
        {
            if (CurrentUser.LoginToken.GetType() == typeof(JwtSecurityToken))
            {
                JwtSecurityToken token = new JwtSecurityToken(CurrentUser.LoginToken);
            }
    }
}