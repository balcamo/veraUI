using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VeraAPI.Models.Security
{
    public class DomainUser : User
    {
        public string DomainName { get; set; }
        public string DomainUpn { get; set; }
        public string DomainUserName { get; set; }
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