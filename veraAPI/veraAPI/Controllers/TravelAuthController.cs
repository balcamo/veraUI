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
        public BaseForm[] Get(int userID)
        {
            // call function to get active forms
            log.WriteLogEntry("Starting Get active travel forms...");
            log.WriteLogEntry("Token Header received " + userID);
            BaseForm[] result = null;
            try
            {
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
                log.WriteLogEntry(ex.Message);
            }

            // return array of active travel auth forms
            log.WriteLogEntry("Forms returned " + result.Count<BaseForm>() + " " + result[0].UserID + " " + result[0].FormDataID);
            log.WriteLogEntry("End Get active travel forms.");
            return result;
        }

        // POST: api/API
        public string Post(int userID, [FromBody]TravelAuthForm travelAuthForm)
        {
            log.WriteLogEntry("Begin Post TravelAuthForm...");
            string result = string.Empty;

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
            log.WriteLogEntry("End Post TravelAuthForm.");
            return result;
        }

        // PUT: api/API/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/API/5
        public void Delete(int id)
        {
        }
    }
}
