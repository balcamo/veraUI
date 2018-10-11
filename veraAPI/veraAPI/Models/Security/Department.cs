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

        public static readonly int ITDept = 1;
        public static readonly int OperationsDept = 2;
        public static readonly int FinanceDept = 3;
        public static readonly int EngineeringDept = 4;
        public static readonly int ElectricDept = 5;
        public static readonly int WaterDept = 6;
        public static readonly int MaintenanceDept = 7;
        public static readonly int OfficeDept = 8;
        public static readonly int ExecutiveDept = 9;
        public static readonly int TestDept = 10;
    }
}