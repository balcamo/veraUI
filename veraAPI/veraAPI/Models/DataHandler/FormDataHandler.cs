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


namespace VeraAPI.Models.DataHandler
{
    public class FormDataHandler : SQLDataHandler
    {
        public BaseForm FormData { get; private set; }
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
            this.FormData = new BaseForm();
        }

        public FormDataHandler(BaseForm webForm, string dbServer, string dbName) : base(dbServer)
        {
            this.log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "FormDataHandler" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            this.dbServer = dbServer;
            this.dbName = dbName;
            this.dataConnectionString = GetDataConnectionString();
            this.FormData = webForm;
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

            switch (this.FormData.TemplateID)
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
            // Returns the SQL generated travel_id from the travel table
            log.WriteLogEntry("Starting InsertTravelAuth...");
            bool result = false;
            TravelAuthForm Travel = (TravelAuthForm)FormData;
            if (Travel.String7 != null)
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
                userEmail = Travel.String7;
                log.WriteLogEntry("Travel Auth user email set to submitter signature " + userEmail);

                // Attempt data typing of required form fields prior to SQL call
                // Set the result to false and return in the catch and do not insert the form data
                // Verbose logging to assist debugging
                try
                {
                    log.WriteLogEntry("Try conversion of data form fields to correct types.");
                    log.WriteLogEntry("TravelBegin: " + Travel.Date1);
                    departDate = DateTime.Parse(Travel.Date1);
                    log.WriteLogEntry("TravelEnd: " + Travel.Date2);
                    returnDate = DateTime.Parse(Travel.Date2);
                    log.WriteLogEntry("DistrictVehicle: " + Travel.Bool2);
                    districtVehicle = Travel.Bool2 == "true" ? true : false;
                    log.WriteLogEntry("RegistrationCost: " + Travel.Decimal2);
                    registrationAmt = decimal.Parse(Travel.Decimal2);
                    log.WriteLogEntry("Airfare: " + Travel.Decimal3);
                    airfareAmt = decimal.Parse(Travel.Decimal3);
                    log.WriteLogEntry("RentalCar: " + Travel.Decimal4);
                    rentalAmt = decimal.Parse(Travel.Decimal4);
                    log.WriteLogEntry("FuelParking: " + Travel.Decimal5);
                    fuelParkingAmt = decimal.Parse(Travel.Decimal5);
                    log.WriteLogEntry("Mileage: " + Travel.Decimal7);
                    estimatedMiles = int.Parse(Travel.Decimal7);
                    log.WriteLogEntry("Lodging: " + Travel.Decimal8);
                    lodgingAmt = decimal.Parse(Travel.Decimal8);
                    log.WriteLogEntry("PerDiem: " + Travel.Decimal9);
                    perdiemAmt = decimal.Parse(Travel.Decimal9);
                    log.WriteLogEntry("TravelDays: " + Travel.Decimal10);
                    travelDays = int.Parse(Travel.Decimal10);
                    log.WriteLogEntry("Misc: " + Travel.Decimal11);
                    miscAmt = decimal.Parse(Travel.Decimal11);
                    log.WriteLogEntry("Advance: " + Travel.Bool3);
                    requestAdvance = Travel.Bool3 == "true" ? true : false;
                    log.WriteLogEntry("AdvanceAmount: " + Travel.Decimal13);
                    advanceAmt = decimal.Parse(Travel.Decimal13);
                    log.WriteLogEntry("Policy: " + Travel.Bool4);
                    travelPolicy = Travel.Bool4 == "true" ? true : false;

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
                        cmd.Parameters.AddWithValue("@firstName", Travel.String1);
                        cmd.Parameters.AddWithValue("@lastName", Travel.String2);
                        cmd.Parameters.AddWithValue("@phone", Travel.Integer1);
                        cmd.Parameters.AddWithValue("@email", Travel.String3);
                        cmd.Parameters.AddWithValue("@eventDescription", Travel.String4);
                        cmd.Parameters.AddWithValue("@eventLocation", Travel.String5);
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
                        cmd.Parameters.AddWithValue("@submitterSig", Travel.String7);
                        try
                        {
                            // Try opening the SQL connection and executing the above constructed SQL query
                            conn.Open();
                            this.FormData.FormDataID = (int)cmd.ExecuteScalar();
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
            log.WriteLogEntry("End InsertTravelAuth.");
            return result;
        }

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
                    cmd.Parameters.AddWithValue("@templateID", FormData.TemplateID);
                    try
                    {
                        conn.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                JobTemplate template = new JobTemplate(FormData.TemplateID)
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

        public bool LoadTravelAuth(int dataID)
        {
            log.WriteLogEntry("Begin LoadTravelAuth...");
            bool result = false;
            string cmdString = string.Format(@"select * from {0}.dbo.travel where travel_id = @dataID", dbName);
            log.WriteLogEntry("SQL command string: " + cmdString);
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
                                Travel.String1 = rdr["first_name"].ToString();
                                Travel.String2 = rdr["last_name"].ToString();
                                Travel.Integer1 = rdr["phone"].ToString();
                                Travel.String3 = rdr["email"].ToString();
                                Travel.String4 = rdr["event_description"].ToString();
                                Travel.String5 = rdr["event_location"].ToString();
                                Travel.Date1 = rdr["depart_date"].ToString();
                                Travel.Date2 = rdr["return_date"].ToString();
                                Travel.Bool2 = rdr["district_vehicle"].ToString();
                                Travel.Decimal2 = rdr["registration_amt"].ToString();
                                Travel.Decimal3 = rdr["airfare_amt"].ToString();
                                Travel.Decimal4 = rdr["rental_amt"].ToString();
                                Travel.Decimal5 = rdr["fuel_parking_amt"].ToString();
                                Travel.Decimal7 = rdr["estimated_miles"].ToString();
                                Travel.Decimal8 = rdr["lodging_amt"].ToString();
                                Travel.Decimal9 = rdr["perdiem_amt"].ToString();
                                Travel.Decimal10 = rdr["travel_days"].ToString();
                                Travel.Decimal11 = rdr["misc_amt"].ToString();
                                Travel.Decimal12 = rdr["total_cost"].ToString();
                                Travel.Decimal13 = rdr["advance_amt"].ToString();
                                Travel.Bool3 = rdr["request_advance"].ToString();
                                Travel.Bool4 = rdr["travel_policy"].ToString();
                                Travel.Bool1 = rdr["preparer_name"].ToString();
                                Travel.String7 = rdr["submitter_approval"].ToString();
                                log.WriteLogEntry("Retrieved travel data " + dataID + " " + Travel.String4);
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

        public int LoadTravelAuthForms(int userID)
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