using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using VeraAPI.Models;
using VeraAPI.Models.Forms;
using VeraAPI.Models.Templates;
using VeraAPI.Models.Tools;
using VeraAPI.Models.JobService;

namespace VeraAPI.Models.DataHandler
{
    public class FormDataHandler : SQLDataHandler
    {
        public BaseForm WebForm { get; private set; }
        public List<BaseForm> WebForms { get; private set; }
        public FormTemplate Template { get; private set; }
        //public string userEmail { get; set; }

        private static Scribe log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "FormDataHandler" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");

        public FormDataHandler(string dbServer, string dbName) : base(dbServer)
        {
            this.dbServer = dbServer;
            this.dbName = dbName;
            this.dataConnectionString = GetDataConnectionString();
            this.WebForm = new BaseForm();
        }

        public FormDataHandler(BaseForm webForm, string dbServer, string dbName) : base(dbServer)
        {
            this.dbServer = dbServer;
            this.dbName = dbName;
            this.dataConnectionString = GetDataConnectionString();
            this.WebForm = webForm;
        }

        public FormDataHandler(List<BaseForm> webForms, string dbServer, string dbName) : base(dbServer)
        {
            this.dbServer = dbServer;
            this.dbName = dbName;
            this.dataConnectionString = GetDataConnectionString();
            this.WebForms = webForms;
        }

        public bool InsertTravelAuth(TravelAuthForm travelForm)
        {
            log.WriteLogEntry("Starting InsertTravelAuth...");
            bool result = false;
            string tableName = Template.TableName;
            string cmdString = string.Format(@"insert into {0}.dbo.{1} (first_name, last_name, phone, email, event_description, event_location, depart_date, return_date, district_vehicle, district_vehicle_number, registration_amt, airfare_amt, rental_amt, 
                                    fuel_parking_amt, estimated_miles, lodging_amt, perdiem_amt, travel_days, misc_amt, request_advance, advance_amt, travel_policy, submit_date, submitter_approval, submitter_id, supervisor_approval, supervisor_id, 
                                    manager_approval, manager_id, approval_status) output inserted.travel_id 
                                    values (@firstName, @lastName, @phone, @email, @eventDescription, @eventLocation, @departDate, @returnDate, @districtVehicle, @districtVehicleNumber, @registrationAmt, @airfareAmt, @rentalAmt, @fuelParkingAmt, @estimatedMiles, 
                                    @lodgingAmt, @perdiemAmt, @travelDays, @miscAmt, @requestAdvance, @advanceAmt, @travelPolicy, GETDATE(), @submitterEmail, @submitterID, @supervisorEmail, @supervisorID, @managerEmail, @managerID, @status)", dbName, tableName);
            DateTime departDate = DateTime.MinValue, returnDate = DateTime.MinValue;
            bool districtVehicle = false, requestAdvance = false, travelPolicy = false;
            decimal registrationAmt = 0, airfareAmt = 0, rentalAmt = 0, fuelParkingAmt = 0, lodgingAmt = 0, perdiemAmt = 0, miscAmt = 0, advanceAmt = 0;
            int estimatedMiles = 0, travelDays = 0;
            int status = Constants.PendingValue;
            string districtVehicleNum = string.Empty;

            try
            {
                log.WriteLogEntry("TravelBegin: " + travelForm.TravelBegin);
                departDate = DateTime.Parse(travelForm.TravelBegin);
                log.WriteLogEntry("TravelEnd: " + travelForm.TravelEnd);
                returnDate = DateTime.Parse(travelForm.TravelEnd);
                log.WriteLogEntry("DistrictVehicle: " + travelForm.DistVehicle);
                districtVehicle = travelForm.DistVehicle == "true" ? true : false;
                log.WriteLogEntry("RegistrationCost: " + travelForm.RegistrationCost);
                registrationAmt = decimal.Parse(travelForm.RegistrationCost);
                log.WriteLogEntry("Airfare: " + travelForm.Airfare);
                airfareAmt = decimal.Parse(travelForm.Airfare);
                log.WriteLogEntry("RentalCar: " + travelForm.RentalCar);
                rentalAmt = decimal.Parse(travelForm.RentalCar);
                log.WriteLogEntry("FuelParking: " + travelForm.Fuel);
                fuelParkingAmt = decimal.Parse(travelForm.Fuel);
                log.WriteLogEntry("Mileage: " + travelForm.Mileage);
                estimatedMiles = int.Parse(travelForm.Mileage);
                log.WriteLogEntry("Lodging: " + travelForm.Lodging);
                lodgingAmt = decimal.Parse(travelForm.Lodging);
                log.WriteLogEntry("PerDiem: " + travelForm.PerDiem);
                perdiemAmt = decimal.Parse(travelForm.PerDiem);
                log.WriteLogEntry("TravelDays: " + travelForm.FullDays);
                travelDays = int.Parse(travelForm.FullDays);
                log.WriteLogEntry("Misc: " + travelForm.Misc);
                miscAmt = decimal.Parse(travelForm.Misc);
                log.WriteLogEntry("Advance: " + travelForm.Advance);
                requestAdvance = travelForm.Advance == "true" ? true : false;
                log.WriteLogEntry("AdvanceAmount: " + travelForm.AdvanceAmount);
                advanceAmt = decimal.Parse(travelForm.AdvanceAmount);
                log.WriteLogEntry("Policy: " + travelForm.Policy);
                travelPolicy = travelForm.Policy == "true" ? true : false;

            }
            catch (Exception ex)
            {
                log.WriteLogEntry("Form data conversion error: " + ex.Message);
                return result;
            }

            // Define the SQL connection and construct SQL query parameters
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    // Construction of query parameters based on travel authorization form data
                    //      and converted data from above
                    cmd.Parameters.AddWithValue("@firstName", travelForm.FirstName);
                    cmd.Parameters.AddWithValue("@lastName", travelForm.LastName);
                    cmd.Parameters.AddWithValue("@phone", travelForm.Phone);
                    cmd.Parameters.AddWithValue("@email", travelForm.Email);
                    cmd.Parameters.AddWithValue("@eventDescription", travelForm.EventTitle);
                    cmd.Parameters.AddWithValue("@eventLocation", travelForm.Location);
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
                    cmd.Parameters.AddWithValue("@submitterID", travelForm.UserID);
                    cmd.Parameters.AddWithValue("@submitterEmail", travelForm.SubmitterEmail);
                    cmd.Parameters.AddWithValue("@supervisorID", travelForm.DeptHeadID);
                    cmd.Parameters.AddWithValue("@supervisorEmail", travelForm.DeptHeadEmail);
                    cmd.Parameters.AddWithValue("@managerID", travelForm.GeneralManagerID);
                    cmd.Parameters.AddWithValue("@managerEmail", travelForm.GeneralManagerEmail);
                    cmd.Parameters.AddWithValue("@status", status);
                    try
                    {
                        // Try opening the SQL connection and executing the above constructed SQL query
                        conn.Open();
                        travelForm.FormDataID = (int)cmd.ExecuteScalar();
                        result = true;
                        log.WriteLogEntry("Successful insert travel ID " + result);
                    }
                    catch (SqlException ex)
                    {
                        log.WriteLogEntry("SQL error " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        log.WriteLogEntry("General program error " + ex.Message);
                    }
                }
            }
            log.WriteLogEntry("End InsertTravelAuth.");
            return result;
        }

        public bool LoadFormTemplate(int templateID)
        {
            log.WriteLogEntry("Begin LoadFormTemplate...");
            bool result = false;
            string cmdString = string.Format(@"select * from {0}.dbo.job_template where template_id = @templateID", dbName);
            log.WriteLogEntry("SQL command string: " + cmdString);
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
                                FormTemplate template = new FormTemplate()
                                {
                                    TemplateID = (int)rdr["template_id"],
                                    TemplateName = rdr["template_name"].ToString(),
                                    TableName = rdr["table_name"].ToString(),
                                    JobDescription = rdr["job_description"].ToString(),
                                    JobWeight = (int)rdr["job_weight"],
                                    JobPriority = (int)rdr["job_priority"],
                                    JobType = rdr["job_type"].ToString()
                                };
                                log.WriteLogEntry("Retrieved form template " + template.TemplateID + " " + template.TemplateName);
                                this.Template = template;
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
            log.WriteLogEntry("End LoadFormTemplate.");
            return result;
        }

        public bool LoadTravelAuthForm()
        {
            log.WriteLogEntry("Begin LoadTravelAuthForm...");
            bool result = false;
            string cmdString = string.Format(@"select * from {0}.dbo.travel where travel_id = @dataID", dbName);
            log.WriteLogEntry("SQL command string: " + cmdString);
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    cmd.Parameters.AddWithValue("@dataID", WebForm.FormDataID);
                    try
                    {
                        conn.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                // SINCE WE CHANGED VARIABLE NAMES DO WE NEED TO CHANGE THESE???
                                // Database column names will remain descriptive for now and use schema field mapping
                                TravelAuthForm travel = new TravelAuthForm(WebForm.FormDataID);
                                travel.FirstName = rdr["first_name"].ToString();
                                travel.LastName = rdr["last_name"].ToString();
                                travel.Phone = rdr["phone"].ToString();
                                travel.Email = rdr["email"].ToString();
                                travel.EventTitle = rdr["event_description"].ToString();
                                travel.Location = rdr["event_location"].ToString();
                                travel.TravelBegin = rdr["depart_date"].ToString();
                                travel.TravelEnd = rdr["return_date"].ToString();
                                travel.DistVehicle = rdr["district_vehicle"].ToString();
                                travel.RegistrationCost = rdr["registration_amt"].ToString();
                                travel.Airfare = rdr["airfare_amt"].ToString();
                                travel.RentalCar = rdr["rental_amt"].ToString();
                                travel.Fuel = rdr["fuel_parking_amt"].ToString();
                                travel.Mileage = rdr["estimated_miles"].ToString();
                                travel.Lodging = rdr["lodging_amt"].ToString();
                                travel.PerDiem = rdr["perdiem_amt"].ToString();
                                travel.FullDays = rdr["travel_days"].ToString();
                                travel.Misc = rdr["misc_amt"].ToString();
                                travel.TotalEstimate = rdr["total_cost"].ToString();
                                travel.AdvanceAmount = rdr["advance_amt"].ToString();
                                travel.Advance = rdr["request_advance"].ToString();
                                travel.Policy = rdr["travel_policy"].ToString();
                                travel.Preparer = rdr["preparer_name"].ToString();
                                travel.SubmitterEmail = rdr["submitter_approval"].ToString();
                                int status = (int)rdr["approval_status"];
                                switch (status)
                                {
                                    case 0:
                                        travel.ApprovalStatus = Constants.DeniedColor;
                                        break;
                                    case 1:
                                        travel.ApprovalStatus = Constants.ApprovedColor;
                                        break;
                                    case 2:
                                        travel.ApprovalStatus = Constants.PendingColor;
                                        break;
                                    default:
                                        travel.ApprovalStatus = Constants.DeniedColor;
                                        break;
                                }
                                log.WriteLogEntry("Retrieved travel data " + travel.FormDataID + " " + travel.EventTitle);
                                WebForm = travel;
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
            log.WriteLogEntry("End LoadTravelAuthForm.");
            return result;
        }

        public int LoadUserTravelAuthForms(int userID)
        {
            log.WriteLogEntry("Begin LoadUserTravelAuthForms...");
            log.WriteLogEntry("User ID" + userID);
            int result = 0;
            List<BaseForm> travelForms = new List<BaseForm>();

            // Load list of travel auth forms where submitter_approval = userID
            string cmdString = string.Format(@"select * from {0}.dbo.travel where submitter_id = @userID", dbName);
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    cmd.Parameters.AddWithValue("@userID", userID);
                    try
                    {
                        conn.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                TravelAuthForm travel = new TravelAuthForm();
                                travel.UserID = userID;
                                travel.FormDataID = (int)rdr["travel_id"];
                                travel.FirstName = rdr["first_name"].ToString();
                                travel.LastName = rdr["last_name"].ToString();
                                travel.Phone = rdr["phone"].ToString();
                                travel.Email = rdr["email"].ToString();
                                travel.EventTitle = rdr["event_description"].ToString();
                                travel.Location = rdr["event_location"].ToString();
                                travel.TravelBegin = rdr["depart_date"].ToString();
                                travel.TravelEnd = rdr["return_date"].ToString();
                                travel.DistVehicle = rdr["district_vehicle"].ToString();
                                travel.RegistrationCost = rdr["registration_amt"].ToString();
                                travel.Airfare = rdr["airfare_amt"].ToString();
                                travel.RentalCar = rdr["rental_amt"].ToString();
                                travel.Fuel = rdr["fuel_parking_amt"].ToString();
                                travel.Mileage = rdr["estimated_miles"].ToString();
                                travel.Lodging = rdr["lodging_amt"].ToString();
                                travel.PerDiem = rdr["perdiem_amt"].ToString();
                                travel.FullDays = rdr["travel_days"].ToString();
                                travel.Misc = rdr["misc_amt"].ToString();
                                travel.TotalEstimate = rdr["total_cost"].ToString();
                                travel.AdvanceAmount = rdr["advance_amt"].ToString();
                                travel.Advance = rdr["request_advance"].ToString();
                                travel.Policy = rdr["travel_policy"].ToString();
                                travel.Preparer = rdr["preparer_name"].ToString();
                                travel.SubmitterEmail = rdr["submitter_approval"].ToString();
                                int status = (int)rdr["approval_status"];
                                switch (status)
                                {
                                    case 0:
                                        travel.ApprovalStatus = Constants.DeniedColor;
                                        break;
                                    case 1:
                                        travel.ApprovalStatus = Constants.ApprovedColor;
                                        break;
                                    case 2:
                                        travel.ApprovalStatus = Constants.PendingColor;
                                        break;
                                    default:
                                        travel.ApprovalStatus = Constants.DeniedColor;
                                        break;
                                }
                                log.WriteLogEntry(string.Format("Retrieved travel data {0} {1} {2} {3}", travel.FormDataID, travel.EventTitle, travel.SubmitterEmail, travel.UserID));
                                travelForms.Add(travel);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        log.WriteLogEntry("SQL error " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        log.WriteLogEntry("General program error " + ex.Message);
                    }
                }
            }
            WebForms = travelForms;
            result = WebForms.Count;
            log.WriteLogEntry("End LoadUserTravelAuthForms.");
            return result;
        }

        public int LoadApproverTravelAuthForms(int userID)
        {
            log.WriteLogEntry("Begin LoadApproverTravelAuthForms...");
            log.WriteLogEntry("User ID" + userID);
            int result = 0;
            List<BaseForm> travelForms = new List<BaseForm>();

            // Load list of travel auth forms where submitter_approval = userID
            string cmdString = string.Format(@"select * from {0}.dbo.travel where supervisor_id = @userID or manager_id = @userID", dbName);
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    cmd.Parameters.AddWithValue("@userID", userID);
                    try
                    {
                        conn.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                TravelAuthForm travel = new TravelAuthForm();
                                travel.UserID = userID;
                                travel.FormDataID = (int)rdr["travel_id"];
                                travel.FirstName = rdr["first_name"].ToString();
                                travel.LastName = rdr["last_name"].ToString();
                                travel.Phone = rdr["phone"].ToString();
                                travel.Email = rdr["email"].ToString();
                                travel.EventTitle = rdr["event_description"].ToString();
                                travel.Location = rdr["event_location"].ToString();
                                travel.TravelBegin = rdr["depart_date"].ToString();
                                travel.TravelEnd = rdr["return_date"].ToString();
                                travel.DistVehicle = rdr["district_vehicle"].ToString();
                                travel.RegistrationCost = rdr["registration_amt"].ToString();
                                travel.Airfare = rdr["airfare_amt"].ToString();
                                travel.RentalCar = rdr["rental_amt"].ToString();
                                travel.Fuel = rdr["fuel_parking_amt"].ToString();
                                travel.Mileage = rdr["estimated_miles"].ToString();
                                travel.Lodging = rdr["lodging_amt"].ToString();
                                travel.PerDiem = rdr["perdiem_amt"].ToString();
                                travel.FullDays = rdr["travel_days"].ToString();
                                travel.Misc = rdr["misc_amt"].ToString();
                                travel.TotalEstimate = rdr["total_cost"].ToString();
                                travel.AdvanceAmount = rdr["advance_amt"].ToString();
                                travel.Advance = rdr["request_advance"].ToString();
                                travel.Policy = rdr["travel_policy"].ToString();
                                travel.Preparer = rdr["preparer_name"].ToString();
                                travel.SubmitterEmail = rdr["submitter_approval"].ToString();
                                int status = (int)rdr["approval_status"];
                                switch (status)
                                {
                                    case 0:
                                        travel.ApprovalStatus = Constants.DeniedColor;
                                        break;
                                    case 1:
                                        travel.ApprovalStatus = Constants.ApprovedColor;
                                        break;
                                    case 2:
                                        travel.ApprovalStatus = Constants.PendingColor;
                                        break;
                                    default:
                                        travel.ApprovalStatus = Constants.DeniedColor;
                                        break;
                                }
                                log.WriteLogEntry(string.Format("Retrieved travel data {0} {1} {2} {3}", travel.FormDataID, travel.EventTitle, travel.SubmitterEmail, travel.UserID));
                                travelForms.Add(travel);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        log.WriteLogEntry("SQL error " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        log.WriteLogEntry("General program error " + ex.Message);
                    }
                }
            }
            WebForms = travelForms;
            result = WebForms.Count;
            log.WriteLogEntry("End LoadApproverTravelAuthForms.");
            return result;
        }
    }
}