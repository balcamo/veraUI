﻿using System;
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
        private static Scribe log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "TravelApprovalController_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");

        // GET: api/TravelApproval
        public BaseForm[] Get(int userID)
        {
            // call function to get active forms
            log.WriteLogEntry("Starting Get active travel forms for approval...");
            BaseForm[] result = null;
            LoginHelper loginHelp = new LoginHelper();
            log.WriteLogEntry("Starting LoginHelper...");
            if (loginHelp.LoadUserSession(userID))
            {
                try
                {
                    FormHelper formHelp = new FormHelper();
                    log.WriteLogEntry("Starting FormHelper...");

                    // Load active forms from system form database by user id = token header
                    formHelp.LoadActiveTravelAuthForms(userID);
                    result = formHelp.WebForms.ToArray();
                }
                catch (Exception ex)
                {
                    log.WriteLogEntry(ex.Message);
                }
            }
            else
            {
                log.WriteLogEntry("FAILED to load active user session!");
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
            LoginHelper loginHelp = new LoginHelper();
            log.WriteLogEntry("Starting LoginHelper...");
            if (loginHelp.LoadUserSession(userID))
            {
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
            }
            else
            {
                log.WriteLogEntry("FAILED to load active user session!");
                result = "Failed to submit travel authorization! User not recognized!";
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
