using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using veraAPI.Models;
using veraAPI.HelperClasses;
using System.Threading;
using System.Threading.Tasks;


namespace veraAPI.Controllers
{
    public class APIController : ApiController
    {
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
            string result;
            AuthHelp authHelper = new AuthHelp();
            try {
                TravelAuthForm authForm = new TravelAuthForm(value);
                result = "Submitted Successfully";


                Task t = Task.Run(() =>
                {
                    authHelper.SubmitAuthForm(authForm);
                });
                //Thread helpThread = new Thread(authHelper.SubmitAuthForm);

                //helpThread.Start(authForm);
            }
            catch(Exception e)
            {
                result = "Submit Failed" + e;
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
