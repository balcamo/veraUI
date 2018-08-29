using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using VeraAPI.Models.Templates;

namespace VeraAPI.Models.JobService
{
    public class JobHeader : JobTemplate
    {
        public int JobID { get; set; }
        public int FormDataID { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime CompleteDate { get; set; }
        public int Duration { get; set; }

        public JobHeader() { }

        public JobHeader(int jobID)
        {
            this.JobID = jobID;
        }
    }
}