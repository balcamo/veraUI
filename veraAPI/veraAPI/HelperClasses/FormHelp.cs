using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VeraAPI.Models;
using VeraAPI.Models.Forms;

namespace VeraAPI.HelperClasses
{
    public class FormHelp
    {
        public UIDataHandler UIData = new UIDataHandler("Valhalla", "Valhalla");
        Scribe Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UIFormHelper_" + DateTime.Now.ToString("yyyyMMdd") + ".log");

        /**
        * 
        * SubmitForm will insert a new record into the database 
        *      it will also send an email to the appropriate parties
        * @param FormData : this is the form that needs to be inserted
        * 
        **/
        public void SubmitForm(BaseForm FormData)
        {
            BaseForm SubmittedForm = FormData;
            Validator FormValidator;
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
                            Log.WriteLogEntry("Submitted form matches inserted form!");
                        else
                            Log.WriteLogEntry("Mismatch!!! Submitted form does not match inserted form!");
                    }
                }
            }
            return;
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
