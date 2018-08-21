using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using VeraAPI.Models.Forms;


namespace VeraAPI.Models.DataHandler
{
    public class UIDataHandler : SQLDataHandler
    {
        private Scribe Log = null;
        private string dataConnectionString = string.Empty;
        private string dbServer = string.Empty;
        private string dbName = string.Empty;
        public JobHeader Job { get; private set; }
        public JobTemplate Template { get; private set; }
        public BaseForm FormData { get; set; } = new BaseForm();
        public string userEmail { get; set; }

        public UIDataHandler(string dbServer, string dbName) : base(dbServer)
        {
            this.dbServer = dbServer;
            this.dbName = dbName;
            this.Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UIDataHandler_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
            this.dataConnectionString = GetDataConnectionString();
        }

        public UIDataHandler(string dbServer, string dbName, Scribe Log) : base(dbServer)
        {
            this.dbServer = dbServer;
            this.dbName = dbName;
            this.Log = Log;
            this.dataConnectionString = GetDataConnectionString();
        }

        public bool InsertJob()
        {
            Log.WriteLogEntry("Begin InsertJob...");
            bool result = false;
            string cmdString = string.Format(@"insert into {0}.dbo.job_header (template_id, data_id, job_description, job_priority, job_weight, job_type, entry_dt) output inserted.job_id values (@templateID, @dataID, @jobDescription, @jobPriority, @jobWeight, @jobType, GETDATE())", dbName);
            // Check JobTemplate and FormData objects for required ID data prior to calling SQL query
            //      if TemplateID or FormDataID are missing skip the SQL call and log the reason
            if (Template.TemplateID > 0 || FormData.FormDataID > 0)
            {
                using (SqlConnection conn = new SqlConnection(dataConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                    {
                        cmd.Parameters.AddWithValue("@templateID", Template.TemplateID);
                        cmd.Parameters.AddWithValue("@dataID", FormData.FormDataID);
                        cmd.Parameters.AddWithValue("@jobDescription", Template.JobDescription);
                        cmd.Parameters.AddWithValue("@jobPriority", Template.JobPriority);
                        cmd.Parameters.AddWithValue("@jobWeight", Template.JobWeight);
                        cmd.Parameters.AddWithValue("@jobType", Template.JobType);
                        try
                        {
                            conn.Open();
                            int jobID = (int)cmd.ExecuteScalar();
                            result = true;
                            Log.WriteLogEntry("Successful insert job " + jobID);
                        }
                        catch (SqlException ex)
                        {
                            result = false;
                            Log.WriteLogEntry("SQL error " + ex.Message);
                        }
                        catch (Exception ex)
                        {
                            result = false;
                            Log.WriteLogEntry("General program error " + ex.Message);
                        }
                    }
                }
            }
            else if (Template.TemplateID < 1)
            {
                result = false;
                Log.WriteLogEntry("No job template loaded.");
            }
            else if (FormData.FormDataID < 1)
            {
                result = false;
                Log.WriteLogEntry("No job template loaded.");
            }
            Log.WriteLogEntry("End InsertJob.");
            return result;
        }

        public bool InsertFormData()
        {
            bool result = false;

            switch (this.FormData.TemplateID)
            {
                case 1:
                    result = InsertTravelAuth();
                    break;
                default:
                    // Redundnant setting result to false as safeguard for default case
                    result = false;
                    break;
            }
            return result;
        }

        private bool InsertTravelAuth()
        {
            // Returns the SQL generated travel_id from the travel table
            Log.WriteLogEntry("Starting InsertTravelAuth...");
            bool result = false;
            TravelAuthForm Travel = (TravelAuthForm)FormData;
            if (Travel.SubmitterSig != null)
            {
                string tableName = Template.TableName;
                string cmdString = string.Format(@"insert into {0}.dbo.{1} (first_name, last_name, phone, email, event_description, event_location, depart_date, return_date, district_vehicle, district_vehicle_number, registration_amt, airfare_amt, rental_amt, 
                                            fuel_parking_amt, estimated_miles, lodging_amt, perdiem_amt, travel_days, misc_amt, request_advance, advance_amt, travel_policy, submit_date, submitter_approval) output inserted.travel_id 
                                            values (@firstName, @lastName, @phone, @email, @eventDescription, @eventLocation, @departDate, @returnDate, @districtVehicle, @districtVehicleNumber, @registrationAmt, @airfareAmt, @rentalAmt, @fuelParkingAmt, @estimatedMiles, 
                                            @lodgingAmt, @perdiemAmt, @travelDays, @miscAmt, @requestAdvance, @advanceAmt, @travelPolicy, GETDATE(), @submitterSig)", dbName, tableName);
                DateTime departDate = DateTime.MinValue, returnDate = DateTime.MinValue;
                bool districtVehicle = false, requestAdvance = false, travelPolicy = false;
                decimal registrationAmt = 0, airfareAmt = 0, rentalAmt = 0, fuelParkingAmt = 0, lodgingAmt = 0, perdiemAmt = 0, miscAmt = 0, advanceAmt = 0;
                int estimatedMiles = 0, travelDays = 0;
                string districtVehicleNum = string.Empty;

                //Capture email address for notification
                userEmail = Travel.SubmitterSig;
                Log.WriteLogEntry("Travel Auth user email set to submitter signature " + userEmail);

                // Attempt data typing of required form fields prior to SQL call
                // Set the result to false and return in the catch and do not insert the form data
                // Verbose logging to assist debugging
                try
                {
                    Log.WriteLogEntry("Try conversion of data form fields to correct types.");
                    Log.WriteLogEntry("TravelBegin: " + Travel.TravelBegin);
                    departDate = DateTime.Parse(Travel.TravelBegin);
                    Log.WriteLogEntry("TravelEnd: " + Travel.TravelEnd);
                    returnDate = DateTime.Parse(Travel.TravelEnd);
                    Log.WriteLogEntry("DistrictVehicle: " + Travel.DistVehicle);
                    districtVehicle = Travel.DistVehicle == "true" ? true : false;
                    Log.WriteLogEntry("DistrictVehicleNum: " + Travel.DistVehicleNum);
                    districtVehicleNum = Travel.DistVehicleNum.ToString();
                    Log.WriteLogEntry("RegistrationCost: " + Travel.RegistrationCost);
                    registrationAmt = decimal.Parse(Travel.RegistrationCost);
                    Log.WriteLogEntry("Airfare: " + Travel.Airfare);
                    airfareAmt = decimal.Parse(Travel.Airfare);
                    Log.WriteLogEntry("RentalCar: " + Travel.RentalCar);
                    rentalAmt = decimal.Parse(Travel.RentalCar);
                    Log.WriteLogEntry("FuelParking: " + Travel.FuelParking);
                    fuelParkingAmt = decimal.Parse(Travel.FuelParking);
                    Log.WriteLogEntry("Mileage: " + Travel.Mileage);
                    estimatedMiles = int.Parse(Travel.Mileage);
                    Log.WriteLogEntry("Lodging: " + Travel.Lodging);
                    lodgingAmt = decimal.Parse(Travel.Lodging);
                    Log.WriteLogEntry("PerDiem: " + Travel.PerDiem);
                    perdiemAmt = decimal.Parse(Travel.PerDiem);
                    Log.WriteLogEntry("TravelDays: " + Travel.FullDays);
                    travelDays = int.Parse(Travel.FullDays);
                    Log.WriteLogEntry("Misc: " + Travel.Misc);
                    miscAmt = decimal.Parse(Travel.Misc);
                    Log.WriteLogEntry("Advance: " + Travel.Advance);
                    requestAdvance = Travel.Advance == "true" ? true : false;
                    Log.WriteLogEntry("AdvanceAmount: " + Travel.AdvanceAmount);
                    advanceAmt = decimal.Parse(Travel.AdvanceAmount);
                    Log.WriteLogEntry("Policy: " + Travel.Policy);
                    travelPolicy = Travel.Policy == "true" ? true : false;

                }
                catch (Exception ex)
                {
                    result = false;
                    Log.WriteLogEntry("Form data conversion error: " + ex.Message);
                    return result;
                }

                // Define the SQL connection and construct SQL query parameters
                using (SqlConnection conn = new SqlConnection(dataConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                    {
                        // Construction of query parameters based on travel authorization form data
                        //      and converted data from above
                        cmd.Parameters.AddWithValue("@firstName", Travel.FirstName);
                        cmd.Parameters.AddWithValue("@lastName", Travel.LastName);
                        cmd.Parameters.AddWithValue("@phone", Travel.Phone);
                        cmd.Parameters.AddWithValue("@email", Travel.Email);
                        cmd.Parameters.AddWithValue("@eventDescription", Travel.EventTitle);
                        cmd.Parameters.AddWithValue("@eventLocation", Travel.Location);
                        cmd.Parameters.AddWithValue("@departDate", departDate);
                        cmd.Parameters.AddWithValue("@returnDate", returnDate);
                        cmd.Parameters.AddWithValue("@districtVehicle", districtVehicle);
                        cmd.Parameters.AddWithValue("@districtVehicleNumber", districtVehicleNum);
                        cmd.Parameters.AddWithValue("@registrationAmt", registrationAmt);
                        cmd.Parameters.AddWithValue("@airfareAmt", airfareAmt);
                        cmd.Parameters.AddWithValue("@rentalAmt", rentalAmt);
                        cmd.Parameters.AddWithValue("@fuelParkingAmt", fuelParkingAmt);
                        cmd.Parameters.AddWithValue("@estimatedMiles", estimatedMiles);
                        cmd.Parameters.AddWithValue("@lodgingAmt", lodgingAmt);
                        cmd.Parameters.AddWithValue("@perdiemAmt", perdiemAmt);
                        cmd.Parameters.AddWithValue("@travelDays", travelDays);
                        cmd.Parameters.AddWithValue("@miscAmt", miscAmt);
                        cmd.Parameters.AddWithValue("@requestAdvance", requestAdvance);
                        cmd.Parameters.AddWithValue("@advanceAmt", advanceAmt);
                        cmd.Parameters.AddWithValue("@travelPolicy", travelPolicy);
                        cmd.Parameters.AddWithValue("@submitterSig", Travel.SubmitterSig);
                        try
                        {
                            // Try opening the SQL connection and executing the above constructed SQL query
                            conn.Open();
                            this.FormData.FormDataID = (int)cmd.ExecuteScalar();
                            result = true;
                            Log.WriteLogEntry("Successful insert travel ID " + result);
                        }
                        catch (SqlException ex)
                        {
                            result = false;
                            Log.WriteLogEntry("SQL error " + ex.Message);
                        }
                        catch (Exception ex)
                        {
                            result = false;
                            Log.WriteLogEntry("General program error " + ex.Message);
                        }
                    }
                }
            }
            else
                Log.WriteLogEntry("Missing submitter signature!");
            Log.WriteLogEntry("End InsertTravelAuth.");
            return result;
        }

        public bool LoadJobTemplate(int templateID)
        {
            Log.WriteLogEntry("Begin LoadJobTemplate...");
            bool result = false;
            string cmdString = string.Format(@"select * from {0}.dbo.job_template where template_id = @templateID", dbName);
            Log.WriteLogEntry("SQL command string: " + cmdString);
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    cmd.Parameters.AddWithValue("@templateID", templateID);
                    try
                    {
                        conn.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                Template = new JobTemplate(templateID);
                                Template.TemplateName = rdr["template_name"].ToString();
                                Template.TableName = rdr["table_name"].ToString();
                                Template.JobDescription = rdr["job_description"].ToString();
                                Template.JobPriority = (int)rdr["job_priority"];
                                Template.JobWeight = (int)rdr["job_weight"];
                                Template.JobType = rdr["job_type"].ToString();
                                Log.WriteLogEntry("Retrieved job template " + templateID + " " + Template.TemplateName);
                                result = true;
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        result = false;
                        Log.WriteLogEntry("SQL error " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        result = false;
                        Log.WriteLogEntry("General program error " + ex.Message);
                    }
                }
            }
            Log.WriteLogEntry("End LoadJobTemplate.");
            return result;
        }

        public bool LoadJobHeader(int jobID)
        {
            Log.WriteLogEntry("Begin LoadJobHeader...");
            bool result = false;
            string cmdString = string.Format(@"select * from {0}.dbo.job_header where job_id = @jobID", dbName);
            Log.WriteLogEntry("SQL command string: " + cmdString);
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
                                Job.DataID = (int)rdr["data_id"];
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
                                Log.WriteLogEntry("Job header retrieved " + jobID + " " + Job.JobDescription);
                                result = true;
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        result = false;
                        Log.WriteLogEntry("SQL error " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        result = false;
                        Log.WriteLogEntry("General program error " + ex.Message);
                    }
                }
            }
            Log.WriteLogEntry("End LoadJobHeader.");
            return result;
        }

        public bool LoadTravelAuth(int dataID)
        {
            Log.WriteLogEntry("Begin LoadTravelAuth...");
            bool result = false;
            string cmdString = string.Format(@"select * from {0}.dbo.travel where travel_id = @dataID", dbName);
            Log.WriteLogEntry("SQL command string: " + cmdString);
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    cmd.Parameters.AddWithValue("@dataID", dataID);
                    try
                    {
                        conn.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                TravelAuthForm Travel = new TravelAuthForm(dataID);
                                Travel.FirstName = rdr["first_name"].ToString();
                                Travel.LastName = rdr["last_name"].ToString();
                                Travel.Phone = rdr["phone"].ToString();
                                Travel.Email = rdr["email"].ToString();
                                Travel.EventTitle = rdr["event_description"].ToString();
                                Travel.Location = rdr["event_location"].ToString();
                                Travel.TravelBegin = rdr["depart_date"].ToString();
                                Travel.TravelEnd = rdr["return_date"].ToString();
                                Travel.DistVehicle = rdr["district_vehicle"].ToString();
                                Travel.DistVehicleNum = rdr["district_vehicle_number"].ToString();
                                Travel.RegistrationCost = rdr["registration_amt"].ToString();
                                Travel.Airfare = rdr["airfare_amt"].ToString();
                                Travel.RentalCar = rdr["rental_amt"].ToString();
                                Travel.FuelParking = rdr["fuel_parking_amt"].ToString();
                                Travel.Mileage = rdr["estimated_miles"].ToString();
                                Travel.Lodging = rdr["lodging_amt"].ToString();
                                Travel.PerDiem = rdr["perdiem_amt"].ToString();
                                Travel.FullDays = rdr["travel_days"].ToString();
                                Travel.Misc = rdr["misc_amt"].ToString();
                                Travel.total = rdr["total_cost"].ToString();
                                Travel.AdvanceAmount = rdr["advance_amt"].ToString();
                                Travel.Advance = rdr["request_advance"].ToString();
                                Travel.Policy = rdr["travel_policy"].ToString();
                                Travel.Preparer = rdr["preparer_name"].ToString();
                                Travel.SubmitterSig = rdr["submitter_approval"].ToString();
                                Log.WriteLogEntry("Retrieved travel data " + dataID + " " + Travel.EventTitle);
                                result = true;
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        result = false;
                        Log.WriteLogEntry("SQL error " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        result = false;
                        Log.WriteLogEntry("General program error " + ex.Message);
                    }
                }
            }
            Log.WriteLogEntry("End LoadTravelAuth.");
            return result;
        }
    }
}