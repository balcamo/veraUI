using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using Microsoft.Exchange.WebServices;
using Microsoft.Exchange.WebServices.Data;
using System.Configuration;
using VeraAPI.Models.Tools;
using VeraAPI.Models.Security;

namespace VeraAPI.Models.OfficeHandler
{
    public class ExchangeHandler
    {
        public string EmailSubject { get; set; } = "Subject line test";
        public string EmailBody { get; set; } = @"Testing send email. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean nibh eros, commodo efficitur tellus vitae, 
            malesuada finibus elit. Nulla vel odio metus. Nulla mi mauris, efficitur at cursus semper, pellentesque eu mi. Praesent maximus imperdiet dui, nec hendrerit turpis 
            vehicula a. Quisque ultricies faucibus odio sit amet suscipit. Sed erat libero, tempor in sodales vel, scelerisque maximus turpis. Vivamus placerat odio eu dignissim 
            sagittis. Nunc ac ornare tellus, a faucibus dolor. Curabitur a magna eget erat vestibulum imperdiet.";
        public string RecipientEmailAddress { get; set; }

        private User CurrentUser;
        private ExchangeService emailer;
        private EmailMessage emailMessage;
        private Scribe log;
        private string mailService;
        private string mailServicePwd;

        public ExchangeHandler()
        {
            log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "ExchangeHandler_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            mailService = WebConfigurationManager.AppSettings.Get("MailService");
            mailServicePwd = WebConfigurationManager.AppSettings.Get("MailServicePwd");
            emailer = new ExchangeService();
            CurrentUser = new User();
        }

        public ExchangeHandler(User user)
        {
            log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "ExchangeHandler_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            mailService = WebConfigurationManager.AppSettings.Get("MailService");
            mailServicePwd = WebConfigurationManager.AppSettings.Get("MailServicePwd");
            emailer = new ExchangeService();
            CurrentUser = user;
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
            log.WriteLogEntry("Starting ConnectExchangeService.");
            bool result = false;
            try
            {
                emailer.Credentials = new WebCredentials(mailService, mailServicePwd);
                emailer.AutodiscoverUrl(mailService, ValidateRedirect);
                result = true;
            }
            catch (Exception ex)
            {
                log.WriteLogEntry("Failed Connecting to Exchange Service.");
                log.WriteLogEntry(ex.Message);
                result = false;
            }
            log.WriteLogEntry("End ConnectExchangeService.");
            return result;
        }

        public bool SendMail()
        {
            log.WriteLogEntry("Begin SendMail...");
            bool result = false;
            log.WriteLogEntry("Connection to Exchange service successful.");
            try
            {
                log.WriteLogEntry("Sender " + emailer.Credentials);
                log.WriteLogEntry("Recipient " + RecipientEmailAddress);
                log.WriteLogEntry("Subject " + EmailSubject);
                log.WriteLogEntry("Body " + EmailBody);
                emailMessage = new EmailMessage(emailer);
                emailMessage.ToRecipients.Add(RecipientEmailAddress);
                emailMessage.Subject = EmailSubject;
                emailMessage.Body = new MessageBody(EmailBody);
                emailMessage.Send();
                result = true;
            }
            catch (Exception ex)
            {
                log.WriteLogEntry("Send email failed!");
                log.WriteLogEntry(ex.Message);
                result = false;
            }
            log.WriteLogEntry("End SendMail.");
            return result;
        }
    }
}