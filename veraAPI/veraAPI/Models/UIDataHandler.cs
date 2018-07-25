using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using veraAPI.Models.Forms;


namespace veraAPI.Models
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
                    this.FormData.FormDataID = InsertTravelAuth();
                    result = true;
                    break;
                default:
                    result = false;
                    break;
            }
            return result;
        }

        private int InsertTravelAuth()
        {
            //Returns the SQL generated travel_id from the travel table
            Log.WriteLogEntry("Starting InsertTravelAuth...");
            TravelAuthForm Travel = (TravelAuthForm)FormData;
            int result = 0;
            string tableName = Template.TableName;
            string cmdString = string.Format(@"insert into {0}.dbo.{1} (first_name, last_name, phone, email, event_description, event_location, depart_date, return_date, district_vehicle, registration_amt, airfare_amt, rental_amt, 
                                            fuel_parking_amt, estimated_miles, lodging_amt, perdiem_amt, travel_days, misc_amt, request_advance, advance_amt, travel_policy) output inserted.travel_id 
                                            values (@firstName, @lastName, @phone, @email, @eventDescription, @eventLocation, @departDate, @returnDate, @districtVehicle, @registrationAmt, @airfareAmt, @rentalAmt, @fuelParkingAmt, @estimatedMiles, 
                                            @lodgingAmt, @perdiemAmt, @travelDays, @miscAmt, @requestAdvance, @advanceAmt, @travelPolicy)", dbName, tableName);
            DateTime departDate = DateTime.Parse(Travel.TravelBegin);
            DateTime returnDate = DateTime.Parse(Travel.TravelEnd);
            bool districtVehicle = Travel.DistVehicle == "true" ? true : false;
            decimal registrationAmt = decimal.Parse(Travel.RegistrationCost);
            decimal airfareAmt = decimal.Parse(Travel.Airfare);
            decimal rentalAmt = decimal.Parse(Travel.RentalCar);
            decimal fuelParkingAmt = decimal.Parse(Travel.FuelParking);
            int estimatedMiles = int.Parse(Travel.Mileage);
            decimal lodgingAmt = decimal.Parse(Travel.Lodging);
            decimal perdiemAmt = decimal.Parse(Travel.PerDiem);
            int travelDays = int.Parse(Travel.FullDays);
            decimal miscAmt = decimal.Parse(Travel.Misc);
            bool requestAdvance = Travel.Advance == "true" ? true : false;
            decimal advanceAmt = decimal.Parse(Travel.AdvanceAmount);
            bool travelPolicy = Travel.Policy == "true" ? true : false;

            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    cmd.Parameters.AddWithValue("@firstName", Travel.FirstName);
                    cmd.Parameters.AddWithValue("@lastName", Travel.LastName);
                    cmd.Parameters.AddWithValue("@phone", Travel.Phone);
                    cmd.Parameters.AddWithValue("@email", Travel.Email);
                    cmd.Parameters.AddWithValue("@eventDescription", Travel.EventTitle);
                    cmd.Parameters.AddWithValue("@eventLocation", Travel.Location);
                    cmd.Parameters.AddWithValue("@departDate", departDate);
                    cmd.Parameters.AddWithValue("@returnDate", returnDate);
                    cmd.Parameters.AddWithValue("@districtVehicle", districtVehicle);
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
                    try
                    {
                        conn.Open();
                        result = (int)cmd.ExecuteScalar();
                        Log.WriteLogEntry("Successful insert travel ID " + result);
                    }
                    catch (SqlException ex)
                    {
                        result = -1;
                        Log.WriteLogEntry("SQL error " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        result = -1;
                        Log.WriteLogEntry("General program error " + ex.Message);
                    }
                }
            }
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
                                Log.WriteLogEntry("Retrieved job template " + templateID);
                                Template = new JobTemplate(templateID);
                                Template.TemplateName = rdr["template_name"].ToString();
                                Template.TableName = rdr["table_name"].ToString();
                                Template.JobDescription = rdr["job_description"].ToString();
                                Template.JobPriority = (int)rdr["job_priority"];
                                Template.JobWeight = (int)rdr["job_weight"];
                                Template.JobType = rdr["job_type"].ToString();
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
    }
}