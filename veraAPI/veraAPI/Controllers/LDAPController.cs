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
        public string Post([FromBody]LoginForm loginCredentials)
        {
            log.WriteLogEntry("Starting Post login user...");
            string result = string.Empty;
            if (loginCredentials.GetType() == typeof(LoginForm))
            {
                log.WriteLogEntry("Login Credentials " + loginCredentials.UserName + " " + loginCredentials.UserPwd);
                LoginHelper loginHelp = new LoginHelper(loginCredentials);
                try
                {
                    log.WriteLogEntry("Starting LoginHelper...");
                    if (loginHelp.AuthenticateDomainCredentials())
                    {
                        if (loginHelp.GetDomainToken())
                        {
                            result = loginHelp.CurrentUser.SessionToken;
                            log.WriteLogEntry(string.Format("LoginHelper CurrentUser {0} {1} {2} {3}", loginHelp.CurrentUser.FirstName, loginHelp.CurrentUser.LastName, loginHelp.CurrentUser.UserEmail, loginHelp.CurrentUser.UserType));
                            UserHelper userHelp = new UserHelper(loginHelp.CurrentUser);
                            log.WriteLogEntry("Starting UserHelper...");
                            if (userHelp.FillUserID())
                            {
                                if (loginHelp.InsertDomainLoginUser())
                                {
                                    log.WriteLogEntry("Success inserting domain login user.");
                                }
                                else
                                    log.WriteLogEntry("Failed inserting domain login user!");
                            }
                            else
                                log.WriteLogEntry("Failed to fill user ID!");
                        }
                        else
                            log.WriteLogEntry("Failed getting domain json web token!");
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
            log.WriteLogEntry("Return result " + result);
            log.WriteLogEntry("End Post login user.");
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
