using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;

namespace VeraAPI.Models
{
    public class JobTemplate
    {

        public int TemplateID { get; set; }
        public string TemplateName { get; set; }
        public string TableName { get; set; }
        public string JobDescription { get; set; }
        public int JobWeight { get; set; }
        public int JobPriority { get; set; }
        public string JobType { get; set; }

        public JobTemplate() { }

        public JobTemplate(int templateID)
        {
            this.TemplateID = templateID;
        }
    }
}