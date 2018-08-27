﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using VeraAPI.Models;
using VeraAPI.Models.Forms;
using VeraAPI.Models.DataHandler;

namespace VeraAPI.HelperClasses
{
    public class FormHelper
    {
        public BaseForm WebForm { get; set; }

        private string dbServer;
        private string dbName;
        private FormDataHandler FormDataHandle;
        private Validator FormValidator;
        private Scribe Log;

        public FormHelper()
        {
            dbServer = WebConfigurationManager.AppSettings.Get("DBServer");
            dbName = WebConfigurationManager.AppSettings.Get("DBName");
            Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UIFormHelper_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
            WebForm = new BaseForm();
            FormDataHandle = new FormDataHandler(dbServer, dbName);
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
            Log.WriteLogEntry("Begin FormHelp SubmitForm...");
            bool result = false;
            // Hold submitted form for comparison
            BaseForm SubmittedForm = WebForm;
            // Set data handler form to submitted form
            FormDataHandle.FormData = WebForm;
            // Load the job template corresponding to the templateID for the submitted form
            if (FormDataHandle.LoadJobTemplate(FormDataHandle.FormData.TemplateID))
            {
                Log.WriteLogEntry("Success load submit form job template.");
                // Insert travel form data into the database
                if (FormDataHandle.InsertFormData())
                {
                    Log.WriteLogEntry("Success insert form data to database.");
                    // Call UIDataHandler method to load the form data from SQL using the submitted form ID
                    FormDataHandle.LoadTravelAuth(SubmittedForm.FormDataID);
                    FormValidator = new Validator(Log);
                    // Compare above stored SubmittedForm to loaded UIDataHandler form
                    if (FormValidator.CompareAlphaBravo(SubmittedForm, FormDataHandle.FormData))
                    {
                        Log.WriteLogEntry("Submitted form matches inserted form!");
                        result = true;
                    }
                    else
                        Log.WriteLogEntry("Mismatch!!! Submitted form does not match inserted form!");
                }
                Log.WriteLogEntry("Failed insert form data to database.");
            }
            Log.WriteLogEntry("End FormHelp SubmitForm result " + result);
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
    }
}