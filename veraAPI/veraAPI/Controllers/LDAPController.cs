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
        private static Scribe log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "LDAPController_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");

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
            log.WriteLogEntry("Starting Post for login credentials...");
            Token result = null;
            if (loginCredentials.GetType() == typeof(LoginForm))
            {
                DomainUser user = new DomainUser
                {
                    UserName = loginCredentials.UserName,
                    UserPwd = loginCredentials.UserPwd
                };
                try
                {
                    log.WriteLogEntry("Starting LoginHelper...");
                    LoginHelper loginHelp = new LoginHelper();
                    if (loginHelp.LoginDomainUser(user))
                    {
                        log.WriteLogEntry("Starting UserHelper...");
                        UserHelper userHelp = new UserHelper(user);
                        if (userHelp.LoadDomainUser(user.UserID))
                        {
                            log.WriteLogEntry(string.Format("Current User {0} {1} {2} {3} {4}", user.UserName, user.UserEmail, user.UserID, user.Department.DeptName, user.Company.CompanyName));

                            // Insert internal domain user into local session database
                            log.WriteLogEntry("Starting LoginHelper...");
                            if (loginHelp.InsertDomainUserSession(user))
                            {
                                log.WriteLogEntry("Success inserting domain login user.");
                                result = user.Token;
                            }
                            else
                                log.WriteLogEntry("FAILED inserting domain login user!");
                        }
                        else
                            log.WriteLogEntry("FAILED to load domain user!");
                    }
                    else
                        log.WriteLogEntry("FAILED authenticate domain credentials!");
                }
                catch (Exception ex)
                {
                    log.WriteLogEntry("Error posting login " + ex.Message);
                }
                log.WriteLogEntry(string.Format("Return result {0} {1} {2}", result.UserID, result.SessionKey, string.Join(",", user.Token.AccessKey)));
            }
            else
                log.WriteLogEntry("Failed login credentials are the wrong type!");
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
