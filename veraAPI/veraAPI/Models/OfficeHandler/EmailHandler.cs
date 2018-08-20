using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Exchange.WebServices;
using Microsoft.Exchange.WebServices.Data;
using System.Configuration;
using VeraAPI.Models;
using VeraAPI.Models.Security;

namespace VeraAPI.Models
{
    public class EmailHandler
    {
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string RecipientEmailAddress { get; set; }
        private User EmailSender;
        private ExchangeService Emailer;
        private EmailMessage EmailMessage;
        private Scribe Log;

        public EmailHandler(User CurrentUser, Scribe Log)
        {
            this.EmailSender = CurrentUser;
            this.Log = Log;
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
            bool result = false;
            try
            {
                Emailer = new ExchangeService()
                {
                    Credentials = new WebCredentials(EmailSender.AdUpn, EmailSender.UserPwd)
                };
                Emailer.AutodiscoverUrl(EmailSender.AdUpn, ValidateRedirect);
                result = true;
            }
            catch (Exception ex)
            {
                Log.WriteLogEntry("Failed Connecting to Exchange Service.");
                Log.WriteLogEntry(ex.Message);
                result = false;
            }
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