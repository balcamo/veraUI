using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VeraAPI.Models;
using VeraAPI.Models.Forms;
using VeraAPI.Models.DataHandler;

namespace VeraAPI.HelperClasses
{
    public class FormHelp
    {
        public UIDataHandler UIData = new UIDataHandler("Valhalla", "Valhalla");
        Scribe Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UIFormHelper_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
        
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

        public void UpdateForm(BaseForm FormData)
        {
            //set up for updating a specific record
        }
    }
}
