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

        public bool InsertTravelAuth(TravelAuthForm travelForm)
        {
            log.WriteLogEntry("Starting InsertTravelAuth...");
            bool result = false;
            string tableName = Template.TableName;
            string cmdString = string.Format(@"insert into {0}.dbo.{1} (first_name, last_name, phone, email, event_description, event_location, depart_date, return_date, district_vehicle, district_vehicle_number, registration_amt, airfare_amt, rental_amt, 
                                    fuel_parking_amt, estimated_miles, lodging_amt, perdiem_amt, travel_days, misc_amt, request_advance, advance_amt, travel_policy, submit_date, submitter_id, supervisor_id, manager_id, approval_status) 
                                    output inserted.form_id
                                    values (@firstName, @lastName, @phone, @email, @eventDescription, @eventLocation, @departDate, @returnDate, @districtVehicle, @districtVehicleNumber, @registrationAmt, @airfareAmt, @rentalAmt, @fuelParkingAmt, @estimatedMiles, 
                                    @lodgingAmt, @perdiemAmt, @travelDays, @miscAmt, @requestAdvance, @advanceAmt, @travelPolicy, GETDATE(), @submitterID, @supervisorID, @managerID, @status)", dbName, tableName);
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
                    cmd.Parameters.AddWithValue("@supervisorID", travelForm.DHID);
                    cmd.Parameters.AddWithValue("@managerID", travelForm.GMID);
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

        public bool LoadTravelAuthForm(int formDataID)
        {
            log.WriteLogEntry("Begin LoadTravelAuthForm...");
            bool result = false;
            string cmdString = string.Format(@"select * from {0}.dbo.travel where form_id = @dataID", dbName);
            log.WriteLogEntry("SQL command string: " + cmdString);
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    cmd.Parameters.AddWithValue("@dataID", formDataID);
                    try
                    {
                        conn.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                TravelAuthForm travel = new TravelAuthForm
                                {
                                    FormDataID = (int)rdr["form_id"],
                                    FirstName = rdr["first_name"].ToString(),
                                    LastName = rdr["last_name"].ToString(),
                                    Phone = rdr["phone"].ToString(),
                                    Email = rdr["email"].ToString(),
                                    EventTitle = rdr["event_description"].ToString(),
                                    Location = rdr["event_location"].ToString(),
                                    TravelBegin = rdr["depart_date"].ToString(),
                                    TravelEnd = rdr["return_date"].ToString(),
                                    DistVehicle = rdr["district_vehicle"].ToString(),
                                    RegistrationCost = rdr["registration_amt"].ToString(),
                                    Airfare = rdr["airfare_amt"].ToString(),
                                    RentalCar = rdr["rental_amt"].ToString(),
                                    Fuel = rdr["fuel_parking_amt"].ToString(),
                                    Mileage = rdr["estimated_miles"].ToString(),
                                    Lodging = rdr["lodging_amt"].ToString(),
                                    PerDiem = rdr["perdiem_amt"].ToString(),
                                    FullDays = rdr["travel_days"].ToString(),
                                    Misc = rdr["misc_amt"].ToString(),
                                    TotalEstimate = rdr["total_cost"].ToString(),
                                    AdvanceAmount = rdr["advance_amt"].ToString(),
                                    Advance = rdr["request_advance"].ToString(),
                                    Policy = rdr["travel_policy"].ToString(),
                                    Preparer = rdr["preparer_name"].ToString(),
                                    SubmitterSig = rdr["submitter_id"].ToString()
                                };
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
                                this.WebForm = travel;
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
                                TravelAuthForm travel = new TravelAuthForm
                                {
                                    UserID = (int)rdr["submitter_id"],
                                    FormDataID = (int)rdr["form_id"],
                                    FirstName = rdr["first_name"].ToString(),
                                    LastName = rdr["last_name"].ToString(),
                                    Phone = rdr["phone"].ToString(),
                                    Email = rdr["email"].ToString(),
                                    EventTitle = rdr["event_description"].ToString(),
                                    Location = rdr["event_location"].ToString(),
                                    TravelBegin = rdr["depart_date"].ToString(),
                                    TravelEnd = rdr["return_date"].ToString(),
                                    DistVehicle = rdr["district_vehicle"].ToString(),
                                    RegistrationCost = rdr["registration_amt"].ToString(),
                                    Airfare = rdr["airfare_amt"].ToString(),
                                    RentalCar = rdr["rental_amt"].ToString(),
                                    Fuel = rdr["fuel_parking_amt"].ToString(),
                                    Mileage = rdr["estimated_miles"].ToString(),
                                    Lodging = rdr["lodging_amt"].ToString(),
                                    PerDiem = rdr["perdiem_amt"].ToString(),
                                    FullDays = rdr["travel_days"].ToString(),
                                    Misc = rdr["misc_amt"].ToString(),
                                    TotalEstimate = rdr["total_cost"].ToString(),
                                    AdvanceAmount = rdr["advance_amt"].ToString(),
                                    Advance = rdr["request_advance"].ToString(),
                                    Policy = rdr["travel_policy"].ToString(),
                                    Preparer = rdr["preparer_name"].ToString(),
                                    SubmitterSig = rdr["submitter_id"].ToString()
                                };
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
                                log.WriteLogEntry(string.Format("Retrieved travel data {0} {1} {2} {3}", travel.FormDataID, travel.EventTitle, travel.SubmitterSig, travel.UserID));
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
                                TravelAuthForm travel = new TravelAuthForm
                                {
                                    UserID = (int)rdr["submitter_id"],
                                    FormDataID = (int)rdr["form_id"],
                                    FirstName = rdr["first_name"].ToString(),
                                    LastName = rdr["last_name"].ToString(),
                                    Phone = rdr["phone"].ToString(),
                                    Email = rdr["email"].ToString(),
                                    EventTitle = rdr["event_description"].ToString(),
                                    Location = rdr["event_location"].ToString(),
                                    TravelBegin = rdr["depart_date"].ToString(),
                                    TravelEnd = rdr["return_date"].ToString(),
                                    DistVehicle = rdr["district_vehicle"].ToString(),
                                    RegistrationCost = rdr["registration_amt"].ToString(),
                                    Airfare = rdr["airfare_amt"].ToString(),
                                    RentalCar = rdr["rental_amt"].ToString(),
                                    Fuel = rdr["fuel_parking_amt"].ToString(),
                                    Mileage = rdr["estimated_miles"].ToString(),
                                    Lodging = rdr["lodging_amt"].ToString(),
                                    PerDiem = rdr["perdiem_amt"].ToString(),
                                    FullDays = rdr["travel_days"].ToString(),
                                    Misc = rdr["misc_amt"].ToString(),
                                    TotalEstimate = rdr["total_cost"].ToString(),
                                    AdvanceAmount = rdr["advance_amt"].ToString(),
                                    Advance = rdr["request_advance"].ToString(),
                                    Policy = rdr["travel_policy"].ToString(),
                                    Preparer = rdr["preparer_name"].ToString(),
                                    SubmitterSig = rdr["submitter_id"].ToString()
                                };
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
                                log.WriteLogEntry(string.Format("Retrieved travel data {0} {1} {2} {3}", travel.FormDataID, travel.EventTitle, travel.SubmitterSig, travel.UserID));
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

        public int UpdateForm(string[,] formFields, string[,] formFilters)
        {
            log.WriteLogEntry("Begin UpdateForm...");
            int result = 0;
            StringBuilder sb = new StringBuilder(string.Format("update {0}.dbo.{1} set ", dbServer, dbName));
            for (int i = 0; i < formFields.GetUpperBound(0); i++)
            {
                for (int j = 0; j < formFields.GetUpperBound(1); j++)
                {
                    sb.Append(formFields[i,j] + " = @" + formFields[i,j]);
                    while (i < formFields.GetUpperBound(0) - 1)
                    {
                        sb.Append(", ");
                    }
                }
            }
            sb.Append(" where ");
            for (int i = 0; i < formFilters.GetUpperBound(0); i++)
            {
                for (int j = 0; j < formFilters.GetUpperBound(1); j++)
                {
                    if (string.Equals(formFilters[i, j].ToLower(), "null"))
                        sb.Append(formFilters[i, j] + " is null");
                    else
                        sb.Append(formFilters[i, j] + " = @" + formFilters[i, j]);
                    while (i < formFilters.GetUpperBound(0) - 1)
                    {
                        sb.Append(" and ");
                    }
                }
            }
            string cmdString = sb.ToString();
            log.WriteLogEntry("Update command string \n" + cmdString);
            log.WriteLogEntry("Forms updated " + result);
            log.WriteLogEntry("End UpdateForm.");
            return result;
        }
    }
}