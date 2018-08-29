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
        private Scribe Log;

        public LDAPController()
        {
            this.Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "LDAPController_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
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
            Log.WriteLogEntry("Begin Post authenticate user...");
            string result = string.Empty;
            try
            {
                if (loginCredentials.GetType() == typeof(LoginForm))
                {
                    LoginHelper loginHelp = new LoginHelper();
                    Log.WriteLogEntry("Success login credentials are the correct type.");
                    loginHelp.LoginCredentials = loginCredentials;
                    if (loginHelp.AuthenticateDomainCredentials())
                    {
                        Log.WriteLogEntry("Success authenticate domain credentials.");
                        if (loginHelp.GetDomainToken())
                        {
                            Log.WriteLogEntry("Success getting domain json web token.");
                            result = loginHelp.CurrentUser.SessionToken;
                            if (loginHelp.InsertDomainLoginUser())
                            {
                                Log.WriteLogEntry("Success inserting domain login user.");
                            }
                            else
                                Log.WriteLogEntry("Failed inserting domain login user!");
                        }
                        else
                            Log.WriteLogEntry("Failed getting domain json web token!");
                    }
                    else
                        Log.WriteLogEntry("Failed authenticate domain credentials!");
                }
                else
                    Log.WriteLogEntry("Failed login credentials are the wrong type!");
            }
            catch (Exception ex)
            {
                Log.WriteLogEntry("Post authentication failed! " + ex.Message);
            }
            Log.WriteLogEntry("Return result " + result);
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
