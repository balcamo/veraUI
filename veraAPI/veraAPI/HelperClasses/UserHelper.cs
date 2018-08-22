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
        public User CurrentUser { get; set; }
        private Scribe Log;

        public UserHelper()
        {
            Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UIUserHelper_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
            dbServer = WebConfigurationManager.AppSettings.Get("DBServer");
            dbName = WebConfigurationManager.AppSettings.Get("DBName");
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
    }
}