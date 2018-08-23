using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VeraAPI.Models.DataHandler
{
    public class DataUser
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmployeeID { get; set; }
        public string Department { get; set; }
        public string SupervisorName { get; set; }
        public string SupervisorEmpID { get; set; }
        public string SupervisorEmail { get; set; }
        public string DepartmentHead { get; set; }
        public string DepartmentHeadEmpID { get; set; }
        public string DepartmentHeadEmail { get; set; }
        public string GeneralManager { get; set; }
        public string GeneralManagerEmpID { get; set; }
        public string GeneralManagerEmail { get; set; }
    }
}