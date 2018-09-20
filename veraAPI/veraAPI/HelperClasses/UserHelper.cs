﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using VeraAPI.Models.DataHandler;
using VeraAPI.Models.Security;
using VeraAPI.Models.Forms;
using VeraAPI.Models.Tools;

namespace VeraAPI.HelperClasses
{
    public class UserHelper
    {
        private string dbServer;
        private string dbName;
        private User CurrentUser;
        private UserDataHandler userData;
        private Scribe log;

        public UserHelper()
        {
            log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UserHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            dbServer = WebConfigurationManager.AppSettings.Get("DBServer");
            dbName = WebConfigurationManager.AppSettings.Get("DBName");
            this.CurrentUser = new User();
        }

        public UserHelper(User user)
        {
            log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UserHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            dbServer = WebConfigurationManager.AppSettings.Get("DBServer");
            dbName = WebConfigurationManager.AppSettings.Get("DBName");
            this.CurrentUser = user;
        }

        // WHAT DATA IS BEING LOADED?
        public bool LoadUserData(string email)
        {
            log.WriteLogEntry("Begin LoadUserData...");
            bool result = false;
            userData = new UserDataHandler(CurrentUser, dbServer, dbName);
            if (userData.LoadUserData(email))
            {
                log.WriteLogEntry("Success loading user data.");
                result = true;
            }
            else
                log.WriteLogEntry("Failed loading user data!");
            log.WriteLogEntry("End LoadUserData.");
            return result;
        }

        // WHY ISN"T THIS DONE WHEN WE LOAD THE DATA
        // This method allows for specifically retrieving the user ID
        // When a new user logs in, their information is retrieved from active directory and does not include user ID
        // There is no corresponding user id in active directory
        public bool FillUserID()
        {
            log.WriteLogEntry("Begin FillUserID...");
            bool result = false;
            userData = new UserDataHandler(CurrentUser, dbServer, dbName);
            if (userData.FillUserID())
            {
                log.WriteLogEntry("Success filling user ID.");
                result = true;
            }
            else
                log.WriteLogEntry("Failed filling user ID!");
            log.WriteLogEntry("End FillUserID.");
            return result;
        }

        public bool FillDepartmentHead()
        {
            log.WriteLogEntry("Begin FillDepartmentHead...");
            bool result = false;
            userData = new UserDataHandler(CurrentUser, dbServer, dbName);
            if (userData.FillDepartmentHead())
            {
                log.WriteLogEntry("Success filling user department head.");
                result = true;
            }
            else
                log.WriteLogEntry("Failed filling user department head!");
            log.WriteLogEntry("End FillDepartmentHead.");
            return result;
        }
    }
}