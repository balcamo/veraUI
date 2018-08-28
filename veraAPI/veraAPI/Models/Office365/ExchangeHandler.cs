using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Exchange.WebServices;
using Microsoft.Exchange.WebServices.Data;
using System.Configuration;
using VeraAPI.Models.Tools;
using VeraAPI.Models.Security;

namespace VeraAPI.Models.OfficeHandler
{
    public class ExchangeHandler
    {
        public string EmailSubject { get; set; } = "Department Head Test Email";
        public string EmailBody { get; set; } = "Testing send email to department head.";
        public string RecipientEmailAddress { get; set; }
        public User CurrentUser { get; set; }
        private ExchangeService Emailer;
        private WebCredentials UserCredentials;
        private EmailMessage EmailMessage;
        private Scribe Log;

        public ExchangeHandler()
        {
            Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UIEmailHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            Emailer = new ExchangeService();
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
            if (CurrentUser.GetType() == typeof(DomainUser))
            {
                DomainUser user = (DomainUser)CurrentUser;
                try
                {
                    Emailer.Credentials = UserCredentials;
                    Emailer.AutodiscoverUrl(user.DomainUpn, ValidateRedirect);
                    result = true;
                }
                catch (Exception ex)
                {
                    Log.WriteLogEntry("Failed Connecting to Exchange Service.");
                    Log.WriteLogEntry(ex.Message);
                    result = false;
                }
            }
            Log.WriteLogEntry("End ConnectExchangeService.");
            return result;
        }

        public bool SendMail()
        {
            Log.WriteLogEntry("Begin SendMail...");
            bool result = false;
            Log.WriteLogEntry("Connection to Exchange service successful.");
            if (CurrentUser.GetType() == typeof(DomainUser))
            {
                DomainUser user = (DomainUser)CurrentUser;
                try
                {
                    UserCredentials = new WebCredentials(user.DomainUpn, user.UserPwd);
                    RecipientEmailAddress = user.DepartmentHeadEmail;
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
            }
            Log.WriteLogEntry("End SendMail.");
            return result;
        }
    }
}