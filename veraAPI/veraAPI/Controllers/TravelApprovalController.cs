using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;
using System.Threading.Tasks;
using VeraAPI.Models;
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
            log.WriteLogEntry("Begin TravelApprovalController GET...");
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
                        if (formHelp.LoadTravelForms(Constants.GetApproverTravelForms, userID) > 0)
                            result = formHelp.WebForms.ToArray();
                        else
                            log.WriteLogEntry("No forms found pending approval.");
                    }
                    catch (Exception ex)
                    {
                        log.WriteLogEntry("General program error! " + ex.Message);
                        result = new BaseForm[0];
                        return result;
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
            log.WriteLogEntry("Forms returned " + result.Length);
            log.WriteLogEntry("End TravelApprovalController GET.");
            return result;
        }

        // POST: api/TravelApproval
        public string Post([FromUri]string restUserID, [FromBody]TravelAuthForm travelAuthForm)
        {
            log.WriteLogEntry("Begin TravelApprovalController POST...");
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
            log.WriteLogEntry("End TravelApprovalController POST.");
            return result;
        }

        // PUT: api/TravelApproval/5
        public void Put([FromUri]string restUserID, [FromBody]TravelAuthForm travelAuthForm)
        {
            log.WriteLogEntry("Begin TravelApprovalController PUT...");
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
                                log.WriteLogEntry("Start form dump...");
                                log.DumpObject(travelAuthForm);
                                log.WriteLogEntry("Starting FormHelper...");
                                FormHelper travelFormHelp = new FormHelper();
                                if (travelFormHelp.ApproveTravelAuthForm(userID, travelAuthForm))
                                {
                                    EmailHelper email = new EmailHelper();
                                    if (travelAuthForm.DHApproval.ToLower() == Constants.ApprovedColor && travelAuthForm.GMApproval.ToLower() != Constants.ApprovedColor)
                                        email.NotifyGeneralManager(user);
                                    else if (travelAuthForm.DHApproval.ToLower() == Constants.DeniedColor && travelAuthForm.GMApproval.ToLower() != Constants.ApprovedColor)
                                        email.NotifySubmitter(travelAuthForm.Email, Constants.NotificationTravelDenied);
                                    else if (travelAuthForm.GMApproval.ToLower() == Constants.ApprovedColor)
                                    {
                                        email.NotifySubmitter(travelAuthForm.Email, Constants.NotificationTravelApproved);
                                        if (bool.TryParse(travelAuthForm.Advance, out bool advance))
                                        {
                                            if (advance)
                                                email.NotifyFinance(0);
                                            else
                                                log.WriteLogEntry("No advance requested.");
                                        }
                                        else
                                            log.WriteLogEntry("FAILED to parse travel form advance boolean!");
                                    }
                                    else
                                    {
                                        log.WriteLogEntry("GM has not approved.");
                                    }
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
            log.WriteLogEntry("End TravelApprovalController PUT.");
        }

        // DELETE: api/TravelApproval/5
        public void Delete(string id)
        {
        }
    }
}
