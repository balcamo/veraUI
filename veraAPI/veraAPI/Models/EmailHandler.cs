using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Exchange.WebServices;
using Microsoft.Exchange.WebServices.Data;
using System.Configuration;
using VeraAPI.Models;

namespace veraAPI.Models
{
    public class EmailHandler
    {
        public string SenderEmailAddress { get; set; } = "test@verawaterandpower.com";
        public string EmailSubject { get; set; } = "UI Test Email";
        public string EmailBody { get; set; } = "Testing UI email sender.";
        public string RecipientEmailAddress { get; set; } = "thuff@verawaterandpower.com";
        private ExchangeService Emailer;
        private EmailMessage EmailMessage;
        private Scribe Log;

        public EmailHandler(Scribe Log)
        {
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
                    Credentials = new WebCredentials(SenderEmailAddress, "verawateR4power")
                };
                Emailer.AutodiscoverUrl(SenderEmailAddress, ValidateRedirect);
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