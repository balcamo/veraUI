using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VeraAPI.Models.Security
{
    public class User
    {
        public int UserID { get; set; }
        public int CompanyNumber { get; set; }
        public int DeptNumber { get; set; }
        public int PositionNumber { get; set; }
        public string UserName { get; set; }
        public string UserPwd { get; set; }
        public string UserEmail { get; set; }
        public bool Authenicated { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int[] UserAccessKey { get; set; } = new int[] { 0, 0, 0, 0, 0 };

        public Token Token { get; set; } = new Token();
        public Company Company { get; set; } = new Company();
    }
}