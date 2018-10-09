using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VeraAPI.Models.Security
{
    public class Company
    {
        public int CompanyNumber { get; set; }
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public int CompanyAccessLevel { get; set; }
        public int GeneralManagerUserID { get; set; }
        public string GeneralManagerName { get; set; }
        public string GeneralManagerEmail { get; set; }
    }
}