using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;
using System.Threading.Tasks;
using VeraAPI.Models;
using VeraAPI.Models.Forms;
using VeraAPI.Models.Templates;
using VeraAPI.HelperClasses;
using VeraAPI.Models.Tools;
using VeraAPI.Models.JobService;
using VeraAPI.Models.Security;


namespace VeraAPI.Controllers
{
    public class TravelAuthController : ApiController
    {
        private static Scribe log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "TravelAuthController_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");

        // GET: api/API
        public string Get()
        {
            return "Submitted to the API";
        }

        // GET: api/API/5
        public BaseForm[] Get(string restUserID)
        {
            // call function to get active forms
            log.WriteLogEntry("Begin TravelAuthController GET...");
            BaseForm[] result = new BaseForm[0];
            if (int.TryParse(restUserID, out int userID))
            {
                log.WriteLogEntry("Starting LoginHelper...");
                LoginHelper loginHelp = new LoginHelper();
                if (loginHelp.LoadUserSession(userID))
                {
                    log.WriteLogEntry("Starting FormHelper...");
                    FormHelper formHelp = new FormHelper();
                    if (formHelp.LoadActiveTravelAuthForms(userID) > 0)
                    {
                        result = formHelp.WebForms.ToArray();
                    }
                    else
                    {
                        log.WriteLogEntry("No active travel forms returned!");
                    }
                }
                else
                {
                    log.WriteLogEntry("FAILED to load active user session!");
                }
            }
            else
                log.WriteLogEntry("FAILED invalid user id!");
            // return array of active travel auth forms
            log.WriteLogEntry("Forms returned " + result.Length);
            log.WriteLogEntry("End TravelAuthController GET.");
            return result;
        }

        // POST: api/API

        public string Post([FromUri]string restUserID, [FromBody]TravelAuthForm travelAuthForm)
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
                        travelAuthForm.TemplateID = TemplateIndex.InsertTravelAuth;
                        try
                        {
                            if (travelAuthForm.GetType() == typeof(TravelAuthForm))
                            {
                                log.DumpObject(travelAuthForm);
                                log.WriteLogEntry("Starting FormHelper...");
                                FormHelper travelFormHelp = new FormHelper();
                                if (travelFormHelp.SubmitTravelAuthForm(user, travelAuthForm))
                                {
                                    log.WriteLogEntry("Starting EmailHelper...");
                                    EmailHelper emailer = new EmailHelper();
                                    if (userID == int.Parse(travelAuthForm.DHID))
                                    {
                                        travelFormHelp.ApproveTravelAuthForm(userID, travelAuthForm);
                                        emailer.NotifyGeneralManager(user);
                                    }
                                    else if (userID == int.Parse(travelAuthForm.GMID))
                                    {
                                        travelFormHelp.ApproveTravelAuthForm(userID, travelAuthForm);
                                        if (bool.TryParse(travelAuthForm.Advance, out bool advance))
                                        {
                                            if (advance)
                                                emailer.NotifyFinance(Constants.NotificationFinanceAdvance);
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
                            else
                            {
                                log.WriteLogEntry("FAILED submitted form is the wrong type!");
                                result = "Failed to submit travel authorization form! Form not recognized!";
                            }
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

        // PUT: api/API/5
        public void Put(string id, [FromBody]string value)
        {
        }

        // DELETE: api/API/5
        public void Delete(string id)
        {
        }
    }
}
