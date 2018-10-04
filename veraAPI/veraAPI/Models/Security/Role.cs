using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VeraAPI.Models.Security
{
    public class Role
    {
        public int RoleNumber { get; set; }
        public string RoleTitle { get; set; }
        public string RoleDescription { get; set; }
    }
}