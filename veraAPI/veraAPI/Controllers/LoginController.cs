using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Claims;
using VeraAPI.Models.Tools;
using VeraAPI.Models.Security;
using VeraAPI.Models.Forms;
using VeraAPI.HelperClasses;

namespace VeraAPI.Controllers
{
    [Authorize]
    public class LoginController : ApiController
    {
        private static Scribe log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "LoginController_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");

        // GET: api/Login
        public IEnumerable<string> Get()
        {
            //
            // The Scope claim tells you what permissions the client application has in the service.
            // In this case we look for a scope value of user_impersonation, or full access to the service as the user.
            //
            if (ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/scope").Value != "user_impersonation")
            {
                throw new HttpResponseException(new HttpResponseMessage { StatusCode = HttpStatusCode.Unauthorized, ReasonPhrase = "The Scope claim does not contain 'user_impersonation' or scope claim not found" });
            }
            
            // NameIdentifier claim contains an immutable, unique identifier for the user.
            Claim subject = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier);

            return new string[] { "value1", "value2" };
        }

        // GET: api/Login/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Login
        public Token Post([FromBody]LoginForm loginForm)
        {
            log.WriteLogEntry("Starting Post for login credentials...");
            Token result = null;
            if (ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/scope").Value == "user_impersonation")
            {
                if (loginForm.GetType() == typeof(LoginForm))
                {
                    DomainUser user = new DomainUser
                    {
                        UserName = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Upn).Value,
                        DomainUpn = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Upn).Value,
                        Authenicated = true
                    };

                    log.WriteLogEntry(string.Format("Authenticated user {0} {1} {2}", user.UserName, user.DomainUpn, user.Authenicated));
                    log.WriteLogEntry("Starting UserHelper...");
                    UserHelper userHelp = new UserHelper();
                    if (userHelp.LoadDomainUser(user))
                    {
                        log.WriteLogEntry(string.Format("Domain user {0} {1} {2} {3} {4} {5} {6}", user.UserName, user.DomainUpn, user.UserEmail, user.Company.CompanyName, user.Department.DeptName, user.Position.PostiionTitle, user.Authenicated));
                        log.WriteLogEntry("Starting LoginHelper...");
                        if (userHelp.InsertDomainUserSession(user))
                        {
                            result = user.Token;
                        }
                        else
                            log.WriteLogEntry("FAILED inserting domain login user!");
                    }
                    else
                        log.WriteLogEntry("FAILED to load domain user!");

                    log.WriteLogEntry(string.Format("Return result {0} {1}", result.UserID, string.Join(",", user.Token.AccessKey)));
                }
                else
                    log.WriteLogEntry("Failed login credentials are the wrong type!");
            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage { StatusCode = HttpStatusCode.Unauthorized, ReasonPhrase = "The Scope claim does not contain 'user_impersonation' or scope claim not found" });
            }
            log.WriteLogEntry("End Post login user.");

            // Return full session token
            return result;
        }

        // PUT: api/Login/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Login/5
        public void Delete(int id)
        {
        }
    }
}
