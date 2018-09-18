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
        private Scribe log;

        public TravelAuthController()
        {
            this.log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "TravelAuthController_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
        }

        // GET: api/API
        public string Get()
        {
            return "Submitted to the API";
        }

        // GET: api/API/5
        public BaseForm[] Get(string tokenHeader)
        {
            // call function to get active forms
            log.WriteLogEntry("Starting Get active travel forms...");
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

        // POST: api/API
        public string Post([FromBody]TravelAuthForm travelAuthForm)
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
                    JobHeader job = new JobHeader();
                    log.WriteLogEntry("Starting FormHelper...");

                    // SubmitForm loads the template for the travel auth form
                    // this provides database table information
                    // insert the form data into the system form database
                    if (travelFormHelp.SubmitTravelAuthForm())
                    {
                        log.WriteLogEntry("Success submitting travel form.");
                        /** Functionality not needed at this time.
                         * 
                        // JobHeader inherits from template and adds job specific information
                        job = (JobHeader)travelFormHelp.Template;
                        job.FormDataID = travelFormHelp.WebForm.FormDataID;
                        JobHelper jobHelp = new JobHelper(job);

                        // Insert into job database t
                        if (jobHelp.InsertFormJob())
                        {
                            log.WriteLogEntry("Success inserting form job.");
                            jobID = jobHelp.Job.JobID;
                        }
                        **/
                    }
                    else
                        log.WriteLogEntry("Fail FormHelp SubmitForm!");
                    //log.WriteLogEntry("Job ID " + jobID);

                    /** var postForm = Task.Run(() =>
                    {
                        log.WriteLogEntry("Inside post form helper task.");
                        // change number to constant once file is made
                        TravelFormHelp.WebForm = travelAuthForm;
                        if (TravelFormHelp.SubmitForm())
                        {
                            log.WriteLogEntry("Success submitting travel form.");
                            job = (JobHeader)TravelFormHelp.Template;
                            job.FormDataID = TravelFormHelp.WebForm.FormDataID;
                        }
                        else
                            log.WriteLogEntry("Fail FormHelp SubmitForm!");
                        return job;
                    });

                    var postJob = postForm.ContinueWith((x) => 
                    {
                        log.WriteLogEntry("Inside post job helper task.");
                        int jobID = 0;
                        if (x.Result != null)
                        {
                            JobHelp.Job = x.Result;
                            if (JobHelp.InsertFormJob())
                            {
                                log.WriteLogEntry("Success inserting form job.");
                                jobID = JobHelp.Job.JobID;
                            }
                        }
                        return jobID;
                    }); **/

                    result = "Travel Authorization Form Submitted.";
                }
                else
                    log.WriteLogEntry("Failed submitted form is the wrong type!");
            }
            catch(Exception e)
            {
                result = "Failed Travel Authorization Submit " + e.Message;
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
