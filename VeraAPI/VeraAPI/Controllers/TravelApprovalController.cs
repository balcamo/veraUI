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
        private static Scribe log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "TravelApprovalController_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");

        // GET: api/TravelApproval
        public BaseForm[] Get(string restUserID)
        {
            // call function to get active forms
            log.WriteLogEntry("Starting Get active travel forms for approval...");
            BaseForm[] result = new BaseForm[0];
            if (int.TryParse(restUserID, out int userID))
            {
                log.WriteLogEntry("Starting LoginHelper...");
                LoginHelper loginHelp = new LoginHelper();
                if (loginHelp.LoadUserSession(userID))
                {
                    try
                    {
                        FormHelper formHelp = new FormHelper();
                        log.WriteLogEntry("Starting FormHelper...");
                        if (formHelp.LoadApproverTravelAuthForms(userID))
                            result = formHelp.WebForms.ToArray();
                        else
                            log.WriteLogEntry("No forms found pending approval.");
                    }
                    catch (Exception ex)
                    {
                        result = new BaseForm[0];
                        log.WriteLogEntry(ex.Message);
                    }
                }
                else
                {
                    log.WriteLogEntry("FAILED to load active user session!");
                }
            }
            else
                log.WriteLogEntry("FAILED invalid user id!");
            // return array of active travel auth forms
            log.WriteLogEntry("Forms returned " + result.Count<BaseForm>() + " " + result[0].UserID + " " + result[0].FormDataID);
            log.WriteLogEntry("End Get active travel forms.");
            return result;
        }

        // POST: api/TravelApproval
        public string Post([FromUri]string restUserID, [FromBody]TravelAuthForm travelAuthForm)
        {
            log.WriteLogEntry("Begin Post TravelAuthForm...");
            string result = string.Empty;
            if (int.TryParse(restUserID, out int userID))
            {
                log.WriteLogEntry("Starting LoginHelper...");
                LoginHelper loginHelp = new LoginHelper();
                if (loginHelp.LoadUserSession(userID))
                {
                    DomainUser user = new DomainUser();
                    log.WriteLogEntry("Starting UserHelper...");
                    UserHelper userHelp = new UserHelper(user);
                    if (userHelp.LoadDomainUser(userID))
                    {
                        result = "TravelApprovalController POST.";
                    }
                    else
                    {
                        log.WriteLogEntry("FAILED to load current user data!");
                        result = "Failed to submit travel authorization! User not found!";
                    }
                }
                else
                {
                    log.WriteLogEntry("FAILED to load active user session!");
                    result = "Failed to submit travel authorization! User not recognized!";
                }
            }
            else
                log.WriteLogEntry("FAILED invalid user id!");
            log.WriteLogEntry("End Post TravelAuthForm.");
            return result;
        }

        // POST: api/TravelApproval
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/TravelApproval/5
        public void Put([FromUri]string restUserID, [FromBody]TravelAuthForm travelAuthForm)
        {
            log.WriteLogEntry("Begin TravelAuthForm PUT...");
            if (int.TryParse(restUserID, out int userID))
            {
                log.WriteLogEntry("Starting LoginHelper...");
                LoginHelper loginHelp = new LoginHelper();
                if (loginHelp.LoadUserSession(userID))
                {
                    DomainUser user = new DomainUser();
                    log.WriteLogEntry("Starting UserHelper...");
                    UserHelper userHelp = new UserHelper(user);
                    if (userHelp.LoadDomainUser(userID))
                    {
                        travelAuthForm.TemplateID = TemplateIndex.UpdateTravelAuth;
                        try
                        {
                            if (travelAuthForm.GetType() == typeof(TravelAuthForm))
                            {
                                log.DumpObject(travelAuthForm);
                                log.WriteLogEntry("Starting FormHelper...");
                                FormHelper travelFormHelp = new FormHelper();
                                if (travelFormHelp.ApproveTravelAuthForm(userID, travelAuthForm))
                                {
                                    EmailHelper email = new EmailHelper();
                                    email.NotifySubmitter(user);
                                    if (bool.TryParse(travelAuthForm.Advance, out bool advance))
                                    {
                                        if (bool.TryParse(travelAuthForm.GMApproval, out bool approve))
                                            email.NotifyFinance();
                                    }
                                    log.WriteLogEntry("SUCCESS travel request approved!");
                                }
                                else
                                    log.WriteLogEntry("FAILED submit travel form!");
                            }
                            else
                            {
                                log.WriteLogEntry("FAILED submitted form is the wrong type!");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.WriteLogEntry("FAILED to submit travel authorization form! " + ex.Message);
                        }
                    }
                    else
                    {
                        log.WriteLogEntry("FAILED to load current user data!");
                    }
                }
                else
                {
                    log.WriteLogEntry("FAILED to load active user session!");
                }
            }
            else
                log.WriteLogEntry("FAILED invalid user id!");
            log.WriteLogEntry("End Post TravelAuthForm.");
        }

        // DELETE: api/TravelApproval/5
        public void Delete(string id)
        {
        }
    }
}
