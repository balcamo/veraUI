using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;

namespace veraAPI.Models
{
    public class JobHeader
    {
        public int JobID { get; set; }
        public int TemplateID { get; set; }
        public int DataID { get; set; }
        public string JobDescription { get; set; }
        public DateTime EntryTimestamp { get; set; }
        public DateTime CompleteDate { get; set; }
        public int Duration { get; set; }
        public string TableName { get; set; }
        public int JobPriority { get; set; }
        public int JobWeight { get; set; }
        public string JobType { get; set; }

        public JobHeader() { }

        public JobHeader(int jobID)
        {
            this.JobID = jobID;
        }
    }
}