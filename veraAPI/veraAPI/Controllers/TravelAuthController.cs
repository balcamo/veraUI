using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VeraAPI.Models;
using VeraAPI.Models.Forms;
using VeraAPI.HelperClasses;
using VeraAPI.Models.Security;
using VeraAPI.Models.DataHandler;
using System.Threading;
using System.Threading.Tasks;


namespace VeraAPI.Controllers
{
    public class TravelAuthController : ApiController
    {
        private FormHelp TravelFormHelper;
        private EmailHelper TravelEmail;
        private UserHelper TravelUser;
        private Scribe Log;

        public TravelAuthController()
        {
            this.Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UITravelAuthController_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
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
        public string Post([FromBody]TravelAuthForm value)
        {
            Log.WriteLogEntry("Begin Post TravelAuthForm...");
            string result = string.Empty;
            value.TemplateID = TemplateIndex.InsertTravelAuth;
            try
            {
                if (value.GetType() == typeof(TravelAuthForm))
                {
                    Log.WriteLogEntry("Verify submitted form is the correct type.");
                    value.setNulls();
                    TravelFormHelper = new FormHelp();
                    TravelEmail = new EmailHelper();
                    TravelUser = new UserHelper();

                    Log.WriteLogEntry("Start Task to submit the travel form.");
                    Task t = Task.Run(() =>
                    {
                        Log.WriteLogEntry("Inside Helper Task.");
                        // change number to constant once file is made
                        if (TravelFormHelper.SubmitForm(value))
                        {
                            Log.WriteLogEntry("Success submitting travel form.");
                            Log.WriteLogEntry("Call Travel email helper load user with user email " + TravelFormHelper.userEmail);
                            if (TravelEmail.LoadUser(TravelFormHelper.userEmail))
                            {
                                Log.WriteLogEntry("Success load email user from database.");
                                if (TravelEmail.SendEmail())
                                    Log.WriteLogEntry("Success send travel authorization email to department head.");
                                else
                                    Log.WriteLogEntry("Failed send travel authorization email.");
                            }
                            else
                                Log.WriteLogEntry("Fail load user from database!");
                        }
                        Log.WriteLogEntry("Fail FormHelp SubmitForm!");
                    });
                    result = "Travel Authorization Form Submitted.";
                }
            }
            catch(Exception e)
            {
                result = "Submit Failed " + e;
            }
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
