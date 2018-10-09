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
        public User CurrentUser { get; private set; }

        private readonly string dbServer = WebConfigurationManager.AppSettings.Get("DBServer");
        private readonly string dbName = WebConfigurationManager.AppSettings.Get("DBName");
        private static Scribe log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UserHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
        private UserDataHandler userData;

        public UserHelper()
        {
            this.CurrentUser = new User();
        }

        public UserHelper(User user)
        {
            this.CurrentUser = user;
        }

        public bool LoadPublicUser(int userID)
        {
            log.WriteLogEntry("Begin LoadUserData...");
            bool result = false;

            // Code goes here

            log.WriteLogEntry("End LoadUserData.");
            return result;
        }

        public bool LoadDomainUser(int userID)
        {
            log.WriteLogEntry("Begin LoadDomainUser...");
            bool result = false;
            DomainUser user = new DomainUser();
            userData = new UserDataHandler(user, dbServer, dbName);
            log.WriteLogEntry("Starting UserDataHandler...");
            if (userData.LoadUserData(userID))
            {
                if (userData.LoadCompany())
                {
                    if (userData.LoadDepartment())
                    {
                        if (userData.LoadPosition())
                        {
                            if (userData.LoadSecurityRoles() > 0)
                            {
                                user.Token.AccessKey[0] = user.CompanyNumber;
                                user.Token.AccessKey[1] = user.DepartmentNumber;
                                user.Token.AccessKey[2] = user.PositionNumber;
                                user.Token.AccessKey[3] = user.SecurityRoles.FirstOrDefault().RoleNumber;
                                user.Token.AccessKey[4] = user.SecurityAccess.FirstOrDefault().AccessNumber;
                                log.WriteLogEntry(string.Format("User access key array values {0} {1} {2} {3} {4}", user.CompanyNumber, user.DepartmentNumber, user.PositionNumber, user.SecurityRoles.FirstOrDefault().RoleNumber, user.SecurityAccess.FirstOrDefault().AccessNumber));
                                log.WriteLogEntry(string.Format("User token {0} {1} {2}", user.Token.UserID, user.Token.SessionKey, string.Join(",", user.Token.AccessKey)));
                                this.CurrentUser = user;
                                result = true;
                            }
                            else
                                log.WriteLogEntry("FAILED to load security roles!");
                        }
                        else
                            log.WriteLogEntry("FAILED to load position!");
                    }
                    else
                        log.WriteLogEntry("FAILED to load department!");
                }
                else
                    log.WriteLogEntry("FAILED to load company!");
            }
            else
                log.WriteLogEntry("FAILED loading user data!");
            log.WriteLogEntry("End LoadDomainUser.");
            return result;
        }
    }
}