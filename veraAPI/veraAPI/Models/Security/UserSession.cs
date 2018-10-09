using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VeraAPI.Models.Security
{
    public class UserSession
    {
        public int UserID { get; set; }
        public int CompanyNumber { get; set; }
        public int DeptNumber { get; set; }
        public int PositionNumber { get; set; }
        public int RoleNumber { get; set; }
        public int DomainNumber { get; set; }
        public int AccessLevel { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmployeeID { get; set; }
        public string DeptName { get; set; }
        public string DeptHeadName { get; set; }
        public string DeptHeadEmail { get; set; }
        public string SupervisorName { get; set; }
        public string SupervisorEmail { get; set; }
        public string DomainUpn { get; set; }
        public string DomainUserName { get; set; }
        public string SessionKey { get; set; }
        public bool Authenicated { get; set; }

        public UserSession(int userID)
        {
            this.UserID = userID;
        }
    }
}