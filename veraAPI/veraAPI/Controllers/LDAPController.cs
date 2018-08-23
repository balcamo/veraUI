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
using VeraAPI.HelperClasses;

namespace VeraAPI.Controllers
{
    public class LDAPController : ApiController
    {
        private LoginHelper LoginHelp;
        private TokenHelper TokenHelp;
        private Scribe Log;

        public LDAPController()
        {
            this.Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UIUserController_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            LoginHelp = new LoginHelper();
            TokenHelp = new TokenHelper();
        }

        // GET: api/User
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/User/5
        public int Get(string userEmail)
        {
            int result = 0;
            if (LoginHelp.ValidateDomainUser(userEmail))
                result = 1;
            Log.WriteLogEntry("Return Result " + result);
            return result;
        }

        public int Get(string userName, string userPwd)
        {
            return 1;
        }

        // POST: api/User
        public void Post([FromBody]string loginCredentials)
        {
            Log.WriteLogEntry("Begin Post LDAP login...");

            TokenHelp.TokenBody = loginToken;

            Log.WriteLogEntry("End Post LDAP login.");
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
