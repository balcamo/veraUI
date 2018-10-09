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
            FormHelper recapHelp = new FormHelper();
            string result = string.Empty;
            if (int.TryParse(restUserID, out int userID))
            {
                string emailType = string.Empty;
                value.TemplateID = TemplateIndex.InsertTravelRecap;
                try
                {
                    if (value.GetType() == typeof(TravelAuthForm))
                    {
                        //TravelAuthForm authForm = new TravelAuthForm(value);
                        result = "Submitted Successfully";
                        if (value.TotalReimburse != null) { emailType = "recap"; }
                        Task t = Task.Run(() =>
                        {
                            // change number to constant once file is made
                            recapHelp.UpdateForm(value, emailType);

                        });
                        //Thread helpThread = new Thread(authHelper.UpdateForm);

                        //helpThread.Start(authForm);
                    }
                }
                catch (Exception e)
                {
                    result = "Submit Failed " + e;
                }
            }
            else
                log.WriteLogEntry("FAILED invalid user id!");
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
