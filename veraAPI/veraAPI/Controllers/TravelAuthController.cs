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
        private FormHelp helper;
        private EmailHelper TravelEmail;
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
            string result = string.Empty;
            value.TemplateID = TemplateIndex.InsertTravelAuth;
            try
            {
                if (value.GetType() == typeof(TravelAuthForm))
                {
                    value.setNulls();

                    Task t = Task.Run(() =>
                    {
                        helper = new FormHelp();
                        // change number to constant once file is made
                        if (helper.SubmitForm(value))
                        {
                            TravelEmail = new EmailHelper();
                            if (TravelEmail.LoadUser(helper.userEmail))
                            {
                                TravelEmail.SendEmail();
                            }
                            result = "Travel Authorization Form Submitted.";
                        }
                    });
                    //Thread helpThread = new Thread(authHelper.SubmitAuthForm);

                    //helpThread.Start(authForm);
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
