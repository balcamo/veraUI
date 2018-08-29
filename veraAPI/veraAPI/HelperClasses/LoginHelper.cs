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
            LoginCredentials = new LoginForm();
            CurrentUser = new User();
        }

        public bool AuthenticateDomainCredentials()
        {
            Log.WriteLogEntry("Begin AuthenticateDomainCredentials...");
            bool result = false;
            LDAPHandle = new LDAPHandler(CurrentUser, DomainName);
            if (LDAPHandle.ValidateDomain())
            {
                if (LDAPHandle.AuthenticateUser(LoginCredentials.UserName, LoginCredentials.UserPwd))
                {
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
            TokenHandle = new TokenHandler(CurrentUser);
            if (TokenHandle.GenerateDomainToken())
            {
                Log.WriteLogEntry("Success generating domain login token." + CurrentUser.SessionToken);
                result = true;
            }
            Log.WriteLogEntry("End GetToken.");
            return result;
        }

        public bool InsertDomainLoginUser()
        {
            Log.WriteLogEntry("Begin InsertLoginUser...");
            bool result = false;
            UserData = new UserDataHandler(CurrentUser, DbServer, DbName);
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

        public bool CompareSessionToken()
        {
            Log.WriteLogEntry("Starting ConvertSessionToken.");
            bool result = false;
            TokenHandle = new TokenHandler(CurrentUser);
            if (TokenHandle.TokenToString())
            {
                Log.WriteLogEntry("Success converting session token.");
                result = true;
            }
            else
                Log.WriteLogEntry("Failed converting session token!");
            Log.WriteLogEntry("End ConvertSessionToken.");
            return result;
        }
    }
}