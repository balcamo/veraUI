using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Text;
using VeraAPI.Models;
using VeraAPI.Models.Forms;
using VeraAPI.Models.Templates;
using VeraAPI.Models.DataHandler;
using VeraAPI.Models.Tools;
using VeraAPI.Models.OfficeHandler;
using VeraAPI.Models.Security;

namespace VeraAPI.HelperClasses
{
    public class FormHelper
    {
        public BaseForm WebForm { get; private set; }
        public List<BaseForm> WebForms { get; private set; }

        private readonly string dbServer = WebConfigurationManager.AppSettings.Get("DBServer");
        private readonly string dbName = WebConfigurationManager.AppSettings.Get("DBName");
        private static Scribe log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "FormHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");

        /**
        * 
        * SubmitForm will insert a new record into the database 
        *      it will also send an email to the appropriate parties
        * @param FormData : this is the form that needs to be inserted
        * 
        **/
        public bool SubmitTravelAuthForm(User user, BaseForm webForm)
        {
            log.WriteLogEntry("Begin SubmitTravelAuthForm...");
            bool result = false;
            if (webForm.GetType() == typeof(TravelAuthForm))
            {
                TravelAuthForm travelForm = (TravelAuthForm)webForm;
                FormTemplate formTemplate = new FormTemplate();
                if (user.GetType() == typeof(DomainUser))
                {
                    DomainUser domainUser = (DomainUser)user;
                    travelForm.UserID = domainUser.UserID;
                    travelForm.DHID = domainUser.Department.DeptHeadUserID.ToString();
                    travelForm.DHEmail = domainUser.Department.DeptHeadEmail;
                    travelForm.DHApproval = Constants.PendingValue.ToString();
                    travelForm.DHApprovalDate = DateTime.Now.ToString();
                    travelForm.GMID = domainUser.Company.GeneralManagerUserID.ToString();
                    travelForm.GMEmail = domainUser.Company.GeneralManagerEmail;
                    travelForm.GMApproval = Constants.PendingValue.ToString();
                    travelForm.GMApprovalDate = DateTime.Now.ToString();
                    travelForm.ApprovalStatus = Constants.PendingValue.ToString();
                    travelForm.SubmitDate = DateTime.Now.ToString();
                    if (travelForm.Advance.ToLower() == "true")
                        travelForm.AdvanceStatus = Constants.PendingValue.ToString();
                    else
                        travelForm.AdvanceStatus = Constants.ApprovedValue.ToString();
                    travelForm.AdvanceDate = DateTime.Now.ToString();
                    travelForm.RecapStatus = Constants.NotStartedValue.ToString();
                    travelForm.RecapDate = DateTime.Now.ToString();

                    // Load the job template corresponding to the templateID for the submitted form
                    log.WriteLogEntry("Starting FormDataHandler...");
                    FormDataHandler formDataHandle = new FormDataHandler(dbServer, dbName);
                    if (formDataHandle.LoadFormTemplate(formTemplate, webForm.TemplateID))
                    {
                        // Insert travel form data into the database
                        if (formDataHandle.InsertTravelAuth(travelForm, formTemplate.TableName))
                        {
                            result = true;
                        }
                        else
                            log.WriteLogEntry("FAILED insert form data to database.");
                    }
                    else
                        log.WriteLogEntry("FAILED loading form template!");
                }
            }
            else
                log.WriteLogEntry("FAILED submitted form is not a travel authorization form!");
            log.WriteLogEntry("End SubmitTravelAuthForm.");
            return result;
        }

        public bool UpdateTravelAdvanceAmount(int userID, BaseForm webForm)
        {
            log.WriteLogEntry("Begin UpdateTravelAuthForm...");
            bool result = false;
            string[,] formFields = new string[0, 0];
            string[,] formFilters = new string[0, 0];

            if (webForm.GetType() == typeof(TravelAuthForm))
            {
                TravelAuthForm travelForm = (TravelAuthForm)webForm;

                // Build field and filter list to update SQL
                try
                {
                    formFields = new string[,] {
                            { "advance_amt", travelForm.AdvanceAmount.ToString() },
                            { "advance_status", Constants.PendingValue.ToString() },
                            { "advance_date", DateTime.Now.ToString() },
                            { "close_status", Constants.PendingValue.ToString() },
                            { "close_date", DateTime.Now.ToString() }
                        };
                    formFilters = new string[,] {
                            { "submitter_id", userID.ToString() },
                            { "form_id", travelForm.FormDataID.ToString() }
                        };
                }
                catch (Exception ex)
                {
                    log.WriteLogEntry("ERROR building SQL field and filter arrays!\n" + ex.Message);
                    return result;
                }

                log.WriteLogEntry("Starting FormDataHandler...");
                FormDataHandler formData = new FormDataHandler(dbServer, dbName);
                if (formData.UpdateTravelForm(formFields, formFilters) > 0)
                    result = true;
                else
                    log.WriteLogEntry("FAILED No records updated!");
            }
            else
                log.WriteLogEntry("FAILED submitted form is not a travel authorization form!");
            log.WriteLogEntry("End UpdateTravelAuthForm.");
            return result;
        }

        public bool SubmitTravelRecapForm(int userID, BaseForm webForm)
        {
            log.WriteLogEntry("Begin SubmitTravelRecapForm...");
            bool result = false;

            if (webForm.GetType() == typeof(TravelAuthForm))
            {
                TravelAuthForm travelForm = (TravelAuthForm)webForm;
                log.WriteLogEntry(string.Format("User {0} is recapping form {1}.", userID, travelForm.FormDataID));

                decimal registrationAmt = 0, airfareAmt = 0, rentalAmt = 0, fuelAmt = 0, parkingAmt = 0, mileageAmt = 0, lodgingAmt = 0, perdiemAmt = 0, miscAmt = 0,
                    totalAmt = 0, reimburseAmt = 0;
                int miles = 0, travelDays = 0, fullDays = 0;
                string[,] formFields = new string[0, 0];
                string[,] formFilters = new string[0, 0];

                // Check field values for correct data type
                try
                {
                    log.WriteLogEntry("Registration Cost: " + travelForm.RecapRegistrationCost);
                    registrationAmt = decimal.Parse(travelForm.RecapRegistrationCost);
                    log.WriteLogEntry("Airfare: " + travelForm.RecapAirfare);
                    airfareAmt = decimal.Parse(travelForm.RecapAirfare);
                    log.WriteLogEntry("RentalCar: " + travelForm.RecapRentalCar);
                    rentalAmt = decimal.Parse(travelForm.RecapRentalCar);
                    log.WriteLogEntry("Fuel: " + travelForm.RecapFuel);
                    fuelAmt = decimal.Parse(travelForm.RecapFuel);
                    log.WriteLogEntry("Parking: " + travelForm.RecapParkingTolls);
                    parkingAmt = decimal.Parse(travelForm.RecapParkingTolls);
                    log.WriteLogEntry("Miles Driven: " + travelForm.RecapMileage);
                    miles = int.Parse(travelForm.RecapMileage);
                    log.WriteLogEntry("Mileage Reimbursement: " + travelForm.RecapMileageAmount);
                    mileageAmt = int.Parse(travelForm.RecapMileageAmount);
                    log.WriteLogEntry("Lodging: " + travelForm.RecapLodging);
                    lodgingAmt = decimal.Parse(travelForm.RecapLodging);
                    log.WriteLogEntry("PerDiem: " + travelForm.RecapPerDiem);
                    perdiemAmt = decimal.Parse(travelForm.RecapPerDiem);
                    log.WriteLogEntry("Travel Days: " + travelForm.RecapTravelDays);
                    travelDays = int.Parse(travelForm.RecapTravelDays);
                    log.WriteLogEntry("Full Days: " + travelForm.RecapFullDays);
                    fullDays = int.Parse(travelForm.RecapFullDays);
                    log.WriteLogEntry("Misc: " + travelForm.RecapMisc);
                    miscAmt = decimal.Parse(travelForm.RecapMisc);
                    log.WriteLogEntry("Total Recap: " + travelForm.TotalRecap);
                    totalAmt = decimal.Parse(travelForm.TotalRecap);
                    log.WriteLogEntry("Total Reimbursement: " + travelForm.TotalReimburse);
                    reimburseAmt = decimal.Parse(travelForm.TotalReimburse, System.Globalization.NumberStyles.AllowLeadingSign | System.Globalization.NumberStyles.AllowDecimalPoint);
                }
                catch (Exception ex)
                {
                    log.WriteLogEntry("ERROR in form data conversion!\n" + ex.Message);
                    return result;
                }

                // Build field and filter list to update SQL
                try
                {
                    formFields = new string[,] {
                        { "recap_registration_amt", registrationAmt.ToString() },
                        { "recap_airfare_amt", airfareAmt.ToString() },
                        { "recap_rental_amt", rentalAmt.ToString() },
                        { "recap_fuel_amt", fuelAmt.ToString() },
                        { "recap_parking_amt", parkingAmt.ToString() },
                        { "recap_miles", miles.ToString() },
                        { "recap_mileage_amt", mileageAmt.ToString() },
                        { "recap_lodging_amt", lodgingAmt.ToString() },
                        { "recap_perdiem_amt", perdiemAmt.ToString() },
                        { "recap_travel_days", travelDays.ToString() },
                        { "recap_full_days", fullDays.ToString() },
                        { "recap_misc_amt", miscAmt.ToString() },
                        { "recap_total_amt", totalAmt.ToString() },
                        { "reimburse_amt", reimburseAmt.ToString() },
                        { "recap_date", DateTime.Now.ToString() },
                        { "recap_status", Constants.PendingValue.ToString() }
                    };
                    formFilters = new string[,] {
                        { "submitter_id", userID.ToString() },
                        { "form_id", travelForm.FormDataID.ToString() }
                    };
                }
                catch (Exception ex)
                {
                    log.WriteLogEntry("ERROR building SQL field and filter arrays!\n" + ex.Message);
                    return result;
                }

                log.WriteLogEntry("Starting FormDataHandler...");
                FormDataHandler formData = new FormDataHandler(dbServer, dbName);
                if (formData.UpdateTravelForm(formFields, formFilters) > 0)
                    result = true;
                else
                    log.WriteLogEntry("FAILED No records updated!");
            }
            else
                log.WriteLogEntry("FAILED submitted form is not a travel authorization form!");
            log.WriteLogEntry("End SubmitTravelRecapForm.");
            return result;
        }

        public bool LoadTravelAuthForm(int formDataID)
        {
            log.WriteLogEntry("Begin LoadTravelAuthForm...");
            bool result = false;
            TravelAuthForm travelForm = new TravelAuthForm();
            log.WriteLogEntry("Starting FormDataHandler...");
            FormDataHandler formDataHandle = new FormDataHandler(dbServer, dbName);
            if (formDataHandle.LoadTravelAuthForm(travelForm, formDataID))
            {
                if (ConvertStatusValues(travelForm))
                {
                    this.WebForm = travelForm;
                    result = true;
                }
            }
            else
                log.WriteLogEntry("FAILED to load travel auth form!");
            log.WriteLogEntry("End LoadTravelAuthForm.");
            return result;
        }

        public int LoadTravelForms(int commandID, int fieldID)
        {
            log.WriteLogEntry("Begin LoadTravelForms...");
            log.WriteLogEntry("Command ID: " + commandID + " Field ID: " + fieldID);
            int result = 0;
            string[] formFields = new string[0];
            string[,] formFilters = new string[0, 0];
            this.WebForms = new List<BaseForm>();
            List<BaseForm> travelForms = new List<BaseForm>();
            FormDataHandler formDataHandle = new FormDataHandler(dbServer, dbName);

            switch (commandID)
            {
                case Constants.GetFinanceTravelForms: // Load Finance Travel Forms
                    {
                        log.WriteLogEntry("Handling command ID: " + commandID);
                        string cmdString = string.Format(@"select * from valhalla.dbo.travel where close_status = 2 and ((request_advance = 1 and advance_status = 2) or (advance_status = 1 and recap_status = 2))", dbName);

                        log.WriteLogEntry("Starting FormDataHandler...");
                        if (formDataHandle.LoadTravelForms(travelForms, cmdString) > 0)
                        {
                            foreach (TravelAuthForm travelForm in travelForms)
                            {
                                if (ConvertStatusValues(travelForm))
                                {
                                    log.DumpObject(travelForm);
                                    this.WebForms.Add(travelForm);
                                    result++;
                                }
                                else
                                {
                                    log.WriteLogEntry("FAILED to convert status colors!");
                                    break;
                                }
                            }
                        }
                        else
                        {
                            log.WriteLogEntry("No forms found.");
                        }
                        break;
                    }
                case Constants.GetUserTravelForms: // Load User Travel Forms
                    {
                        log.WriteLogEntry("Handling command ID: " + commandID);
                        int userID = fieldID;
                        try
                        {
                            formFields = new string[] { "*" };
                            formFilters = new string[,] {
                                    { "submitter_id", userID.ToString(), "=", "and"},
                                    {"close_status", Constants.PendingValue.ToString(), "=", "and" }
                                };
                        }
                        catch (Exception ex)
                        {
                            log.WriteLogEntry("ERROR building SQL field and filter arrays!\n" + ex.Message);
                            return result;
                        }

                        log.WriteLogEntry("Starting FormDataHandler...");
                        if (formDataHandle.LoadTravelForms(travelForms, formFields, formFilters) > 0)
                        {
                            foreach (TravelAuthForm travelForm in travelForms)
                            {
                                if (ConvertStatusValues(travelForm))
                                {
                                    log.DumpObject(travelForm);
                                    this.WebForms.Add(travelForm);
                                    result++;
                                }
                                else
                                {
                                    log.WriteLogEntry("FAILED to convert status colors!");
                                    break;
                                }
                            }
                        }
                        else
                        {
                            log.WriteLogEntry("No forms found.");
                        }
                        break;
                    }
                case Constants.GetApproverTravelForms: // Load Approver Travel Forms
                    {
                        log.WriteLogEntry("Handling command ID: " + commandID);
                        int userID = fieldID;
                        try
                        {
                            formFields = new string[] { "*" };
                            formFilters = new string[,] {
                                    { "submitter_id", userID.ToString(), "=", "and" },
                                    { "approval_status", Constants.PendingValue.ToString(), "=", "and" }
                                };
                        }
                        catch (Exception ex)
                        {
                            log.WriteLogEntry("ERROR building SQL field and filter arrays!\n" + ex.Message);
                            return result;
                        }

                        log.WriteLogEntry("Starting FormDataHandler...");
                        if (formDataHandle.LoadTravelForms(travelForms, formFields, formFilters) > 0)
                        {
                            foreach (TravelAuthForm travelForm in travelForms)
                            {
                                if (ConvertStatusValues(travelForm))
                                {
                                    if (userID == int.Parse(travelForm.GMID) && travelForm.DHApproval.ToLower() == Constants.ApprovedColor)
                                    {
                                        this.WebForms.Add(travelForm);
                                        result++;
                                    }
                                    else if (userID == int.Parse(travelForm.DHID) && travelForm.DHApproval.ToLower() == Constants.PendingColor)
                                    {
                                        this.WebForms.Add(travelForm);
                                        result++;
                                    }
                                    else
                                        log.WriteLogEntry("No forms available for approval by user " + userID);
                                    log.DumpObject(travelForm);
                                }
                                else
                                {
                                    log.WriteLogEntry("FAILED to convert status colors!");
                                    break;
                                }
                            }
                        }
                        else
                        {
                            log.WriteLogEntry("No forms available for approval by user " + userID);
                        }
                        break;
                    }
                default:
                    {
                        log.WriteLogEntry("Fell through switch button ID. No action taken.");
                        return result;
                    }
            }
            log.WriteLogEntry("Count loaded forms " + result);
            log.WriteLogEntry("End LoadTravelForms.");
            return result;
        }

        public bool ApproveTravelAuthForm(int userID, BaseForm webForm)
        {
            log.WriteLogEntry("Begin ApproveTravelAuthForm...");
            bool result = false;
            string[,] formFields = new string[0, 0];
            string[,] formFilters = new string[0, 0];

            if (webForm.GetType() == typeof(TravelAuthForm))
            {
                TravelAuthForm travelForm = (TravelAuthForm)webForm;
                log.WriteLogEntry(string.Format("User {0} is approving form {1}.", userID, travelForm.FormDataID));
                if (userID == int.Parse(travelForm.GMID))
                {
                    if (travelForm.GMApproval == Constants.DeniedColor)
                        travelForm.ApprovalStatus = Constants.DeniedColor;
                    else
                    {
                        travelForm.GMApproval = Constants.ApprovedColor;
                        travelForm.ApprovalStatus = Constants.ApprovedColor;
                    }

                    // Build field and filter list to update SQL
                    try
                    {
                        formFields = new string[,] {
                            { "manager_approval_date", DateTime.Now.ToString() },
                            { "manager_approval_status", GetStatusValue(travelForm.GMApproval).ToString() },
                            { "approval_date", DateTime.Now.ToString() },
                            { "approval_status", GetStatusValue(travelForm.ApprovalStatus).ToString() }
                        };
                        formFilters = new string[,] {
                            { "manager_id", userID.ToString() },
                            { "form_id", travelForm.FormDataID.ToString() }
                        };
                    }
                    catch (Exception ex)
                    {
                        log.WriteLogEntry("ERROR building SQL field and filter arrays!\n" + ex.Message);
                        return result;
                    }
                }
                else if (userID == int.Parse(travelForm.DHID))
                {
                    if (travelForm.DHApproval == Constants.DeniedColor)
                        travelForm.ApprovalStatus = Constants.DeniedColor;
                    else
                        travelForm.DHApproval = Constants.ApprovedColor;

                    // Build field and filter list to update SQL
                    try
                    {
                        formFields = new string[,] {
                            { "supervisor_approval_date", DateTime.Now.ToString() },
                            { "supervisor_approval_status", GetStatusValue(travelForm.DHApproval).ToString() }
                        };
                        formFilters = new string[,] {
                            { "supervisor_id", userID.ToString() },
                            { "form_id", travelForm.FormDataID.ToString() }
                        };
                    }
                    catch (Exception ex)
                    {
                        log.WriteLogEntry("ERROR building SQL field and filter arrays!\n" + ex.Message);
                        return result;
                    }
                }
                log.WriteLogEntry("Starting FormDataHandler...");
                FormDataHandler formData = new FormDataHandler(dbServer, dbName);
                if (formData.UpdateTravelForm(formFields, formFilters) > 0)
                    result = true;
                else
                    log.WriteLogEntry("FAILED No records updated!");
            }
            else
                log.WriteLogEntry("FAILED not a travel form!");
            log.WriteLogEntry("End ApproveTravelAuthForm.");
            return result;
        }

        public bool HandleFinanceAction(int commandID, BaseForm webForm)
        {
            log.WriteLogEntry("Begin HandleFinanceAction...");
            bool result = false;
            string[,] formFields = new string[0, 0];
            string[,] formFilters = new string[0, 0];

            if (webForm.GetType() == typeof(TravelAuthForm))
            {
                TravelAuthForm travel = (TravelAuthForm)webForm;
                switch (commandID)
                {
                    case 0: // Approve Advance
                        {
                            log.WriteLogEntry("Approve advance.");
                            try
                            {
                                formFields = new string[,] {
                                    { "advance_status", Constants.ApprovedValue.ToString() },
                                    { "advance_date", DateTime.Now.ToString() }
                                };
                                formFilters = new string[,] {
                                    { "form_id", travel.FormDataID.ToString() }
                                };
                            }
                            catch (Exception ex)
                            {
                                log.WriteLogEntry("ERROR building SQL field and filter arrays!\n" + ex.Message);
                                return result;
                            }
                            break;
                        }
                    case 1: // Approve Recap
                        {
                            log.WriteLogEntry("Approve recap.");
                            try
                            {
                                formFields = new string[,] {
                                    { "recap_status", Constants.ApprovedValue.ToString() },
                                    { "recap_date", DateTime.Now.ToString() },
                                    { "close_status", Constants.ApprovedValue.ToString() },
                                    { "close_date", DateTime.Now.ToString() }
                                };
                                formFilters = new string[,] {
                                    { "form_id", travel.FormDataID.ToString() }
                                };
                            }
                            catch (Exception ex)
                            {
                                log.WriteLogEntry("ERROR building SQL field and filter arrays!\n" + ex.Message);
                                return result;
                            }
                            break;
                        }
                    case 2: // Deny Advance
                        {
                            log.WriteLogEntry("Deny advance.");
                            try
                            {
                                formFields = new string[,] {
                                    { "advance_status", Constants.DeniedValue.ToString() },
                                    { "advance_date", DateTime.Now.ToString() }
                                };
                                formFilters = new string[,] {
                                    { "form_id", travel.FormDataID.ToString() }
                                };
                            }
                            catch (Exception ex)
                            {
                                log.WriteLogEntry("ERROR building SQL field and filter arrays!\n" + ex.Message);
                                return result;
                            }
                            break;
                        }
                    case 3: // Deny Recap
                        {
                            log.WriteLogEntry("Deny recap.");
                            try
                            {
                                formFields = new string[,] {
                                    { "recap_status", Constants.DeniedValue.ToString() },
                                    { "recap_date", DateTime.Now.ToString() }
                                };
                                formFilters = new string[,] {
                                    { "form_id", travel.FormDataID.ToString() }
                                };
                            }
                            catch (Exception ex)
                            {
                                log.WriteLogEntry("ERROR building SQL field and filter arrays!\n" + ex.Message);
                                return result;
                            }
                            break;
                        }
                    default:
                        {
                            log.WriteLogEntry("Fell through switch. No action taken.");
                            return result;
                        }
                }
                log.WriteLogEntry("Starting FormDataHandler...");
                FormDataHandler formData = new FormDataHandler(dbServer, dbName);
                if (formData.UpdateTravelForm(formFields, formFilters) > 0)
                    result = true;
                else
                    log.WriteLogEntry("FAILED No records updated!");
            }
            else
                log.WriteLogEntry("FAILED not a travel form!");
            log.WriteLogEntry("End HandleFinanceAction.");
            return result;
        }

        private string GetStatusColor(int status)
        {
            log.WriteLogEntry("Getting status color.");
            string result = string.Empty;
            switch (status)
            {
                case 0:
                    result = Constants.DeniedColor;
                    break;
                case 1:
                    result = Constants.ApprovedColor;
                    break;
                case 2:
                    result = Constants.PendingColor;
                    break;
                default:
                    result = Constants.PendingColor;
                    break;
            }
            log.WriteLogEntry("Returning color: " + result);
            return result;
        }

        private int GetStatusValue(string status)
        {
            log.WriteLogEntry("Getting status value.");
            if (!int.TryParse(status, out int result))
            {
                switch (status.ToLower())
                {
                    case "red":
                        result = Constants.DeniedValue;
                        break;
                    case "green":
                        result = Constants.ApprovedValue;
                        break;
                    case "yellow":
                        result = Constants.PendingValue;
                        break;
                    default:
                        result = Constants.PendingValue;
                        break;
                }
            }
            else
                log.WriteLogEntry("Status value is a number: " + status);
            log.WriteLogEntry("Returning status value: " + result);
            return result;
        }

        private bool ConvertStatusValues(TravelAuthForm travelForm)
        {
            log.WriteLogEntry("Converting status values.");
            bool result = false;
            if (int.TryParse(travelForm.ApprovalStatus, out int approve))
            {
                if (int.TryParse(travelForm.DHApproval, out int dhApprove))
                {
                    if (int.TryParse(travelForm.GMApproval, out int gmApprove))
                    {
                        travelForm.ApprovalStatus = GetStatusColor(approve);
                        travelForm.DHApproval = GetStatusColor(dhApprove);
                        travelForm.GMApproval = GetStatusColor(gmApprove);
                        result = true;
                    }
                    else
                        log.WriteLogEntry("FAILED GM approval status not recognized!");
                }
                else
                    log.WriteLogEntry("FAILED DH approval status not recognized!");
            }
            else
                log.WriteLogEntry("FAILED approval status not recognized!");
            return result;
        }
    }
}
