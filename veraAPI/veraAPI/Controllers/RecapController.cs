using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using VeraAPI.HelperClasses;
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
            log.WriteLogEntry("Begin TravelAuthController POST...");
            string result = string.Empty;
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
                        value.TemplateID = TemplateIndex.InsertTravelAuth;
                        try
                        {
                            log.DumpObject(value);
                            log.WriteLogEntry("Starting FormHelper...");
                            FormHelper travelFormHelp = new FormHelper();
                            if (travelFormHelp.SubmitTravelRecapForm(userID, value))
                            {
                                log.WriteLogEntry("Starting EmailHelper...");
                                EmailHelper emailer = new EmailHelper();
                                if (userID == int.Parse(value.DHID))
                                {
                                    travelFormHelp.ApproveTravelAuthForm(userID, value);
                                    emailer.NotifyGeneralManager(user);
                                }
                                else if (userID == int.Parse(value.GMID))
                                {
                                    travelFormHelp.ApproveTravelAuthForm(userID, value);
                                    if (bool.TryParse(value.Advance, out bool advance))
                                    {
                                        if (advance)
                                            emailer.NotifyFinance();
                                        else
                                            log.WriteLogEntry("No advance requested.");
                                    }
                                    else
                                        log.WriteLogEntry("FAILED to parse travel form advance boolean!");
                                }
                                else
                                {
                                    emailer.NotifyDepartmentHead(user);
                                }
                            }
                            else
                                log.WriteLogEntry("Fail FormHelp SubmitForm!");
                            result = "Travel Authorization Form Submitted.";
                        }
                        catch (Exception ex)
                        {
                            log.WriteLogEntry("FAILED to submit travel authorization form! " + ex.Message);
                            result = "Failed Travel Authorization Submit " + ex.Message;
                            return result;
                        }
                    }
                    else
                    {
                        log.WriteLogEntry("FAILED to load current user data!");
                        result = "Failed to submit travel authorization! User not found!";
                    }
                }
                else
                {
                    log.WriteLogEntry("FAILED to load current user session!");
                    result = "Failed to submit travel authorization! User not recognized!";
                }
            }
            else
                log.WriteLogEntry("FAILED invalid user id!");
            log.WriteLogEntry("End TravelAuthController POST.");
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
