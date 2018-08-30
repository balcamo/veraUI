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
        public JobHeader Job { get; private set; }

        private JobDataHandler JobDataHandle;
        private string dbServer;
        private string dbName;
        private Scribe log;

        public JobHelper(JobHeader job)
        {
            log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "JobHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            dbServer = WebConfigurationManager.AppSettings.Get("DBServer");
            dbName = WebConfigurationManager.AppSettings.Get("DBName");
            this.Job = job;
        }

        public bool InsertFormJob()
        {
            log.WriteLogEntry("Begin InsertFormjob...");
            bool result = false;
            JobDataHandle = new JobDataHandler(Job, dbServer, dbName);
            if (JobDataHandle.InsertJob())
            {
                result = true;
            }
            else
                log.WriteLogEntry("Failed insert job!");
            log.WriteLogEntry("End InsertFormJob.");
            return result;
        }
    }
}