using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;
using System.Threading.Tasks;
using VeraAPI.Models;
using VeraAPI.Models.Security;
using VeraAPI.Models.Forms;
using VeraAPI.HelperClasses;

namespace VeraAPI.Controllers
{
    public class LDAPController : ApiController
    {
        private LoginHelper LoginHelp;
        private Scribe Log;

        public LDAPController()
        {
            this.Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "LDAPController_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            LoginHelp = new LoginHelper();
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
        public string[] Post([FromBody]LoginForm loginCredentials)
        {
            Log.WriteLogEntry("Begin Post authenticate user...");
            string[] result = new string[] { string.Empty, "0" };
            try
            {
                if (loginCredentials.GetType() == typeof(LoginForm))
                {
                    LoginHelp.LoginCredentials = loginCredentials;
                    if (LoginHelp.AuthenticateDomainCredentials())
                    {
                        LoginHelp.GetDomainToken();
                        string token = LoginHelp.SessionToken;
                        result[0] = token;
                        result[1] = "1";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLogEntry("Post authentication failed! " + ex.Message);
            }
            Log.WriteLogEntry("End Post authenticate user.");
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
