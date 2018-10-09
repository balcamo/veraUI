using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VeraAPI.Models.Security
{
    public class DomainUser : User
    {
        public string DomainUserName { get; set; }
        public string DomainUpn { get; set; }
        public string EmployeeID { get; set; }
        public string SupervisorName { get; set; }
        public string SupervisorEmpID { get; set; }
        public string SupervisorEmail { get; set; }

        public Department Department { get; set; } = new Department();
        public Position Position { get; set; } = new Position();
        public List<Role> SecurityRoles { get; set; } = new List<Role>();
        public List<Access> SecurityAccess { get; set; } = new List<Access>();
        public Domain Domain { get; set; } = new Domain();

        public DomainUser()
        {
            SecurityRoles.Add(new Role { RoleNumber = 0, RoleTitle = "None", RoleDescription = "Unknown Role" });
            SecurityAccess.Add(new Access { AccessNumber = 0, AccessTitle = "None", AccessDescription = "No Access" });
        }

        public DomainUser(int userID) : base(userID)
        {
            SecurityRoles.Add(new Role { RoleNumber = 0, RoleTitle = "None", RoleDescription = "Unknown Role" });
            SecurityAccess.Add(new Access { AccessNumber = 0, AccessTitle = "None", AccessDescription = "No Access" });
        }
    }
}