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
            string[,] formFields = new string[,] { };
            string[,] formFilters = new string[,] { };

            if (webForm.GetType() == typeof(TravelAuthForm))
            {
                TravelAuthForm travel = (TravelAuthForm)webForm;
                bool dhApprove = bool.TryParse(travel.DHApproval, out bool dh);
                bool gmApprove = bool.TryParse(travel.GMApproval, out bool gm);
                if (dhApprove)
                {
                    formFields[0, 0] = "supervisor_approval_date";
                    formFields[0, 1] = DateTime.Now.ToShortDateString();
                    formFilters[0, 0] = "supervisor_id";
                    formFilters[0, 1] = userID.ToString();
                    formFilters[1, 0] = "form_id";
                    formFilters[1, 1] = webForm.FormDataID.ToString();
                    formFilters[2, 0] = "supervisor_approval_date";
                    formFilters[2, 1] = "null";
                    FormDataHandler formData = new FormDataHandler(dbServer, dbName);
                    if (formData.UpdateForm(formFields, formFilters) > 0)
                        result = true;
                }
                if (gmApprove)
                {
                    formFields[0, 0] = "manager_approval_date";
                    formFields[0, 1] = DateTime.Now.ToShortDateString();
                    formFilters[0, 0] = "manager_id";
                    formFilters[0, 1] = userID.ToString();
                    formFilters[1, 0] = "form_id";
                    formFilters[1, 1] = webForm.FormDataID.ToString();
                    formFilters[2, 0] = "manager_approval_date";
                    formFilters[2, 1] = "null";
                    FormDataHandler formData = new FormDataHandler(dbServer, dbName);
                    if (formData.UpdateForm(formFields, formFilters) > 0)
                        result = true;
                }
            }
            log.WriteLogEntry("End ApproveTravelAuthForm.");
            return result;
        }
    }
}
