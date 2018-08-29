using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using VeraAPI.Models.Tools;
using VeraAPI.Models.Templates;
using VeraAPI.Models.JobService;

namespace VeraAPI.Models.DataHandler
{
    public class JobDataHandler : SQLDataHandler
    {
        public JobHeader Job { get; private set; }

        private Scribe log = null;

        public JobDataHandler(JobHeader job, string dbServer, string dbName) : base(dbServer)
        {
            this.log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "JobDataHandler_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            this.dbServer = dbServer;
            this.dbName = dbName;
            this.dataConnectionString = GetDataConnectionString();
            this.Job = job;
        }

        public bool InsertJob()
        {
            log.WriteLogEntry("Begin InsertJob...");
            bool result = false;
            string cmdString = string.Format(@"insert into {0}.dbo.job_header (template_id, data_id, job_description, job_priority, job_weight, job_type, entry_dt) output inserted.job_id values (@templateID, @dataID, @jobDescription, @jobPriority, @jobWeight, @jobType, GETDATE())", dbName);
            // Check JobTemplate and FormData objects for required ID data prior to calling SQL query
            //      if TemplateID or FormDataID are missing skip the SQL call and log the reason
            if (Job.TemplateID > 0 && Job.FormDataID > 0)
            {
                using (SqlConnection conn = new SqlConnection(dataConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                    {
                        cmd.Parameters.AddWithValue("@dataID", Job.FormDataID);
                        cmd.Parameters.AddWithValue("@templateID", Job.TemplateID);
                        cmd.Parameters.AddWithValue("@jobDescription", Job.JobDescription);
                        cmd.Parameters.AddWithValue("@jobPriority", Job.JobPriority);
                        cmd.Parameters.AddWithValue("@jobWeight", Job.JobWeight);
                        cmd.Parameters.AddWithValue("@jobType", Job.JobType);
                        try
                        {
                            conn.Open();
                            int jobID = (int)cmd.ExecuteScalar();
                            if (jobID > 0)
                            {
                                log.WriteLogEntry("Successful insert job " + jobID);
                                Job.JobID = jobID;
                                result = true;
                            }
                            else
                                log.WriteLogEntry("Failed insert job to database!");
                        }
                        catch (SqlException ex)
                        {
                            result = false;
                            log.WriteLogEntry("SQL error " + ex.Message);
                        }
                        catch (Exception ex)
                        {
                            result = false;
                            log.WriteLogEntry("General program error " + ex.Message);
                        }
                    }
                }
            }
            else if (Job.TemplateID < 1)
            {
                result = false;
                log.WriteLogEntry("No job template loaded.");
            }
            else if (Job.FormDataID < 1)
            {
                result = false;
                log.WriteLogEntry("No form data loaded.");
            }
            log.WriteLogEntry("End InsertJob.");
            return result;
        }

        public bool LoadJobHeader(int jobID)
        {
            log.WriteLogEntry("Begin LoadJobHeader...");
            bool result = false;
            string cmdString = string.Format(@"select * from {0}.dbo.job_header where job_id = @jobID", dbName);
            log.WriteLogEntry("SQL command string: " + cmdString);
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    cmd.Parameters.AddWithValue("@jobID", jobID);
                    try
                    {
                        conn.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                Job = new JobHeader(jobID);
                                Job.TemplateID = (int)rdr["template_id"];
                                Job.FormDataID = (int)rdr["data_id"];
                                Job.JobDescription = rdr["job_description"].ToString();
                                var entryDate = rdr["entry_dt"];
                                if (entryDate != DBNull.Value)
                                    Job.EntryDate = DateTime.Parse(entryDate.ToString());
                                var completeDate = rdr["complete_dt"];
                                if (completeDate != DBNull.Value)
                                    Job.CompleteDate = DateTime.Parse(completeDate.ToString());
                                var duration = rdr["duration"];
                                if (duration != DBNull.Value)
                                    Job.Duration = (int)duration;
                                var jobPriority = rdr["job_priority"];
                                if (jobPriority != DBNull.Value)
                                    Job.JobPriority = (int)jobPriority;
                                var jobWeight = rdr["job_weight"];
                                if (jobWeight != DBNull.Value)
                                    Job.JobWeight = (int)jobWeight;
                                Job.JobType = rdr["job_type"].ToString();
                                log.WriteLogEntry("Job header retrieved " + jobID + " " + Job.JobDescription);
                                result = true;
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        result = false;
                        log.WriteLogEntry("SQL error " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        result = false;
                        log.WriteLogEntry("General program error " + ex.Message);
                    }
                }
            }
            log.WriteLogEntry("End LoadJobHeader.");
            return result;
        }
    }
}