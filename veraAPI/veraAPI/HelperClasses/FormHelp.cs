using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using VeraAPI.Models;
using VeraAPI.Models.Forms;
using VeraAPI.Models.DataHandler;

namespace VeraAPI.HelperClasses
{
    public class FormHelp
    {
        private string dbServer;
        private string dbName;
        private UIDataHandler UIData;
        private Validator FormValidator;
        private Scribe Log;
        public string userEmail { get; set; }

        public FormHelp()
        {
            dbServer = WebConfigurationManager.AppSettings.Get("DBServer");
            dbName = WebConfigurationManager.AppSettings.Get("DBName");
            Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UIFormHelper_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
        }
        /**
        * 
        * SubmitForm will insert a new record into the database 
        *      it will also send an email to the appropriate parties
        * @param FormData : this is the form that needs to be inserted
        * 
        **/
        public bool SubmitForm(BaseForm FormData)
        {
            bool result = false;
            UIData = new UIDataHandler(dbServer, dbName);
            BaseForm SubmittedForm = FormData;
            UIData.FormData = FormData;
            if (UIData.LoadJobTemplate(UIData.FormData.TemplateID))
            {
                if (UIData.InsertFormData())
                {
                    if (UIData.InsertJob())
                    {
                        // Call UIDataHandler method to load the form data from SQL using the submitted form ID
                        UIData.LoadTravelAuth(SubmittedForm.FormDataID);
                        FormValidator = new Validator(Log);
                        // Compare above stored SubmittedForm to loaded UIDataHandler form
                        if (FormValidator.CompareAlphaBravo(SubmittedForm, UIData.FormData))
                        {
                            Log.WriteLogEntry("Submitted form matches inserted form!");
                            userEmail = UIData.userEmail;
                            result = true;
                        }
                        else
                            Log.WriteLogEntry("Mismatch!!! Submitted form does not match inserted form!");
                    }
                }
            }
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
