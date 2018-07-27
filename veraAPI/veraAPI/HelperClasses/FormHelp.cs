using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using veraAPI.Models;
using veraAPI.Models.Forms;

namespace veraAPI.HelperClasses
{
    public class FormHelp
    {
        public UIDataHandler UIData = new UIDataHandler("Valhalla", "Valhalla");
        Scribe Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UIFormHelper_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
        
        public void SubmitForm(BaseForm FormData)
        {
            TravelAuthForm Submitted;
            Validator FormValidator;
            UIData.FormData = FormData;
            if (UIData.LoadJobTemplate(UIData.FormData.TemplateID))
            {
                if (UIData.InsertFormData())
                {
                    if (UIData.InsertJob())
                    {
                        Submitted = (TravelAuthForm)UIData.FormData;
                        UIData.LoadTravelAuth(Submitted.FormDataID);
                        Log.WriteLogEntry("Submitted last name: " + Submitted.LastName + " Inserted last name: " + UIData.FormData);
                        FormValidator = new Validator();
                        if (FormValidator.CompareAlphaBravo(Submitted, UIData.FormData))
                            Log.WriteLogEntry("Submitted form matches inserted form!");
                        else
                            Log.WriteLogEntry("Mismatch!!! Submitted form does not match inserted form!");
                    }
                }
            }
            return;
        }
    }
}
