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

namespace VeraAPI.HelperClasses
{
    public class FormHelper
    {
        public BaseForm WebForm { get; set; }
        public List<BaseForm> WebForms { get; private set; }
        public JobTemplate Template { get; private set; }

        private string dbServer;
        private string dbName;
        private FormDataHandler formDataHandle;
        //private Validator formValidator;
        private Scribe log;

        public FormHelper()
        {
            log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "FormHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            dbServer = WebConfigurationManager.AppSettings.Get("DBServer");
            dbName = WebConfigurationManager.AppSettings.Get("DBName");
            this.WebForm = new BaseForm();
        }

        public FormHelper(BaseForm webForm)
        {
            log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "FormHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            dbServer = WebConfigurationManager.AppSettings.Get("DBServer");
            dbName = WebConfigurationManager.AppSettings.Get("DBName");
            this.WebForm = webForm;
        }

        /**
        * 
        * SubmitForm will insert a new record into the database 
        *      it will also send an email to the appropriate parties
        * @param FormData : this is the form that needs to be inserted
        * 
        **/
        public bool SubmitTravelAuthForm()
        {
            log.WriteLogEntry("Begin FormHelp SubmitForm...");
            bool result = false;
            formDataHandle = new FormDataHandler(WebForm, dbServer, dbName);

            // Hold submitted form for comparison
            BaseForm SubmittedForm = WebForm;

            // Load the job template corresponding to the templateID for the submitted form
            if (formDataHandle.LoadFormTemplate())
            {
                Template = formDataHandle.Template;

                // Insert travel form data into the database
                if (formDataHandle.InsertFormData())
                {
                    log.WriteLogEntry("Success insert form data to database.");
                    result = true;
                }
                log.WriteLogEntry("Failed insert form data to database.");
            }
            else
                log.WriteLogEntry("Failed loading form template!");
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
            formDataHandle.LoadTravelAuth();
            log.WriteLogEntry("End LoadTravelAuthForm.");
            return result;
        }

        public int LoadActiveTravelAuthForms(string userID)
        {
            log.WriteLogEntry("Starting LoadActiveTravelAuthForms...");
            log.WriteLogEntry("User ID (passed token header) " + userID);
            int result = 0;
            WebForms = new List<BaseForm>();
            formDataHandle = new FormDataHandler(WebForms, dbServer, dbName);
            formDataHandle.LoadTravelAuthForms(userID);
            log.WriteLogEntry("Count loaded forms " + WebForms.Count);
            log.WriteLogEntry("End LoadActiveTravelAuthForms.");
            return result;
        }
    }
}
