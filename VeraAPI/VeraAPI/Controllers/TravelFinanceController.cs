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
    public class TravelFinanceController : ApiController
    {
        private static Scribe log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "TravelFinanceController_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");

        // GET: api/TravelFinance
        public BaseForm[] Get(string restUserID)
        {
            // call function to get active forms
            log.WriteLogEntry("Begin TravelFinanceController GET...");
            BaseForm[] result = new BaseForm[0];
            if (int.TryParse(restUserID, out int userID))
            {
                log.WriteLogEntry("Starting LoginHelper...");
                LoginHelper loginHelp = new LoginHelper();
                if (loginHelp.LoadUserSession(userID))
                {
                    log.WriteLogEntry("Starting FormHelper...");
                    FormHelper formHelp = new FormHelper();
                    if (formHelp.LoadTravelForms(Constants.GetFinanceTravelForms, userID) > 0)
                    {
                        result = formHelp.WebForms.ToArray();
                    }
                    else
                    {
                        log.WriteLogEntry("No finance travel forms loaded.");
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
            log.WriteLogEntry("Count of forms returned " + result.Count<BaseForm>());
            log.WriteLogEntry("End TravelFinanceController GET.");
            return result;
        }

        // POST: api/TravelFinance
        public string Post([FromUri]string restUserID, [FromBody]TravelAuthForm travelAuthForm)
        {
            log.WriteLogEntry("Begin TravelFinanceController POST...");
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
                        result = "TravelFinanceController POST.";
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
            log.WriteLogEntry("End TravelFinanceController POST.");
            return result;
        }

        // POST: api/TravelFinance
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/TravelFinance/5
        // for restButtonID 0 = Approve Advance 1 = Approve Recap (closes auth form) 2 = Deny Advance 3 = Deny Recap
        public void Put([FromUri]string restUserID, [FromUri]string restButtonID, [FromBody]TravelAuthForm travelAuthForm)
        {
            log.WriteLogEntry("Begin TravelFinanceController PUT...");
            if (int.TryParse(restUserID, out int userID))
            {
                if (int.TryParse(restButtonID, out int buttonID))
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
                            try
                            {
                                if (travelAuthForm.GetType() == typeof(TravelAuthForm))
                                {
                                    log.DumpObject(travelAuthForm);
                                    log.WriteLogEntry("Starting FormHelper...");
                                    FormHelper travelFormHelp = new FormHelper();
                                    if (travelFormHelp.HandleFinanceAction(buttonID, travelAuthForm))
                                    {
                                        log.WriteLogEntry("Starting EmailHelper...");
                                        EmailHelper email = new EmailHelper();
                                        switch (buttonID)
                                        {
                                            case 0:
                                                {
                                                    email.NotifySubmitter(travelAuthForm.Email, Constants.NotificationTravelAdvance);
                                                    break;
                                                }
                                            case 1:
                                                {
                                                    email.NotifySubmitter(travelAuthForm.Email, Constants.NotificationTravelRecap);
                                                    break;
                                                }
                                            case 2:
                                                {
                                                    email.NotifySubmitter(travelAuthForm.Email, travelAuthForm.MailMessage);
                                                    break;
                                                }
                                            case 3:
                                                {
                                                    email.NotifySubmitter(travelAuthForm.Email, travelAuthForm.MailMessage);
                                                    break;
                                                }
                                            default:
                                                {
                                                    log.WriteLogEntry("Fell through button ID switch. No email sent.");
                                                    break;
                                                }
                                        }
                                    }
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
                    log.WriteLogEntry("FAILED invalid button id!");
            }
            else
                log.WriteLogEntry("FAILED invalid user id!");
            log.WriteLogEntry("End TravelFinanceController PUT.");
        }

        // DELETE: api/TravelFinance/5
        public void Delete(string id)
        {
        }
    }
}
