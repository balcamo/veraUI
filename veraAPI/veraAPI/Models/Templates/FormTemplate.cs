﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VeraAPI.Models.Templates
{
    public class FormTemplate
    {
        public int TemplateID { get; set; }
        public string TemplateName { get; set; }
        public string TableName { get; set; }
        public string JobDescription { get; set; }
        public int JobWeight { get; set; }
        public int JobPriority { get; set; }
        public string JobType { get; set; }

        public FormTemplate() { }

        public FormTemplate(int templateID)
        {
            this.TemplateID = templateID;
        }
    }
}