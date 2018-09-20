using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;
using System.Threading.Tasks;
using VeraAPI.Models.Tools;
using VeraAPI.Models.Security;
using VeraAPI.Models.Forms;
using VeraAPI.HelperClasses;

namespace VeraAPI.Controllers
{
    public class LDAPController : ApiController
    {
        private Scribe log;

        public LDAPController()
        {
            this.log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "LDAPController_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
        }

        // GET: api/User
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/User/5
        public int Get(string userEmail)
        {
            return 1;
        }

        public int Get(string userName, string userPwd)
        {
            return 1;
        }

        // POST: api/User
        public Token Post([FromBody]LoginForm loginCredentials)
        {
            log.WriteLogEntry("Starting Post login user...");
            Token result = null;
            if (loginCredentials.GetType() == typeof(LoginForm))
            {
                // Pass login credentials from POST
                LoginHelper loginHelp = new LoginHelper(loginCredentials);
                try
                {
                    log.WriteLogEntry("Starting LoginHelper...");

                    // Validate login credentials against Active Directory
                    // Load email, domain upn, first and last name, user name, employee id, and department
                    // Set user type to internal user (1)
                    if (loginHelp.AuthenticateDomainCredentials())
                    {
                        UserHelper userHelp = new UserHelper(loginHelp.CurrentUser);
                        log.WriteLogEntry("Starting UserHelper...");

                        // Load user id from local login database by user email
                        if (userHelp.FillUserID())
                        {
                            if (userHelp.FillDepartmentHead())
                            {
                                // Generate JSON Web Token based on internal domain credentials
                                if (loginHelp.GetDomainToken())
                                {
                                    // Session Token includes user id from local user database, encoded JSON Web Token, and user type
                                    result = loginHelp.CurrentUser.Token;
                                    log.WriteLogEntry(string.Format("LoginHelper CurrentUser {0} {1} {2} {3}", loginHelp.CurrentUser.UserName, loginHelp.CurrentUser.UserEmail, loginHelp.CurrentUser.UserID, loginHelp.CurrentUser.UserType));

                                    // Insert internal domain user into local session database
                                    if (loginHelp.InsertDomainLoginUser())
                                    {
                                        log.WriteLogEntry("Success inserting domain login user.");
                                    }
                                    else
                                        log.WriteLogEntry("Failed inserting domain login user!");
                                }
                                else
                                    log.WriteLogEntry("Failed getting domain json web token!");
                            }
                            else
                                log.WriteLogEntry("Failed to fill user department head!");
                        }
                        else
                            log.WriteLogEntry("Failed to fill user ID!");
                    }
                    else
                        log.WriteLogEntry("Failed authenticate domain credentials!");
                }
                catch (Exception ex)
                {
                    log.WriteLogEntry("Error posting login " + ex.Message);
                }
            }
            else
                log.WriteLogEntry("Failed login credentials are the wrong type!");
            log.WriteLogEntry(string.Format("Return result {0} {1} {2}", result.UserID, result.SessionKey, result.UserType));
            log.WriteLogEntry("End Post login user.");

            // Return full session token
            return result;
        }

        // PUT: api/User/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/User/5
        public void Delete(int id)
        {
        }
    }
}
