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
        //public JobTemplate Template { get; private set; }

        private readonly string dbServer = WebConfigurationManager.AppSettings.Get("DBServer");
        private readonly string dbName = WebConfigurationManager.AppSettings.Get("DBName");
        private static Scribe log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "FormHelper_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
        private FormDataHandler formDataHandle;
        //private Validator formValidator;

        public FormHelper()
        {
            this.WebForm = new BaseForm();
        }

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
                if (user.GetType() == typeof(DomainUser))
                {
                    DomainUser domainUser = (DomainUser)user;
                    travelForm.UserID = domainUser.UserID;
                    travelForm.DHID = domainUser.Department.DeptHeadUserID.ToString();
                    travelForm.GMID = domainUser.Company.GeneralManagerUserID.ToString();

                    // Load the job template corresponding to the templateID for the submitted form
                    log.WriteLogEntry("Starting FormDataHandler...");
                    formDataHandle = new FormDataHandler(dbServer, dbName);
                    if (formDataHandle.LoadFormTemplate(webForm.TemplateID))
                    {
                        // Insert travel form data into the database
                        if (formDataHandle.InsertTravelAuth(travelForm))
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

        public bool LoadTravelAuthForm()
        {
            log.WriteLogEntry("Starting LoadTravelAuthForm...");
            bool result = false;
            formDataHandle = new FormDataHandler(dbServer, dbName);
            formDataHandle.LoadTravelAuthForm(WebForm.FormDataID);
            log.WriteLogEntry("End LoadTravelAuthForm.");
            return result;
        }

        public int LoadActiveTravelAuthForms(int userID)
        {
            log.WriteLogEntry("Starting LoadActiveTravelAuthForms...");
            int result = 0;
            formDataHandle = new FormDataHandler(dbServer, dbName);
            formDataHandle.LoadUserTravelAuthForms(userID);
            WebForms = formDataHandle.WebForms;
            log.WriteLogEntry("Count loaded forms " + WebForms.Count);
            log.WriteLogEntry("End LoadActiveTravelAuthForms.");
            return result;
        }

        public int LoadApproverTravelAuthForms(int userID)
        {
            log.WriteLogEntry("Starting LoadApproverTravelAuthForms...");
            int result = 0;
            formDataHandle = new FormDataHandler(dbServer, dbName);
            formDataHandle.LoadApproverTravelAuthForms(userID);
            WebForms = formDataHandle.WebForms;
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
                try
                {
                    bool dhApprove = bool.TryParse(travel.DHApproval, out bool dh);
                    bool gmApprove = bool.TryParse(travel.GMApproval, out bool gm);
                    log.WriteLogEntry(string.Format("Form DHApproval {0} Bool dhApprove {1}", travel.DHApproval, dhApprove));
                    log.WriteLogEntry(string.Format("Form GMApproval {0} Bool gmApprove {1}", travel.GMApproval, gmApprove));
                    if (dhApprove)
                    {
                        string[,] formFields = new string[,] { { "supervisor_approval_date", DateTime.Now.ToString() } };
                        string[,] formFilters = new string[,] { { "supervisor_id", userID.ToString() }, { "form_id", travel.FormDataID.ToString() }, { "supervisor_approval_date", "null" } };
                        log.WriteLogEntry(string.Format("FormFields array length 0 {0} length 1 {1}", formFields.GetLength(0), formFields.GetLength(1)));
                        log.WriteLogEntry(string.Format("FormFilters array length 0 {0} length 1 {1}", formFilters.GetLength(0), formFilters.GetLength(1)));

                        log.WriteLogEntry("Starting FormDataHandler...");
                        FormDataHandler formData = new FormDataHandler(dbServer, dbName);
                        if (formData.UpdateForm(formFields, formFilters) > 0)
                            result = true;
                    }
                    else
                        log.WriteLogEntry("Department Head approval FALSE");

                    if (gmApprove)
                    {
                        string[,] formFields = new string[,] { { "manager_approval_date", DateTime.Now.ToString() } };
                        string[,] formFilters = new string[,] { { "manager_id", userID.ToString() }, { "form_id", travel.FormDataID.ToString() }, { "manager_approval_date", "null" } };
                        log.WriteLogEntry(string.Format("FormFields array length 0 {0} length 1 {1}", formFields.GetLength(0), formFields.GetLength(1)));
                        log.WriteLogEntry(string.Format("FormFilters array length 0 {0} length 1 {1}", formFilters.GetLength(0), formFilters.GetLength(1)));

                        log.WriteLogEntry("Starting FormDataHandler...");
                        FormDataHandler formData = new FormDataHandler(dbServer, dbName);
                        if (formData.UpdateForm(formFields, formFilters) > 0)
                            result = true;
                        else
                            log.WriteLogEntry("FAILED no records updated!");
                    }
                    else
                        log.WriteLogEntry("General Manager approval FALSE");
                }
                catch (Exception ex)
                {
                    log.WriteLogEntry("General Program Error \n" + ex.Message);
                }
            }
            else
                log.WriteLogEntry("FAILED not a travel form!");
            log.WriteLogEntry("End ApproveTravelAuthForm.");
            return result;
        }
    }
}
