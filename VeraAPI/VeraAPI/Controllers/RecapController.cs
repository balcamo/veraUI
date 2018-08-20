using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VeraAPI.HelperClasses;
using VeraAPI.Models;
using System.Threading.Tasks;

namespace veraAPI.Controllers
{
    public class RecapController : ApiController
    {
        FormHelp helper = new FormHelp();
        // GET: api/Recap
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Recap/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Recap
        public string Post([FromBody]TravelAuthForm value)
        {
            string result = string.Empty;
            string emailType = string.Empty;
            value.TemplateID = TemplateIndex.InsertTravelAuth;
            try
            {
                if (value.GetType() == typeof(TravelAuthForm))
                {
                    //TravelAuthForm authForm = new TravelAuthForm(value);
                    result = "Submitted Successfully";
                    value.setNulls();
                    if(value.TotalReimburse != null) { emailType = "recap"; }
                    Task t = Task.Run(() =>
                    {
                        // change number to constant once file is made
                        helper.UpdateForm(value, emailType);

                    });
                    //Thread helpThread = new Thread(authHelper.UpdateForm);

                    //helpThread.Start(authForm);
                }
            }
            catch (Exception e)
            {
                result = "Submit Failed " + e;
            }
            return result;
        }

        // PUT: api/Recap/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Recap/5
        public void Delete(int id)
        {
        }
    }
}
