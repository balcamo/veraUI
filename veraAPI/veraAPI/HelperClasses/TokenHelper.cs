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
    public class TokenHelper
    {
        public TokenForm SessionToken { get; private set; }
        public User CurrentUser { get; private set; }

        private string DomainName;
        private string DbServer;
        private string DbName;
        private TokenHandler TokenHandle;
        private Scribe log;

        public TokenHelper()
        {
            log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "LoginHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            DomainName = "LocalDomain";
            DbServer = WebConfigurationManager.AppSettings.Get("LoginServer");
            DbName = WebConfigurationManager.AppSettings.Get("LoginDB");
        }

        public bool GetDomainToken()
        {
            log.WriteLogEntry("Begin GetToken...");
            bool result = false;
            TokenHandle = new TokenHandler(CurrentUser);
            if (TokenHandle.GenerateDomainToken())
            {
                log.WriteLogEntry("Success generating domain login token." + CurrentUser.SessionToken);
                result = true;
            }
            else
                log.WriteLogEntry("Failed generating domain login token.");
            log.WriteLogEntry("End GetToken.");
            return result;
        }

        public bool CompareSessionToken()
        {
            log.WriteLogEntry("Starting ConvertSessionToken.");
            bool result = false;
            TokenHandle = new TokenHandler(CurrentUser);
            if (TokenHandle.TokenToString())
            {
                log.WriteLogEntry("Success converting session token.");
                result = true;
            }
            else
                log.WriteLogEntry("Failed converting session token!");
            log.WriteLogEntry("End ConvertSessionToken.");
            return result;
        }
    }
}