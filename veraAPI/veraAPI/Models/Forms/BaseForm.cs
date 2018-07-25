using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeraUITest
{
    public class BaseForm
    {
        public int FormDataID { get; set; }
        public int TemplateID { get; set; }

        public BaseForm() { }

        public BaseForm(int formDataID)
        {
            this.FormDataID = formDataID;
        }
    }
}
