using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeraAPI.Models.Forms
{
    public class BaseForm
    {
        public int FormDataID { get; set; }
        public int TemplateID { get; set; }
        public string UserID { get; set; }

        public BaseForm() { }

        public BaseForm(int formDataID)
        {
            this.FormDataID = formDataID;
        }
    }
}
