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
    public class UserHelper
    {
        private string dbServer;
        private string dbName;
        private UserDataHandler UserData;
        private User CurrentUser;
        private Scribe Log;

        public UserHelper()
        {
            dbServer = WebConfigurationManager.AppSettings.Get("DBServer");
            dbName = WebConfigurationManager.AppSettings.Get("DBName");
            Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UIUserHelper_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
        }
    }
}