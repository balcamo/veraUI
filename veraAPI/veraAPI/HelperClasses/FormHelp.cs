﻿using System;
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
                        FormValidator = new Validator(Log);
                        // since SubmittedForm and UIData.FormData 
                        // are both made from FormData what's the point in comparing?
                        if (FormValidator.CompareAlphaBravo(SubmittedForm, UIData.FormData))
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
