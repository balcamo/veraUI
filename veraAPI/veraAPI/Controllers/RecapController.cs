using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using VeraAPI.HelperClasses;
using VeraAPI.Models;
using VeraAPI.Models.Tools;
using VeraAPI.Models.Forms;
using VeraAPI.Models.Templates;
using VeraAPI.Models.Security;

namespace VeraAPI.Controllers
{
    public class RecapController : ApiController
    {
        private static Scribe log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "RecapController_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");

        // GET: api/Recap
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Recap/5
        public string Get(string userID)
        {
            return "value";
        }

        // POST: api/Recap
        public string Post(string restUserID, [FromBody]TravelAuthForm value)
        {
            log.WriteLogEntry("Begin RecapController POST...");
            string result = string.Empty;
            log.WriteLogEntry("REST user id " + restUserID);
            if (int.TryParse(restUserID, out int userID))
            {
                log.WriteLogEntry("Starting LoginHelper...");
                LoginHelper loginHelp = new LoginHelper();
                if (loginHelp.LoadUserSession(userID))
                {
                    DomainUser user = new DomainUser();
                    log.WriteLogEntry("Starting UserHelper...");
                    UserHelper userHelp = new UserHelper(user);
                    if (userHelp.LoadDomainUser(userID))
                    {
                        try
                        {
                            log.DumpObject(value);
                            log.WriteLogEntry("Starting FormHelper...");
                            FormHelper travelFormHelp = new FormHelper();
                            if (travelFormHelp.SubmitTravelRecapForm(userID, value))
                            {
                                log.WriteLogEntry("Starting EmailHelper...");
                                EmailHelper emailer = new EmailHelper();
                                emailer.NotifyFinance(Constants.NotificationFinanceRecap);
                                result = "Travel Recap Form Submitted.";
                            }
                            else
                            {
                                result = "Failed to submit recap form!";
                                log.WriteLogEntry(result);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = "ERROR in Travel Recap Submit!\n" + ex.Message;
                            log.WriteLogEntry(result);
                            return result;
                        }
                    }
                    else
                    {
                        result = "Failed to submit travel recap! User not found!";
                        log.WriteLogEntry(result);
                    }
                }
                else
                {
                    result = "Failed to submit travel recap! User not recognized!";
                    log.WriteLogEntry(result);
                }
            }
            else
            {
                result = "Failed to submit travel recap! Invalid user id!";
                log.WriteLogEntry(result);
            }
            log.WriteLogEntry("End RecapController POST.");
            return result;
        }

        // PUT: api/Recap/5
        public void Put(string id, [FromBody]string value)
        {
        }

        // DELETE: api/Recap/5
        public void Delete(string id)
        {
        }
    }
}
