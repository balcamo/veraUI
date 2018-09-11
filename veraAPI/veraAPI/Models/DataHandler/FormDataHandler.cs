using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
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
        public JobTemplate Template { get; set; }
        public string userEmail { get; set; }

        private Scribe log;

        public FormDataHandler(string dbServer, string dbName) : base(dbServer)
        {
            this.log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "FormDataHandler" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            this.dbServer = dbServer;
            this.dbName = dbName;
            this.dataConnectionString = GetDataConnectionString();
            this.WebForm = new BaseForm();
        }

        public FormDataHandler(BaseForm webForm, string dbServer, string dbName) : base(dbServer)
        {
            this.log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "FormDataHandler" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            this.dbServer = dbServer;
            this.dbName = dbName;
            this.dataConnectionString = GetDataConnectionString();
            this.WebForm = webForm;
        }

        public FormDataHandler(List<BaseForm> webForms, string dbServer, string dbName) : base(dbServer)
        {
            this.log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "FormDataHandler" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            this.dbServer = dbServer;
            this.dbName = dbName;
            this.dataConnectionString = GetDataConnectionString();
            this.WebForms = webForms;
        }

        public bool InsertFormData()
        {
            bool result = false;

            switch (this.WebForm.TemplateID)
            {
                case 1:
                    result = InsertTravelAuth();
                    break;
                default:
                    break;
            }
            return result;
        }

        private bool InsertTravelAuth()
        {
            // DOES THIS INSERT THE USER ID NUMBER????

            // Returns the SQL generated travel_id from the travel table
            log.WriteLogEntry("Starting InsertTravelAuth...");
            bool result = false;
            if (WebForm.GetType() == typeof(TravelAuthForm))
            {
                TravelAuthForm travel = (TravelAuthForm)WebForm;
                if (travel.String7 != null)
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
                    userEmail = travel.String7;
                    log.WriteLogEntry("travel Auth user email set to submitter signature " + userEmail);

                    // Attempt data typing of required form fields prior to SQL call
                    // Set the result to false and return in the catch and do not insert the form data
                    // Verbose logging to assist debugging
                    try
                    {
                        log.WriteLogEntry("Try conversion of data form fields to correct types.");
                        log.WriteLogEntry("TravelBegin: " + travel.Date1);
                        departDate = DateTime.Parse(travel.Date1);
                        log.WriteLogEntry("TravelEnd: " + travel.Date2);
                        returnDate = DateTime.Parse(travel.Date2);
                        log.WriteLogEntry("DistrictVehicle: " + travel.Bool2);
                        districtVehicle = travel.Bool2 == "true" ? true : false;
                        log.WriteLogEntry("RegistrationCost: " + travel.Decimal2);
                        registrationAmt = decimal.Parse(travel.Decimal2);
                        log.WriteLogEntry("Airfare: " + travel.Decimal3);
                        airfareAmt = decimal.Parse(travel.Decimal3);
                        log.WriteLogEntry("RentalCar: " + travel.Decimal4);
                        rentalAmt = decimal.Parse(travel.Decimal4);
                        log.WriteLogEntry("FuelParking: " + travel.Decimal5);
                        fuelParkingAmt = decimal.Parse(travel.Decimal5);
                        log.WriteLogEntry("Mileage: " + travel.Decimal7);
                        estimatedMiles = int.Parse(travel.Decimal7);
                        log.WriteLogEntry("Lodging: " + travel.Decimal8);
                        lodgingAmt = decimal.Parse(travel.Decimal8);
                        log.WriteLogEntry("PerDiem: " + travel.Decimal9);
                        perdiemAmt = decimal.Parse(travel.Decimal9);
                        log.WriteLogEntry("TravelDays: " + travel.Decimal10);
                        travelDays = int.Parse(travel.Decimal10);
                        log.WriteLogEntry("Misc: " + travel.Decimal11);
                        miscAmt = decimal.Parse(travel.Decimal11);
                        log.WriteLogEntry("Advance: " + travel.Bool3);
                        requestAdvance = travel.Bool3 == "true" ? true : false;
                        log.WriteLogEntry("AdvanceAmount: " + travel.Decimal13);
                        advanceAmt = decimal.Parse(travel.Decimal13);
                        log.WriteLogEntry("Policy: " + travel.Bool4);
                        travelPolicy = travel.Bool4 == "true" ? true : false;

                    }
                    catch (Exception ex)
                    {
                        result = false;
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
                            cmd.Parameters.AddWithValue("@firstName", travel.String1);
                            cmd.Parameters.AddWithValue("@lastName", travel.String2);
                            cmd.Parameters.AddWithValue("@phone", travel.Integer1);
                            cmd.Parameters.AddWithValue("@email", travel.String3);
                            cmd.Parameters.AddWithValue("@eventDescription", travel.String4);
                            cmd.Parameters.AddWithValue("@eventLocation", travel.String5);
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
                            cmd.Parameters.AddWithValue("@submitterSig", travel.String7);
                            try
                            {
                                // Try opening the SQL connection and executing the above constructed SQL query
                                conn.Open();
                                travel.FormDataID = (int)cmd.ExecuteScalar();
                                result = true;
                                log.WriteLogEntry("Successful insert travel ID " + result);
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
                else
                    log.WriteLogEntry("Missing submitter signature!");
            }
            log.WriteLogEntry("End InsertTravelAuth.");
            return result;
        }

        // WHAT IS THIS FUNCTION FOR???
        public bool LoadFormTemplate()
        {
            log.WriteLogEntry("Begin LoadFormTemplate...");
            bool result = false;
            string cmdString = string.Format(@"select * from {0}.dbo.job_template where template_id = @templateID", dbName);
            log.WriteLogEntry("SQL command string: " + cmdString);
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    cmd.Parameters.AddWithValue("@templateID", WebForm.TemplateID);
                    try
                    {
                        conn.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                JobTemplate template = new JobTemplate(WebForm.TemplateID)
                                {
                                    TemplateName = rdr["template_name"].ToString(),
                                    TableName = rdr["table_name"].ToString(),
                                    JobDescription = rdr["job_description"].ToString(),
                                    JobWeight = (int)rdr["job_weight"],
                                    JobPriority = (int)rdr["job_priority"],
                                    JobType = rdr["job_type"].ToString()
                                };
                                log.WriteLogEntry("Retrieved form template " + template.TemplateID + " " + template.TemplateName);
                                Template = template;
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

        public bool LoadTravelAuth()
        {
            log.WriteLogEntry("Begin LoadTravelAuth...");
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
                                TravelAuthForm travel = new TravelAuthForm(WebForm.FormDataID);
                                travel.String1 = rdr["first_name"].ToString();
                                travel.String2 = rdr["last_name"].ToString();
                                travel.Integer1 = rdr["phone"].ToString();
                                travel.String3 = rdr["email"].ToString();
                                travel.String4 = rdr["event_description"].ToString();
                                travel.String5 = rdr["event_location"].ToString();
                                travel.Date1 = rdr["depart_date"].ToString();
                                travel.Date2 = rdr["return_date"].ToString();
                                travel.Bool2 = rdr["district_vehicle"].ToString();
                                travel.Decimal2 = rdr["registration_amt"].ToString();
                                travel.Decimal3 = rdr["airfare_amt"].ToString();
                                travel.Decimal4 = rdr["rental_amt"].ToString();
                                travel.Decimal5 = rdr["fuel_parking_amt"].ToString();
                                travel.Decimal7 = rdr["estimated_miles"].ToString();
                                travel.Decimal8 = rdr["lodging_amt"].ToString();
                                travel.Decimal9 = rdr["perdiem_amt"].ToString();
                                travel.Decimal10 = rdr["travel_days"].ToString();
                                travel.Decimal11 = rdr["misc_amt"].ToString();
                                travel.Decimal12 = rdr["total_cost"].ToString();
                                travel.Decimal13 = rdr["advance_amt"].ToString();
                                travel.Bool3 = rdr["request_advance"].ToString();
                                travel.Bool4 = rdr["travel_policy"].ToString();
                                travel.Bool1 = rdr["preparer_name"].ToString();
                                travel.String7 = rdr["submitter_approval"].ToString();
                                log.WriteLogEntry("Retrieved travel data " + travel.FormDataID + " " + travel.String4);
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
            log.WriteLogEntry("End LoadTravelAuth.");
            return result;
        }

        public int LoadTravelAuthForms(string userID)
        {
            log.WriteLogEntry("Begin LoadTravelAuth...");
            int result = 0;
            List<TravelAuthForm> travelForms = new List<TravelAuthForm>();
            string cmdString = string.Format(@"select * from {0}.dbo.travel where submitter_id = @userID", dbName);
            log.WriteLogEntry("SQL command string: " + cmdString);
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
                                // SINCE WE CHANGED VARIABLE NAMES DO WE NEED TO CHANGE THESE???
                                TravelAuthForm travel = new TravelAuthForm();
                                travel.FormDataID = (int)rdr["travel_id"];
                                travel.String1 = rdr["first_name"].ToString();
                                travel.String2 = rdr["last_name"].ToString();
                                travel.Integer1 = rdr["phone"].ToString();
                                travel.String3 = rdr["email"].ToString();
                                travel.String4 = rdr["event_description"].ToString();
                                travel.String5 = rdr["event_location"].ToString();
                                travel.Date1 = rdr["depart_date"].ToString();
                                travel.Date2 = rdr["return_date"].ToString();
                                travel.Bool2 = rdr["district_vehicle"].ToString();
                                travel.Decimal2 = rdr["registration_amt"].ToString();
                                travel.Decimal3 = rdr["airfare_amt"].ToString();
                                travel.Decimal4 = rdr["rental_amt"].ToString();
                                travel.Decimal5 = rdr["fuel_parking_amt"].ToString();
                                travel.Decimal7 = rdr["estimated_miles"].ToString();
                                travel.Decimal8 = rdr["lodging_amt"].ToString();
                                travel.Decimal9 = rdr["perdiem_amt"].ToString();
                                travel.Decimal10 = rdr["travel_days"].ToString();
                                travel.Decimal11 = rdr["misc_amt"].ToString();
                                travel.Decimal12 = rdr["total_cost"].ToString();
                                travel.Decimal13 = rdr["advance_amt"].ToString();
                                travel.Bool3 = rdr["request_advance"].ToString();
                                travel.Bool4 = rdr["travel_policy"].ToString();
                                travel.Bool1 = rdr["preparer_name"].ToString();
                                travel.String7 = rdr["submitter_approval"].ToString();
                                log.WriteLogEntry(string.Format("Retrieved travel data {0} {1} {2}", travel.FormDataID, travel.String4, travel.String7));
                                WebForms.Add(travel);
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
            result = WebForms.Count;
            log.WriteLogEntry("End LoadTravelAuth.");
            return result;
        }
    }
}