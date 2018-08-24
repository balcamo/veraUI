using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using VeraAPI.Models;
using VeraAPI.Models.DataHandler;
using VeraAPI.Models.Security;
using Newtonsoft.Json;

namespace VeraAPI.HelperClasses
{
    public class LoginHelper
    {
        public LoginForm LoginCredentials { get; set; }
        public string JsonToken { get; private set; }

        private string domainName;
        private User LoginUser;
        private LDAPHandler LDAPHandle;
        private TokenHandler TokenHandle;
        private Scribe Log;

        public LoginHelper()
        {
            Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "LoginHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            domainName = "LocalDomain";
        }

        public bool AuthenticateDomainCredentials()
        {
            Log.WriteLogEntry("Begin AuthenticateDomainCredentials...");
            bool result = false;
            LDAPHandle = new LDAPHandler(domainName);
            if (LDAPHandle.ValidateDomain())
            {
                if (LDAPHandle.AuthenticateUser(LoginCredentials.UserName, LoginCredentials.UserPwd))
                {
                    LoginUser = LDAPHandle.CurrentUser;
                    LoginUser.UserType = 1;
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
            TokenHandle.CurrentUser = LoginUser;
            if (TokenHandle.GenerateDomainToken())
            {
                Log.WriteLogEntry("Success generating domain token.");
                Log.WriteLogEntry("Token string " + TokenHandle.SessionToken.SessionToken);
                JsonToken = JsonConvert.SerializeObject(TokenHandle.SessionToken);
                if (JsonToken != null)
                {
                    Log.WriteLogEntry("Success converting token string to json string");
                    result = true;
                }
                else
                    Log.WriteLogEntry("Failed to convert token string to json string!");
            }
            Log.WriteLogEntry("End GetToken.");
            return result;
        }
    }
}