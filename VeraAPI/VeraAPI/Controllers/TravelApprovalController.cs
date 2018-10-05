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
    public class TravelApprovalController : ApiController
    {
        private Scribe log;

        public TravelApprovalController()
        {
            this.log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "TravelApprovalController_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
        }
        // GET: api/TravelApproval
        public BaseForm[] Get(string tokenHeader)
        {
            // call function to get active forms
            log.WriteLogEntry("Starting Get active travel forms for approval...");
            log.WriteLogEntry("Token Header received " + tokenHeader);
            BaseForm[] result = null;
            try
            {
                FormHelper formHelp = new FormHelper();
                log.WriteLogEntry("Starting FormHelper...");

                // Load active forms from system form database by user id = token header
                formHelp.LoadActiveTravelAuthForms(tokenHeader);
                result = formHelp.WebForms.ToArray();
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

        // POST: api/TravelApproval
        public string Post(int userID, [FromBody]TravelAuthForm travelAuthForm)
        {
            log.WriteLogEntry("Begin Post TravelAuthForm...");
            string result = string.Empty;
            //int jobID = 0;

            // Get template ID for insert travel authorization from static class TemplateIndex
            travelAuthForm.TemplateID = TemplateIndex.InsertTravelAuth;
            try
            {
                if (travelAuthForm.GetType() == typeof(TravelAuthForm))
                {
                    FormHelper travelFormHelp = new FormHelper(travelAuthForm);
                    log.WriteLogEntry("Starting FormHelper...");

                    // SubmitForm loads the template for the travel auth form
                    // this provides database table information
                    // insert the form data into the system form database
                    if (travelFormHelp.SubmitTravelAuthForm())
                    {
                        log.WriteLogEntry("Success submitting travel form.");
                        EmailHelper emailer = new EmailHelper();
                        emailer.LoadDomainEmailUser(travelAuthForm.Email);
                        emailer.NotifyDepartmentHead();
                    }
                    else
                        log.WriteLogEntry("Fail FormHelp SubmitForm!");
                    result = "Travel Authorization Form Submitted.";
                }
                else
                    log.WriteLogEntry("Failed submitted form is the wrong type!");
            }
            catch (Exception e)
            {
                result = "Failed Travel Authorization Submit " + e.Message;
            }
            log.WriteLogEntry("End Post TravelAuthForm.");
            return result;
        }

        // POST: api/TravelApproval
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/TravelApproval/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/TravelApproval/5
        public void Delete(int id)
        {
        }
    }
}
