using System;
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
        private readonly string dbServer = WebConfigurationManager.AppSettings.Get("DBServer");
        private readonly string dbName = WebConfigurationManager.AppSettings.Get("DBName");
        private readonly User CurrentUser;
        private UserDataHandler userData;
        private static Scribe log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UserHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");

        public UserHelper()
        {
            this.CurrentUser = new User();
        }

        public UserHelper(User user)
        {
            this.CurrentUser = user;
        }

        public bool LoadPublicUser()
        {
            log.WriteLogEntry("Begin LoadUserData...");
            bool result = false;
            userData = new UserDataHandler(CurrentUser, dbServer, dbName);
            if (userData.LoadUserData())
            {
                log.WriteLogEntry("Success loading user data.");
                result = true;
            }
            else
                log.WriteLogEntry("Failed loading user data!");
            log.WriteLogEntry("End LoadUserData.");
            return result;
        }

        public bool LoadDomainUser()
        {
            log.WriteLogEntry("Begin LoadDomainUser...");
            bool result = false;
            if (CurrentUser.GetType() == typeof(DomainUser))
            {
                DomainUser user = (DomainUser)CurrentUser;
                userData = new UserDataHandler(user, dbServer, dbName);
                if (userData.LoadUserData())
                {
                    if (userData.LoadCompany())
                    {
                        if (userData.LoadDepartment())
                        {
                            if (userData.LoadPosition())
                            {
                                if (userData.LoadSecurityRoles())
                                {
                                    user.Token.AccessKey[0] = user.CompanyNumber;
                                    user.Token.AccessKey[1] = user.DeptNumber;
                                    user.Token.AccessKey[2] = user.PositionNumber;
                                    user.Token.AccessKey[3] = user.SecurityRoles.FirstOrDefault().RoleNumber;
                                    log.WriteLogEntry(string.Format("User token {0} {1} {2}", user.Token.UserID, user.Token.SessionKey, string.Join(",", user.UserAccessKey)));
                                    result = true;
                                }
                            }
                        }
                        else
                            log.WriteLogEntry("FAILED to load department!");
                    }
                    else
                        log.WriteLogEntry("FAILED to load company!");
                }
                else
                    log.WriteLogEntry("FAILED loading user data!");
            }
            else
                log.WriteLogEntry("FAILED not a domain user!");
            log.WriteLogEntry("End LoadDomainUser.");
            return result;
        }
    }
}