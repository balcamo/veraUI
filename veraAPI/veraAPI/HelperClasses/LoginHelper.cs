using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using VeraAPI.Models.DataHandler;
using VeraAPI.Models.Security;
using VeraAPI.Models.Forms;
using VeraAPI.Models.Tools;
using Newtonsoft.Json;

namespace VeraAPI.HelperClasses
{
    public class LoginHelper
    {
        public LoginForm LoginCredentials { get; set; }
        public User CurrentUser { get; private set; }

        private string DomainName;
        private string DbServer;
        private string DbName;
        private LDAPHandler LDAPHandle;
        private TokenHandler TokenHandle;
        private UserDataHandler UserData;
        private Scribe Log;

        public LoginHelper()
        {
            Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "LoginHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            DomainName = "LocalDomain";
            DbServer = WebConfigurationManager.AppSettings.Get("LoginServer");
            DbName = WebConfigurationManager.AppSettings.Get("LoginDB");
        }

        public bool AuthenticateDomainCredentials()
        {
            Log.WriteLogEntry("Begin AuthenticateDomainCredentials...");
            bool result = false;
            LDAPHandle = new LDAPHandler(DomainName);
            if (LDAPHandle.ValidateDomain())
            {
                if (LDAPHandle.AuthenticateUser(LoginCredentials.UserName, LoginCredentials.UserPwd))
                {
                    CurrentUser = LDAPHandle.CurrentUser;
                    Log.WriteLogEntry(string.Format("Current User {0} {1} {2} {3}", CurrentUser.FirstName, CurrentUser.LastName, CurrentUser.UserName, CurrentUser.UserEmail));
                    result = true;
                }
            }
            else
                Log.WriteLogEntry("Failed LDAPHandler ValidateDomain!");
            Log.WriteLogEntry("End AuthenticateDomainCredentials.");
            return result;
        }

        public bool GetDomainToken()
        {
            Log.WriteLogEntry("Begin GetToken...");
            bool result = false;
            TokenHandle = new TokenHandler();
            TokenHandle.CurrentUser = CurrentUser;
            if (TokenHandle.GenerateDomainToken())
            {
                Log.WriteLogEntry("Success generating domain login token." + CurrentUser.LoginToken);
                result = true;
            }
            Log.WriteLogEntry("End GetToken.");
            return result;
        }

        public bool InsertDomainLoginUser()
        {
            Log.WriteLogEntry("Begin InsertLoginUser...");
            bool result = false;
            Log.WriteLogEntry(string.Format("DbServer {0} DbName {1}", DbServer, DbName));
            UserData = new UserDataHandler(DbServer, DbName);
            UserData.CurrentUser = CurrentUser;
            if (UserData.InsertDomainLoginUser())
            {
                Log.WriteLogEntry("Success inserting domain login user.");
                result = true;
            }
            else
                Log.WriteLogEntry("Failed inserting domain login user!");
            Log.WriteLogEntry("End InsertLoginUser.");
            return result;
        }
    }
}