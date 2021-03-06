﻿using System;
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
        private static Scribe log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "FormDataHandler" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");

        public FormDataHandler(string dbServer, string dbName) : base(dbServer)
        {
            this.dbServer = dbServer;
            this.dbName = dbName;
            this.dataConnectionString = GetDataConnectionString();
        }

        public bool InsertTravelAuth(TravelAuthForm travelForm, string tableName)
        {
            log.WriteLogEntry("Starting InsertTravelAuth...");
            bool result = false;
            string cmdString = string.Format(@"insert into {0}.dbo.travel (first_name, last_name, phone, email, event_description, event_location, depart_date, return_date, district_vehicle, 
                                    registration_amt, airfare_amt, rental_amt, fuel_amt, parking_amt, mileage_amt, lodging_amt, perdiem_amt, travel_days, full_days, misc_amt, misc_description, total_cost_amt, 
                                    request_advance, advance_amt, advance_status, advance_date, travel_policy, submit_date, submitter_id, supervisor_id, supervisor_email, supervisor_approval_status, 
                                    supervisor_approval_date, manager_id, manager_email, manager_approval_status, manager_approval_date, approval_status, recap_status, recap_date, close_status, close_date) 
                                    output inserted.form_id
                                    values (@firstName, @lastName, @phone, @email, @eventDescription, @eventLocation, @departDate, @returnDate, @districtVehicle, @registrationAmt, @airfareAmt, 
                                    @rentalAmt, @fuelAmt, @parkingAmt, @mileageAmt, @lodgingAmt, @perdiemAmt, @travelDays, @fullDays, @miscAmt, @miscDescription, @totalAmt, @requestAdvance, @advanceAmt, @advanceStatus, 
                                    @advanceDate, @travelPolicy, @submitDate, @submitterID, @supervisorID, @supervisorEmail, @supervisorApprove, @supervisorDate, @managerID, @managerEmail, @managerApprove, 
                                    @managerDate, @status, @recapStatus, @recapDate, @closeStatus, @closeDate)", dbName);
            DateTime departDate = DateTime.MinValue, returnDate = DateTime.MinValue;
            bool districtVehicle = false, requestAdvance = false, travelPolicy = false;
            decimal registrationAmt = 0, airfareAmt = 0, rentalAmt = 0, fuelAmt = 0, parkingAmt = 0, mileageAmt = 0, lodgingAmt = 0, perdiemAmt = 0, miscAmt = 0,
                totalAmt = 0, advanceAmt = 0;
            int travelDays = 0, fullDays = 0;

            try
            {
                log.WriteLogEntry("Travel Begin: " + travelForm.TravelBegin);
                departDate = DateTime.Parse(travelForm.TravelBegin);
                log.WriteLogEntry("Travel End: " + travelForm.TravelEnd);
                returnDate = DateTime.Parse(travelForm.TravelEnd);
                log.WriteLogEntry("District Vehicle: " + travelForm.DistVehicle);
                districtVehicle = travelForm.DistVehicle.ToLower() == "true" ? true : false;
                log.WriteLogEntry("Registration Cost: " + travelForm.RegistrationCost);
                registrationAmt = decimal.Parse(travelForm.RegistrationCost);
                log.WriteLogEntry("Airfare: " + travelForm.Airfare);
                airfareAmt = decimal.Parse(travelForm.Airfare);
                log.WriteLogEntry("RentalCar: " + travelForm.RentalCar);
                rentalAmt = decimal.Parse(travelForm.RentalCar);
                log.WriteLogEntry("Fuel: " + travelForm.Fuel);
                fuelAmt = decimal.Parse(travelForm.Fuel);
                log.WriteLogEntry("Parking: " + travelForm.ParkingTolls);
                parkingAmt = decimal.Parse(travelForm.ParkingTolls);
                log.WriteLogEntry("Mileage: " + travelForm.Mileage);
                mileageAmt = decimal.Parse(travelForm.Mileage);
                log.WriteLogEntry("Lodging: " + travelForm.Lodging);
                lodgingAmt = decimal.Parse(travelForm.Lodging);
                log.WriteLogEntry("PerDiem: " + travelForm.PerDiem);
                perdiemAmt = decimal.Parse(travelForm.PerDiem);
                log.WriteLogEntry("Travel Days: " + travelForm.TravelDays);
                travelDays = int.Parse(travelForm.TravelDays);
                log.WriteLogEntry("Full Days: " + travelForm.FullDays);
                fullDays = int.Parse(travelForm.FullDays);
                log.WriteLogEntry("Misc: " + travelForm.Misc);
                miscAmt = decimal.Parse(travelForm.Misc);
                log.WriteLogEntry("Total Cost: " + travelForm.TotalEstimate);
                totalAmt = decimal.Parse(travelForm.TotalEstimate);
                log.WriteLogEntry("Advance: " + travelForm.Advance);
                requestAdvance = travelForm.Advance.ToLower() == "true" ? true : false;
                log.WriteLogEntry("Advance Amount: " + travelForm.AdvanceAmount);
                advanceAmt = decimal.Parse(travelForm.AdvanceAmount);
                log.WriteLogEntry("Policy: " + travelForm.Policy);
                travelPolicy = travelForm.Policy.ToLower() == "true" ? true : false;
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
                    cmd.Parameters.AddWithValue("@registrationAmt", registrationAmt);
                    cmd.Parameters.AddWithValue("@airfareAmt", airfareAmt);
                    cmd.Parameters.AddWithValue("@rentalAmt", rentalAmt);
                    cmd.Parameters.AddWithValue("@fuelAmt", fuelAmt);
                    cmd.Parameters.AddWithValue("@parkingAmt", parkingAmt);
                    cmd.Parameters.AddWithValue("@mileageAmt", mileageAmt);
                    cmd.Parameters.AddWithValue("@lodgingAmt", lodgingAmt);
                    cmd.Parameters.AddWithValue("@perdiemAmt", perdiemAmt);
                    cmd.Parameters.AddWithValue("@travelDays", travelDays);
                    cmd.Parameters.AddWithValue("@fullDays", fullDays);
                    cmd.Parameters.AddWithValue("@miscAmt", miscAmt);
                    cmd.Parameters.AddWithValue("@miscDescription", travelForm.MiscExplain);
                    cmd.Parameters.AddWithValue("@totalAmt", totalAmt);
                    cmd.Parameters.AddWithValue("@requestAdvance", requestAdvance);
                    cmd.Parameters.AddWithValue("@advanceAmt", advanceAmt);
                    cmd.Parameters.AddWithValue("@travelPolicy", travelPolicy);
                    cmd.Parameters.AddWithValue("@submitterID", travelForm.UserID);
                    cmd.Parameters.AddWithValue("@supervisorID", travelForm.DHID);
                    cmd.Parameters.AddWithValue("@supervisorEmail", travelForm.DHEmail);
                    cmd.Parameters.AddWithValue("@supervisorApprove", travelForm.DHApproval);
                    cmd.Parameters.AddWithValue("@supervisorDate", travelForm.DHApprovalDate);
                    cmd.Parameters.AddWithValue("@managerID", travelForm.GMID);
                    cmd.Parameters.AddWithValue("@managerEmail", travelForm.GMEmail);
                    cmd.Parameters.AddWithValue("@managerApprove", travelForm.GMApproval);
                    cmd.Parameters.AddWithValue("@managerDate", travelForm.GMApprovalDate);
                    cmd.Parameters.AddWithValue("@status", travelForm.ApprovalStatus);
                    cmd.Parameters.AddWithValue("@advanceStatus", travelForm.AdvanceStatus);
                    cmd.Parameters.AddWithValue("@advanceDate", travelForm.AdvanceDate);
                    cmd.Parameters.AddWithValue("@recapStatus", travelForm.RecapStatus);
                    cmd.Parameters.AddWithValue("@recapDate", travelForm.RecapDate);
                    cmd.Parameters.AddWithValue("@submitDate", travelForm.SubmitDate);
                    cmd.Parameters.AddWithValue("@closeStatus", travelForm.CloseStatus);
                    cmd.Parameters.AddWithValue("@closeDate", travelForm.CloseDate);
                    
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

        public bool LoadFormTemplate(FormTemplate formTemplate, int templateID)
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
                                formTemplate = new FormTemplate()
                                {
                                    TemplateID = (int)rdr["template_id"],
                                    TemplateName = rdr["template_name"].ToString(),
                                    TableName = rdr["table_name"].ToString(),
                                    JobDescription = rdr["job_description"].ToString(),
                                    JobWeight = (int)rdr["job_weight"],
                                    JobPriority = (int)rdr["job_priority"],
                                    JobType = rdr["job_type"].ToString()
                                };
                                log.WriteLogEntry("Retrieved form template " + formTemplate.TemplateID + " " + formTemplate.TemplateName);
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

        public bool LoadTravelAuthForm(TravelAuthForm travelForm, int formDataID)
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
                                travelForm = new TravelAuthForm
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
                                    Fuel = rdr["fuel_amt"].ToString(),
                                    ParkingTolls = rdr["parking_amt"].ToString(),
                                    Mileage = rdr["mileage_amt"].ToString(),
                                    Lodging = rdr["lodging_amt"].ToString(),
                                    PerDiem = rdr["perdiem_amt"].ToString(),
                                    FullDays = rdr["full_days"].ToString(),
                                    TravelDays = rdr["travel_days"].ToString(),
                                    Misc = rdr["misc_amt"].ToString(),
                                    TotalEstimate = rdr["total_cost_amt"].ToString(),
                                    AdvanceAmount = rdr["advance_amt"].ToString(),
                                    Advance = rdr["request_advance"].ToString(),
                                    Policy = rdr["travel_policy"].ToString(),
                                    Preparer = rdr["preparer_name"].ToString(),
                                    SubmitterSig = rdr["submitter_email"].ToString(),
                                    DHID = rdr["supervisor_id"].ToString(),
                                    DHApproval = rdr["supervisor_approval_status"].ToString(),
                                    GMID = rdr["manager_id"].ToString(),
                                    GMApproval = rdr["manager_approval_status"].ToString(),
                                    ApprovalStatus = rdr["approval_status"].ToString()
                                };
                                log.WriteLogEntry("Retrieved travel data " + travelForm.FormDataID + " " + travelForm.EventTitle);
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

        public int LoadTravelForms(List<BaseForm> travelForms, string cmdString)
        {
            log.WriteLogEntry("Begin LoadTravelForms...");
            int result = 0;

            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
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
                                    Fuel = rdr["fuel_amt"].ToString(),
                                    ParkingTolls = rdr["parking_amt"].ToString(),
                                    Mileage = rdr["mileage_amt"].ToString(),
                                    Lodging = rdr["lodging_amt"].ToString(),
                                    PerDiem = rdr["perdiem_amt"].ToString(),
                                    FullDays = rdr["full_days"].ToString(),
                                    TravelDays = rdr["travel_days"].ToString(),
                                    Misc = rdr["misc_amt"].ToString(),
                                    MiscExplain = rdr["misc_description"].ToString(),
                                    TotalEstimate = rdr["total_cost_amt"].ToString(),
                                    AdvanceAmount = rdr["advance_amt"].ToString(),
                                    Advance = rdr["request_advance"].ToString(),
                                    Policy = rdr["travel_policy"].ToString(),
                                    Preparer = rdr["preparer_name"].ToString(),
                                    SubmitterSig = rdr["submitter_email"].ToString(),
                                    DHID = rdr["supervisor_id"].ToString(),
                                    DHApproval = rdr["supervisor_approval_status"].ToString(),
                                    GMID = rdr["manager_id"].ToString(),
                                    GMApproval = rdr["manager_approval_status"].ToString(),
                                    ApprovalStatus = rdr["approval_status"].ToString(),
                                    ApprovalDate = rdr["approval_date"].ToString(),
                                    AdvanceStatus = rdr["advance_status"].ToString(),
                                    AdvanceDate = rdr["advance_date"].ToString(),
                                    RecapRegistrationCost = rdr["recap_registration_amt"].ToString(),
                                    RecapAirfare = rdr["recap_airfare_amt"].ToString(),
                                    RecapRentalCar = rdr["recap_rental_amt"].ToString(),
                                    RecapFuel = rdr["recap_fuel_amt"].ToString(),
                                    RecapParkingTolls = rdr["recap_parking_amt"].ToString(),
                                    RecapMileage = rdr["recap_miles"].ToString(),
                                    RecapMileageAmount = rdr["recap_mileage_amt"].ToString(),
                                    RecapLodging = rdr["recap_lodging_amt"].ToString(),
                                    RecapPerDiem = rdr["recap_perdiem_amt"].ToString(),
                                    RecapTravelDays = rdr["recap_travel_days"].ToString(),
                                    RecapFullDays = rdr["recap_full_days"].ToString(),
                                    RecapMisc = rdr["recap_misc_amt"].ToString(),
                                    TotalRecap = rdr["recap_total_amt"].ToString(),
                                    TotalReimburse = rdr["reimburse_amt"].ToString(),
                                    RecapStatus = rdr["recap_status"].ToString(),
                                    RecapDate = rdr["recap_date"].ToString()
                                };
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
            result = travelForms.Count;
            log.WriteLogEntry("End LoadTravelForms.");
            return result;
        }

        public int LoadTravelForms(List<BaseForm> travelForms, string[] formFields, string[,] formFilters)
        {
            log.WriteLogEntry("Begin LoadTravelForms...");
            int result = 0;
            StringBuilder sbCommand = new StringBuilder("select ");
            for (int i = 0; i < formFields.GetLength(0); i++)
            {
                string fieldName = formFields[i];
                sbCommand.Append(fieldName);
                if (i < formFields.GetLength(0) - 1)
                {
                    sbCommand.Append(", ");
                }
            }
            sbCommand.Append(string.Format(" from {0}.dbo.travel where ", dbServer));
            for (int i = 0; i < formFilters.GetLength(0); i++)
            {
                sbCommand.Append(formFilters[i, 0]);
                if (string.Equals(formFilters[i, 1].ToLower(), "null"))
                    sbCommand.Append(" is null");
                else
                    sbCommand.Append(string.Format(" {0} @filter{1}", formFilters[i,2], i));
                if (i < formFilters.GetLength(0) - 1)
                {
                    sbCommand.Append(string.Format(" {0} ", formFilters[i+1,3]));
                }
            }
            string cmdString = sbCommand.ToString();
            log.WriteLogEntry("SQL Command string:\n" + cmdString);

            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    try
                    {
                        for (int i = 0; i < formFields.GetLength(0); i++)
                        {
                            string parmName = "@field" + i;
                            string parmValue = formFields[i];
                            log.WriteLogEntry("Field Name: " + parmName + "\tValue: " + parmValue);
                            cmd.Parameters.AddWithValue(parmName, parmValue);
                        }
                        for (int i = 0; i < formFilters.GetLength(0); i++)
                        {
                            string parmName = "@filter" + i;
                            string parmValue = formFilters[i, 1];
                            log.WriteLogEntry("Filter Name: " + parmName + "\tValue: " + parmValue);
                            if (!string.Equals(parmValue.ToLower(), "null"))
                                cmd.Parameters.AddWithValue(parmName, parmValue);
                        }
                        log.WriteLogEntry("SQL Command Parameters:");
                        foreach (SqlParameter parm in cmd.Parameters)
                        {
                            log.WriteLogEntry("Name: " + parm.ParameterName + "\tValue: " + parm.SqlValue);
                        }
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
                                    Fuel = rdr["fuel_amt"].ToString(),
                                    ParkingTolls = rdr["parking_amt"].ToString(),
                                    Mileage = rdr["mileage_amt"].ToString(),
                                    Lodging = rdr["lodging_amt"].ToString(),
                                    PerDiem = rdr["perdiem_amt"].ToString(),
                                    FullDays = rdr["full_days"].ToString(),
                                    TravelDays = rdr["travel_days"].ToString(),
                                    Misc = rdr["misc_amt"].ToString(),
                                    MiscExplain = rdr["misc_description"].ToString(),
                                    TotalEstimate = rdr["total_cost_amt"].ToString(),
                                    AdvanceAmount = rdr["advance_amt"].ToString(),
                                    Advance = rdr["request_advance"].ToString(),
                                    Policy = rdr["travel_policy"].ToString(),
                                    Preparer = rdr["preparer_name"].ToString(),
                                    SubmitterSig = rdr["submitter_email"].ToString(),
                                    DHID = rdr["supervisor_id"].ToString(),
                                    DHApproval = rdr["supervisor_approval_status"].ToString(),
                                    GMID = rdr["manager_id"].ToString(),
                                    GMApproval = rdr["manager_approval_status"].ToString(),
                                    ApprovalStatus = rdr["approval_status"].ToString(),
                                    ApprovalDate = rdr["approval_date"].ToString(),
                                    AdvanceStatus = rdr["advance_status"].ToString(),
                                    AdvanceDate = rdr["advance_date"].ToString(),
                                    RecapRegistrationCost = rdr["recap_registration_amt"].ToString(),
                                    RecapAirfare = rdr["recap_airfare_amt"].ToString(),
                                    RecapRentalCar = rdr["recap_rental_amt"].ToString(),
                                    RecapFuel = rdr["recap_fuel_amt"].ToString(),
                                    RecapParkingTolls = rdr["recap_parking_amt"].ToString(),
                                    RecapMileage = rdr["recap_miles"].ToString(),
                                    RecapMileageAmount = rdr["recap_mileage_amt"].ToString(),
                                    RecapLodging = rdr["recap_lodging_amt"].ToString(),
                                    RecapPerDiem = rdr["recap_perdiem_amt"].ToString(),
                                    RecapTravelDays = rdr["recap_travel_days"].ToString(),
                                    RecapFullDays = rdr["recap_full_days"].ToString(),
                                    RecapMisc = rdr["recap_misc_amt"].ToString(),
                                    TotalRecap = rdr["recap_total_amt"].ToString(),
                                    TotalReimburse = rdr["reimburse_amt"].ToString(),
                                    RecapStatus = rdr["recap_status"].ToString(),
                                    RecapDate = rdr["recap_date"].ToString()
                                };
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
            result = travelForms.Count;
            log.WriteLogEntry("End LoadTravelForms.");
            return result;
        }

        public int LoadApproverTravelAuthForms(List<BaseForm> travelForms, int userID)
        {
            log.WriteLogEntry("Begin LoadApproverTravelAuthForms...");
            log.WriteLogEntry("User ID" + userID);
            int result = 0;

            string cmdString = string.Format(@"select * from {0}.dbo.travel where (supervisor_id = @userID or manager_id = @userID) and approval_status = @status", dbName);
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    cmd.Parameters.AddWithValue("@userID", userID);
                    cmd.Parameters.AddWithValue("@status", Constants.PendingValue);
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
                                    Fuel = rdr["fuel_amt"].ToString(),
                                    ParkingTolls = rdr["parking_amt"].ToString(),
                                    Mileage = rdr["mileage_amt"].ToString(),
                                    Lodging = rdr["lodging_amt"].ToString(),
                                    PerDiem = rdr["perdiem_amt"].ToString(),
                                    FullDays = rdr["full_days"].ToString(),
                                    TravelDays = rdr["travel_days"].ToString(),
                                    Misc = rdr["misc_amt"].ToString(),
                                    MiscExplain = rdr["misc_description"].ToString(),
                                    TotalEstimate = rdr["total_cost_amt"].ToString(),
                                    AdvanceAmount = rdr["advance_amt"].ToString(),
                                    Advance = rdr["request_advance"].ToString(),
                                    Policy = rdr["travel_policy"].ToString(),
                                    Preparer = rdr["preparer_name"].ToString(),
                                    SubmitterSig = rdr["submitter_email"].ToString(),
                                    DHID = rdr["supervisor_id"].ToString(),
                                    DHApproval = rdr["supervisor_approval_status"].ToString(),
                                    GMID = rdr["manager_id"].ToString(),
                                    GMApproval = rdr["manager_approval_status"].ToString(),
                                    ApprovalStatus = rdr["approval_status"].ToString()
                                };
                                log.WriteLogEntry(string.Format("Retrieved travel data {0} {1} {2} {3} {4} {5}", travel.FormDataID, travel.UserID, travel.EventTitle, travel.ApprovalStatus, travel.DHApproval, travel.GMApproval));
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
            result = travelForms.Count;
            log.WriteLogEntry("End LoadApproverTravelAuthForms.");
            return result;
        }

        public int UpdateTravelForm(string[,] formFields, string[,] formFilters)
        {
            log.WriteLogEntry("Begin UpdateForm...");
            int result = 0;
            StringBuilder sbCommand = new StringBuilder(string.Format("update {0}.dbo.travel set ", dbServer));
            for (int i = 0; i < formFields.GetLength(0); i++)
            {
                string fieldName = formFields[i, 0];
                sbCommand.Append(fieldName + " = @field" + i);
                if (i < formFields.GetLength(0) - 1)
                {
                    sbCommand.Append(", ");
                }
            }
            sbCommand.Append(" where ");
            for (int i = 0; i < formFilters.GetLength(0); i++)
            {
                string fieldName = formFilters[i, 0];
                if (string.Equals(formFilters[i, 1].ToLower(), "null"))
                    sbCommand.Append(fieldName + " is null");
                else
                    sbCommand.Append(fieldName + " = @filter" + i);
                if (i < formFilters.GetLength(0) - 1)
                {
                    sbCommand.Append(" and ");
                }
            }
            string cmdString = sbCommand.ToString();
            log.WriteLogEntry("Update command string:\n" + cmdString);

            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    try
                    {
                        for (int i = 0; i < formFields.GetLength(0); i++)
                        {
                            string parmName = "@field" + i;
                            string parmValue = formFields[i, 1];
                            log.WriteLogEntry("Field Name: " + parmName + "\tValue: " + parmValue);
                            cmd.Parameters.AddWithValue(parmName, parmValue);
                        }
                        for (int i = 0; i < formFilters.GetLength(0); i++)
                        {
                            string parmName = "@filter" + i;
                            string parmValue = formFilters[i, 1];
                            log.WriteLogEntry("Filter Name: " + parmName + "\tValue: " + parmValue);
                            if (!string.Equals(parmValue.ToLower(), "null"))
                                cmd.Parameters.AddWithValue(parmName, parmValue);
                        }
                        log.WriteLogEntry("SQL Command Parameters:");
                        foreach (SqlParameter parm in cmd.Parameters)
                        {
                            log.WriteLogEntry("Name: " + parm.ParameterName + "\tValue: " + parm.SqlValue);
                        }
                        conn.Open();
                        result = cmd.ExecuteNonQuery();
                        log.WriteLogEntry("Updated row count " + result);
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
            log.WriteLogEntry("End UpdateForm.");
            return result;
        }
    }
}