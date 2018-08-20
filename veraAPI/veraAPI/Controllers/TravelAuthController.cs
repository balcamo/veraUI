using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VeraAPI.Models;
using VeraAPI.Models.Forms;
using VeraAPI.HelperClasses;
using System.Threading;
using System.Threading.Tasks;


namespace VeraAPI.Controllers
{
    public class TravelAuthController : ApiController
    {
        FormHelp helper = new FormHelp();

        public TravelAuthController() { }

        // GET: api/API
        public string Get()
        {
            return "Submitted to the API";
        }

        // GET: api/API/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/API
        public string Post([FromBody]TravelAuthForm value)
        {
            string result = string.Empty;
            value.TemplateID = TemplateIndex.InsertTravelAuth;
            try {
                if (value.GetType() == typeof(TravelAuthForm))
                {
                    //TravelAuthForm authForm = new TravelAuthForm(value);
                    result = "Submitted Successfully";
                    value.setNulls();

                    Task t = Task.Run(() =>
                    {
                        // change number to constant once file is made
                        helper.SubmitForm(value);
                        
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
