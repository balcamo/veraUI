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
using VeraAPI.Models.JobService;

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
        private Validator formValidator;
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
        public bool SubmitForm()
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
                    // Call UIDataHandler method to load the form data from SQL using the submitted form ID
                    formDataHandle.LoadTravelAuth();
                    formValidator = new Validator(SubmittedForm, WebForm);
                    // Compare above stored SubmittedForm to loaded UIDataHandler form
                    if (formValidator.CompareAlphaBravo())
                    {
                        log.WriteLogEntry("Submitted form matches inserted form!");
                        result = true;
                    }
                    else
                        log.WriteLogEntry("Mismatch!!! Submitted form does not match inserted form!");
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
        public bool LoadForm()
        {
            log.WriteLogEntry("Starting LoadForm.");
            bool result = false;
            formDataHandle = new FormDataHandler(WebForm, dbServer, dbName);
            formDataHandle.LoadTravelAuth();
            log.WriteLogEntry("End LoadForm.");
            return result;
        }

        public int LoadActiveForms(int userID)
        {
            log.WriteLogEntry("Starting LoadActiveForms.");
            int result = 0;
            WebForms = new List<BaseForm>();
            formDataHandle = new FormDataHandler(WebForms, dbServer, dbName);
            formDataHandle.LoadTravelAuthForms(userID);
            log.WriteLogEntry("Count loaded forms " + WebForms.Count);
            log.WriteLogEntry("End LoadActiveForms.");
            return result;
        }
    }
}
