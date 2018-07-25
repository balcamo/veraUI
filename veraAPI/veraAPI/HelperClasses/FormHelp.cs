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
        
        public void SubmitForm(BaseForm FormData)
        {
            UIData.FormData = FormData;
            if (UIData.LoadJobTemplate(UIData.FormData.TemplateID))
            {
                if (UIData.InsertFormData())
                    UIData.InsertJob();
            }
            
            return;
        }
    }
}
