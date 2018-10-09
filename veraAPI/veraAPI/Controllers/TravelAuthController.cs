using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;
using System.Threading.Tasks;
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
            log.WriteLogEntry("Starting TravelAuthController GET...");
            BaseForm[] result = null;
            if (int.TryParse(restUserID, out int userID))
            {
                try
                {
                    log.WriteLogEntry("Starting LoginHelper...");
                    LoginHelper loginHelp = new LoginHelper();
                    if (loginHelp.LoadUserSession(userID))
                    {
                        log.WriteLogEntry("Starting FormHelper...");
                        FormHelper formHelp = new FormHelper();

                        // Load active forms from system form database by user id = token header
                        formHelp.LoadActiveTravelAuthForms(userID);
                        result = formHelp.WebForms.ToArray();
                    }
                }
                catch (Exception ex)
                {
                    log.WriteLogEntry("General program error! " + ex.Message);
                }
            }
            else
                log.WriteLogEntry("FAILED invalid user id!");
            // return array of active travel auth forms
            log.WriteLogEntry("Forms returned " + result.Count<BaseForm>() + " " + result[0].UserID + " " + result[0].FormDataID);
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
                    travelAuthForm.TemplateID = TemplateIndex.InsertTravelAuth;
                    try
                    {
                        if (travelAuthForm.GetType() == typeof(TravelAuthForm))
                        {
                            log.WriteLogEntry("Starting FormHelper...");
                            FormHelper travelFormHelp = new FormHelper(travelAuthForm);
                            if (travelFormHelp.SubmitTravelAuthForm())
                            {
                                EmailHelper emailer = new EmailHelper(loginHelp.CurrentUser);
                                emailer.NotifyDepartmentHead();
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
                    }
                }
                else
                {
                    log.WriteLogEntry("FAILED to load active user session!");
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
