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
                    travelForm.GMID = domainUser.Company.GeneralManagerUserID.ToString();
                    travelForm.GMEmail = domainUser.Company.GeneralManagerEmail;
                    travelForm.GMApproval = Constants.PendingValue.ToString();
                    travelForm.ApprovalStatus = Constants.PendingValue.ToString();

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
                        log.WriteLogEntry("Failed insert form data to database.");
                    }
                    else
                        log.WriteLogEntry("Failed loading form template!");
                }
            }
            else
                log.WriteLogEntry("FAILED submitted form is not a travel authorization form!");
            log.WriteLogEntry("End FormHelp SubmitForm result " + result);
            return result;
        }

        public bool LoadTravelAuthForm(int formDataID)
        {
            log.WriteLogEntry("Starting LoadTravelAuthForm...");
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

        public bool LoadActiveTravelAuthForms(int userID)
        {
            log.WriteLogEntry("Starting LoadActiveTravelAuthForms...");
            bool result = false;
            List<BaseForm> travelForms = new List<BaseForm>();
            log.WriteLogEntry("Starting FormDataHandler...");
            FormDataHandler formDataHandle = new FormDataHandler(dbServer, dbName);
            if (formDataHandle.LoadUserTravelAuthForms(travelForms, userID) > 0)
            {
                WebForms = new List<BaseForm>();
                foreach (TravelAuthForm travelForm in travelForms)
                {
                    if (ConvertStatusValues(travelForm))
                    {
                        this.WebForms.Add(travelForm);
                        result = true;
                        log.WriteLogEntry(string.Format("User: {0} Approval Status: {1} Dept Head: {2} DH Approval: {3} GM: {4} GM Approval {5}", userID, travelForm.ApprovalStatus, travelForm.DHID, travelForm.DHApproval, travelForm.GMID, travelForm.GMApproval));
                    }
                    else
                    {
                        log.WriteLogEntry("FAILED to convert status colors!");
                        result = false;
                        break;
                    }
                }
            }
            else
                log.WriteLogEntry("No active forms loaded.");
            log.WriteLogEntry("Count loaded forms " + WebForms.Count);
            log.WriteLogEntry("End LoadActiveTravelAuthForms.");
            return result;
        }

        public bool LoadApproverTravelAuthForms(int userID)
        {
            log.WriteLogEntry("Starting LoadApproverTravelAuthForms...");
            bool result = false;
            List<BaseForm> travelForms = new List<BaseForm>();
            log.WriteLogEntry("Starting FormDataHandler...");
            FormDataHandler formDataHandle = new FormDataHandler(dbServer, dbName);
            if (formDataHandle.LoadApproverTravelAuthForms(travelForms, userID) > 0)
            {
                WebForms = new List<BaseForm>();
                foreach (TravelAuthForm travelForm in travelForms)
                {
                    if (ConvertStatusValues(travelForm))
                    {
                        if (travelForm.DHApproval == Constants.PendingColor && userID == int.Parse(travelForm.DHID))
                            this.WebForms.Add(travelForm);
                        else if (travelForm.GMApproval == Constants.PendingColor && userID == int.Parse(travelForm.GMID))
                            this.WebForms.Add(travelForm);
                        result = true;
                        log.WriteLogEntry(string.Format("User: {0} Approval Status: {1} Dept Head: {2} DH Approval: {3} GM: {4} GM Approval {5}", userID, travelForm.ApprovalStatus, travelForm.DHID, travelForm.DHApproval, travelForm.GMID, travelForm.GMApproval));
                    }
                    else
                    {
                        log.WriteLogEntry("FAILED to convert status colors!");
                        result = false;
                        break;
                    }
                }
            }
            log.WriteLogEntry("Count loaded forms " + WebForms.Count);
            log.WriteLogEntry("End LoadApproverTravelAuthForms.");
            return result;
        }

        public bool ApproveTravelAuthForm(int userID, BaseForm webForm)
        {
            log.WriteLogEntry("Begin ApproveTravelAuthForm...");
            bool result = false;

            if (webForm.GetType() == typeof(TravelAuthForm))
            {
                TravelAuthForm travel = (TravelAuthForm)webForm;
                log.WriteLogEntry(string.Format("User {0} is approving form {1}.", userID, travel.FormDataID));
                int[] approve = { GetStatusValue(travel.DHApproval), GetStatusValue(travel.GMApproval) };
                string[] approver = { "supervisor", "manager" };
                for (int i = 0; i < approve.Length; i++)
                {
                    try
                    {
                        string[,] formFields = { { approver[i] + "_approval_date", DateTime.Now.ToString() }, { approver[i] + "_approval_status", approve[i].ToString() } };
                        string[,] formFilters = { { approver[i] + "_id", userID.ToString() }, { "form_id", travel.FormDataID.ToString() } };
                        log.WriteLogEntry(string.Format("FormFields array rows {0} columns {1}", formFields.GetLength(0), formFields.GetLength(1)));
                        log.WriteLogEntry(string.Format("FormFilters array rows {0} columns {1}", formFilters.GetLength(0), formFilters.GetLength(1)));

                        log.WriteLogEntry("Starting FormDataHandler...");
                        FormDataHandler formData = new FormDataHandler(dbServer, dbName);
                        if (formData.UpdateTravelForm(formFields, formFilters) > 0)
                            result = true;
                        else
                            log.WriteLogEntry("FAILED No records updated!");
                    }
                    catch (Exception ex)
                    {
                        log.WriteLogEntry("General Program Error \n" + ex.Message);
                    }
                }
            }
            else
                log.WriteLogEntry("FAILED not a travel form!");
            log.WriteLogEntry("End ApproveTravelAuthForm.");
            return result;
        }

        private string GetStatusColor(int status)
        {
            log.WriteLogEntry("Getting status color...");
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
            log.WriteLogEntry("Getting status value...");
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
            log.WriteLogEntry("Converting status values...");
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
