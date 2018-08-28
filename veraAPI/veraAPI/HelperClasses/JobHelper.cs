using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using VeraAPI.Models.JobService;
using VeraAPI.Models.Templates;
using VeraAPI.Models.DataHandler;
using VeraAPI.Models.Tools;

namespace VeraAPI.HelperClasses
{
    public class JobHelper
    {
        public JobHeader Job { get; set; }
        public JobTemplate Template { private get; set; }

        private JobDataHandler JobDataHandle;
        private string dbServer;
        private string dbName;
        private Scribe Log;

        public JobHelper()
        {
            Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "JobHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            dbServer = WebConfigurationManager.AppSettings.Get("DBServer");
            dbName = WebConfigurationManager.AppSettings.Get("DBName");
            Job = new JobHeader();
        }

        public bool InsertFormJob()
        {
            Log.WriteLogEntry("Begin InsertFormjob...");
            bool result = false;
            if (Template != null && Job.FormDataID > 0)
            {
                JobDataHandle = new JobDataHandler(dbServer, dbName);
                JobDataHandle.Template = Template;
                if (JobDataHandle.InsertJob())
                {

                }
            }
            else if (Template == null)
                Log.WriteLogEntry("Failed template not loaded!");
            else if (Job.FormDataID < 1)
                Log.WriteLogEntry("Failed form data not loaded!");
            Log.WriteLogEntry("End InsertFormJob.");
            return result;
        }
    }
}