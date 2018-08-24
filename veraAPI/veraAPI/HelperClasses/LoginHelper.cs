using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using VeraAPI.Models;
using VeraAPI.Models.DataHandler;
using VeraAPI.Models.Security;

namespace VeraAPI.HelperClasses
{
    public class LoginHelper
    {
        public LoginForm LoginCredentials { get; set; }

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
                    result = true;
                }
            }
            else
                Log.WriteLogEntry("Failed LDAPHandler ValidateDomain!");
            Log.WriteLogEntry("End AuthenticateDomainCredentials.");
            return result;
        }

        public bool GetToken()
        {
            Log.WriteLogEntry("Begin GetToken...");
            bool result = false;
            TokenHandle = new TokenHandler();
            Log.WriteLogEntry("End GetToken.");
            return result;
        }
    }
}