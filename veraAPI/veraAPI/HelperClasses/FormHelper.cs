using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using VeraAPI.Models;
using VeraAPI.Models.Forms;
using VeraAPI.Models.Templates;
using VeraAPI.Models.DataHandler;
using VeraAPI.Models.Tools;
using VeraAPI.Models.OfficeHandler;
using VeraAPI.Models.Security;

namespace VeraAPI.HelperClasses
{
    public class FormHelper
    {
        public BaseForm WebForm { get; private set; }
        public List<BaseForm> WebForms { get; private set; }
        //public JobTemplate Template { get; private set; }

        private readonly string dbServer = WebConfigurationManager.AppSettings.Get("DBServer");
        private readonly string dbName = WebConfigurationManager.AppSettings.Get("DBName");
        private static Scribe log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "FormHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
        private FormDataHandler formDataHandle;
        //private Validator formValidator;

        public FormHelper()
        {
            this.WebForm = new BaseForm();
        }

        /**
        * 
        * SubmitForm will insert a new record into the database 
        *      it will also send an email to the appropriate parties
        * @param FormData : this is the form that needs to be inserted
        * 
        **/
        public bool SubmitTravelAuthForm(User user, BaseForm webForm)
        {
            log.WriteLogEntry("Begin SubmitTravelAuthForm...");
            bool result = false;
            if (webForm.GetType() == typeof(TravelAuthForm))
            {
                TravelAuthForm travelForm = (TravelAuthForm)webForm;
                if (user.GetType() == typeof(DomainUser))
                {
                    DomainUser domainUser = (DomainUser)user;
                    travelForm.UserID = domainUser.UserID;
                    travelForm.DeptHeadID = domainUser.Department.DeptHeadUserID.ToString();
                    travelForm.DeptHeadEmail = domainUser.Department.DeptHeadEmail;
                    travelForm.GeneralManagerID = domainUser.Company.GeneralManagerUserID.ToString();
                    travelForm.GeneralManagerEmail = domainUser.Company.GeneralManagerEmail;

                    // Load the job template corresponding to the templateID for the submitted form
                    log.WriteLogEntry("Starting FormDataHandler...");
                    formDataHandle = new FormDataHandler(dbServer, dbName);
                    if (formDataHandle.LoadFormTemplate(webForm.TemplateID))
                    {
                        // Insert travel form data into the database
                        if (formDataHandle.InsertTravelAuth(travelForm))
                        {
                            result = true;
                        }
                        log.WriteLogEntry("Failed insert form data to database.");
                    }
                    else
                        log.WriteLogEntry("Failed loading form template!");
                }
            }
            else
                log.WriteLogEntry("FAILED submitted form is not a travel authorization form!");
            log.WriteLogEntry("End FormHelp SubmitForm result " + result);
            return result;
        }

        /**
         * 
         * UpdateForm will find a record in the database to update
         *      it will also send an email to the appropriate parties
         * @param FormData : this is the form that needs to be updated
         * @param EmailType : the eamil type will be used to send an email to 
         *      those that need to be notified
         *      
         **/
        public void UpdateForm(BaseForm FormData, string EmailType)
        {
            //set up for updating a specific record
            //
            
        }

        // WHAT FORMS ARE BEING LOADED?
        // Load a travel auth form based on the form data ID
        public bool LoadTravelAuthForm()
        {
            log.WriteLogEntry("Starting LoadTravelAuthForm...");
            bool result = false;
            formDataHandle = new FormDataHandler(WebForm, dbServer, dbName);
            formDataHandle.LoadTravelAuthForm();
            log.WriteLogEntry("End LoadTravelAuthForm.");
            return result;
        }

        public int LoadActiveTravelAuthForms(int userID)
        {
            log.WriteLogEntry("Starting LoadActiveTravelAuthForms...");
            log.WriteLogEntry("User ID " + userID);
            int result = 0;
            WebForms = new List<BaseForm>();
            formDataHandle = new FormDataHandler(WebForms, dbServer, dbName);
            formDataHandle.LoadUserTravelAuthForms(userID);
            log.WriteLogEntry("Count loaded forms " + WebForms.Count);
            log.WriteLogEntry("End LoadActiveTravelAuthForms.");
            return result;
        }
    }
}
