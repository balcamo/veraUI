using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VeraAPI.Models.Security
{
    public class Department
    {
        public int DeptNumber { get; set; }
        public string DeptName { get; set; }
        public string DeptEmail { get; set; }
        public int DeptHeadUserID { get; set; }
        public string DeptHeadName { get; set; }
        public string DeptHeadEmail { get; set; }
    }
}