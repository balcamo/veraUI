using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Exchange.WebServices;
using Microsoft.Exchange.WebServices.Data;
using System.Configuration;
using VeraAPI.Models;
using VeraAPI.Models.Security;

namespace VeraAPI.Models.OfficeHandler
{
    public class EmailHandler
    {
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string RecipientEmailAddress { get; set; }
        public LoginUser UserEmail { get; set; }
        private ExchangeService Emailer;
        private WebCredentials UserCredentials;
        private EmailMessage EmailMessage;
        private Scribe Log;

        public EmailHandler()
        {
            Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UIEmailHelper_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
            Emailer = new ExchangeService();
        }

        public EmailHandler(LoginUser CurrentUser)
        {
            Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UIEmailHelper_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
            this.UserEmail = CurrentUser;
            Emailer = new ExchangeService();
            UserCredentials = new WebCredentials(this.UserEmail.AdUpn, this.UserEmail.UserPwd);
        }

        private bool ValidateRedirect(string redirectionUrl)
        {
            bool result = false;
            Uri redirectionUri = new Uri(redirectionUrl);
            if (redirectionUri.Scheme == "https")
                result = true;
            return result;
        }

        public bool ConnectExchangeService()
        {
            Log.WriteLogEntry("Starting ConnectExchangeService.");
            bool result = false;
            try
            {
                Emailer.Credentials = UserCredentials;
                Emailer.AutodiscoverUrl(UserEmail.AdUpn, ValidateRedirect);
                result = true;
            }
            catch (Exception ex)
            {
                Log.WriteLogEntry("Failed Connecting to Exchange Service.");
                Log.WriteLogEntry(ex.Message);
                result = false;
            }
            Log.WriteLogEntry("End ConnectExchangeService.");
            return result;
        }

        public bool SendEmail()
        {
            bool result = false;
            try
            {
                EmailMessage = new EmailMessage(Emailer);
                EmailMessage.ToRecipients.Add(RecipientEmailAddress);
                EmailMessage.Subject = EmailSubject;
                EmailMessage.Body = new MessageBody(EmailBody);
                EmailMessage.Send();
                result = true;
            }
            catch (Exception ex)
            {
                Log.WriteLogEntry("Send email failed!");
                Log.WriteLogEntry(ex.Message);
                result = false;
            }
            return result;
        }
    }
}