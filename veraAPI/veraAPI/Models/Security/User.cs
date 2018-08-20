using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VeraAPI.Models.Security
{
    public class User
    {
        public int UserID { get; set; }
        public string EmployeeID { get; set; }
        public string UserName { get; set; }
        public string UserPwd { get; set; }
        public string UserEmail { get; set; }
        public int UserType { get; set; }
        public string ADDomain { get; set; }
        public string AdUpn { get; set; }
        public string AdSam { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
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
        public bool Authenicated { get; set; }

        public User()
        {
            this.Authenicated = false;
        }

        public User(bool authenticated)
        {
            this.Authenicated = authenticated;
        }
    }
}