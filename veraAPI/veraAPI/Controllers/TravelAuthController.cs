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


namespace VeraAPI.Controllers
{
    public class TravelAuthController : ApiController
    {
        private FormHelper TravelFormHelp;
        private JobHelper JobHelp;
        private Scribe Log;

        public TravelAuthController()
        {
            this.Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "TravelAuthController_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
        }

        // GET: api/API
        public string Get()
        {
            return "Submitted to the API";
        }

        // GET: api/API/5
        public string Get(string userEmail)
        {
            // call function to get active forms
            return userEmail;
        }

        // POST: api/API
        public string Post([FromBody]TravelAuthForm travelAuthForm)
        {
            Log.WriteLogEntry("Begin Post TravelAuthForm...");
            string result = string.Empty;
            TravelFormHelp = new FormHelper();
            JobHelp = new JobHelper();

            // Get template ID for insert travel authorization from static class TemplateIndex
            travelAuthForm.TemplateID = TemplateIndex.InsertTravelAuth;
            try
            {
                if (travelAuthForm.GetType() == typeof(TravelAuthForm))
                {
                    Log.WriteLogEntry("Start Task to submit the travel form.");
                    Task t = Task.Run(() =>
                    {
                        Log.WriteLogEntry("Inside Helper Task.");
                        // change number to constant once file is made
                        TravelFormHelp.WebForm = travelAuthForm;
                        if (TravelFormHelp.SubmitForm())
                        {
                            Log.WriteLogEntry("Success submitting travel form.");
                            JobHelp.Template = TravelFormHelp.Template;
                            JobHelp.Job.FormDataID = TravelFormHelp.WebForm.FormDataID;
                            if (JobHelp.InsertFormJob())
                            {
                                Log.WriteLogEntry("Success inserting form job.");
                            }
                        }
                        Log.WriteLogEntry("Fail FormHelp SubmitForm!");
                    });
                    result = "Travel Authorization Form Submitted.";
                }
                else
                    Log.WriteLogEntry("Failed submitted form is the wrong type!");
            }
            catch(Exception e)
            {
                result = "Failed Travel Authorization Submit " + e.Message;
            }
            Log.WriteLogEntry("End Post TravelAuthForm.");
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
