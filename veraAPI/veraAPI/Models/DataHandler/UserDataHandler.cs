using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using VeraAPI.Models.Security;

namespace VeraAPI.Models.DataHandler
{
    public class UserDataHandler : SQLDataHandler
    {
        private Scribe Log;
        private string dataConnectionString = string.Empty;
        private string dbServer = string.Empty;
        private string dbName = string.Empty;
        public User CurrentUser { get; set; }
        public List<User> Users { get; set; }

        public UserDataHandler(string dbServer, string dbName) : base(dbServer)
        {
            this.dbServer = dbServer;
            this.dbName = dbName;
            this.Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UserDataHandler_" + DateTime.Now.ToString("yyyyMMdd") + ".log");
            this.dataConnectionString = GetDataConnectionString();
        }

        public UserDataHandler(string dbServer, string dbName, Scribe Log) : base(dbServer)
        {
            this.dbServer = dbServer;
            this.dbName = dbName;
            this.Log = Log;
            this.dataConnectionString = GetDataConnectionString();
        }

        public bool LoadUser(string userPrincipalName)
        {
            Log.WriteLogEntry("Begin LoadUser...");
            bool result = false;
            string cmdString = string.Format(@"select * from {0}.dbo.user_account where user_upn = @upn", dbName);
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@upn", userPrincipalName);
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                        {
                            if (rdr.Read())
                            {
                                CurrentUser = new User();
                                CurrentUser.UserID = (int)rdr["user_id"];
                                CurrentUser.UserEmail = rdr["email"].ToString();
                                CurrentUser.AdSam = rdr["user_sam"].ToString();
                                CurrentUser.AdUpn = rdr["user_upn"].ToString();
                                CurrentUser.EmployeeID = rdr["employee_id"].ToString();
                                CurrentUser.FirstName = rdr["first_name"].ToString();
                                CurrentUser.LastName = rdr["last_name"].ToString();
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        Log.WriteLogEntry("SQL error " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLogEntry("General program error " + ex.Message);
                    }
                }
            }
            Log.WriteLogEntry("End LoadUser.");
            return result;
        }

        public int LoadAllUsers()
        {
            Log.WriteLogEntry("Begin LoadAllUsers...");
            int result = 0;
            string cmdString = string.Format(@"select * from {0}.dbo.user_account", dbName);
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    try
                    {
                        conn.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                CurrentUser = new User();
                                CurrentUser.UserID = (int)rdr["user_id"];
                                CurrentUser.UserEmail = rdr["email"].ToString();
                                CurrentUser.AdSam = rdr["user_sam"].ToString();
                                CurrentUser.AdUpn = rdr["user_upn"].ToString();
                                CurrentUser.EmployeeID = rdr["employee_id"].ToString();
                                CurrentUser.FirstName = rdr["first_name"].ToString();
                                CurrentUser.LastName = rdr["last_name"].ToString();
                                Users.Add(CurrentUser);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        Log.WriteLogEntry("SQL error " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLogEntry("General program error " + ex.Message);
                    }
                }
            }
            result = Users.Count;
            Log.WriteLogEntry("End LoadAllUsers.");
            return result;
        }

        public bool FillDepartmentHead()
        {
            Log.WriteLogEntry("Begin FillDepartmentHead...");
            bool result = false;
            string depString = string.Format(@"select dept_head_emp_id from {0}.dbo.department where dept_name = @deptName", dbName);
            string empString = string.Format(@"select first_name, last_name, email from {0}.dbo.user_account where employee_id = @empID", dbName);
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand deptCmd = new SqlCommand(depString, conn))
                    {
                        deptCmd.Parameters.AddWithValue("@deptName", CurrentUser.Department);
                        var emp = deptCmd.ExecuteScalar();
                        if (emp != null)
                        {
                            CurrentUser.DepartmentHeadEmpID = emp.ToString();
                            result = true;
                        }
                    }
                    if (result)
                    {
                        using (SqlCommand empCmd = new SqlCommand(empString, conn))
                        {
                            empCmd.Parameters.AddWithValue("@empID", CurrentUser.DepartmentHeadEmpID);
                            using (SqlDataReader rdr = empCmd.ExecuteReader(CommandBehavior.SingleResult))
                            {
                                if (rdr.Read())
                                {
                                    string empName = string.Format("{0} {1}", rdr["first_name"].ToString(), rdr["last_name"].ToString());
                                    CurrentUser.DepartmentHead = empName;
                                    CurrentUser.DepartmentHeadEmail = rdr["email"].ToString();
                                    result = true;
                                }
                                else
                                    result = false;
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Log.WriteLogEntry("SQL error " + ex.Message);
                }
                catch (Exception ex)
                {
                    Log.WriteLogEntry("General program error " + ex.Message);
                }
            }
            Log.WriteLogEntry("End FillDepartmentHead.");
            return result;
        }

        public bool FillGeneralManager()
        {
            Log.WriteLogEntry("Begin FillGeneralManager...");
            bool result = false;
            string comString = string.Format(@"select general_manager_emp_id from {0}.dbo.company", dbName);
            string empString = string.Format(@"select first_name, last_name, email from {0}.dbo.user_account where employee_id = @empID", dbName);
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand comCmd = new SqlCommand(comString, conn))
                    {
                        var emp = comCmd.ExecuteScalar();
                        if (emp != null)
                        {
                            CurrentUser.GeneralManagerEmpID = emp.ToString();
                            result = true;
                        }
                    }
                    if (result)
                    {
                        using (SqlCommand empCmd = new SqlCommand(empString, conn))
                        {
                            empCmd.Parameters.AddWithValue("@empID", CurrentUser.GeneralManagerEmpID);
                            using (SqlDataReader rdr = empCmd.ExecuteReader(CommandBehavior.SingleResult))
                            {
                                if (rdr.Read())
                                {
                                    string empName = string.Format("{0} {1}", rdr["first_name"].ToString(), rdr["last_name"].ToString());
                                    CurrentUser.GeneralManager = empName;
                                    CurrentUser.GeneralManagerEmail = rdr["email"].ToString();
                                    result = true;
                                }
                                else
                                    result = false;
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Log.WriteLogEntry("SQL error " + ex.Message);
                }
                catch (Exception ex)
                {
                    Log.WriteLogEntry("General program error " + ex.Message);
                }
            }
            Log.WriteLogEntry("End FillGeneralManager.");
            return result;
        }

        public int InsertAllUsers()
        {
            Log.WriteLogEntry("Begin InsertAllUsers...");
            int result = 0;
            string cmdString = string.Format(@"insert into {0}.dbo.user_account (first_name, last_name, email, user_sam, user_upn, employee_id)
                                            values (@firstName, @lastName, @email, @sam, @upn, @empID)", dbName);
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                try
                {
                    conn.Open();
                    Log.WriteLogEntry("Open SQL connection successful.");
                    foreach (User ADUser in Users)
                    {
                        if (ADUser.EmployeeID != null)
                        {
                            using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                            {
                                Log.WriteLogEntry("AD User " + ADUser.FirstName + " " + ADUser.LastName + " " + ADUser.AdUpn + " " + ADUser.EmployeeID);
                                cmd.Parameters.AddWithValue("@firstName", ADUser.FirstName);
                                cmd.Parameters.AddWithValue("@lastName", ADUser.LastName);
                                cmd.Parameters.AddWithValue("@email", ADUser.UserEmail);
                                cmd.Parameters.AddWithValue("@sam", ADUser.AdSam);
                                cmd.Parameters.AddWithValue("@upn", ADUser.AdUpn);
                                cmd.Parameters.AddWithValue("@empID", ADUser.EmployeeID);
                                cmd.ExecuteNonQuery();
                                result++;
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Log.WriteLogEntry("SQL error " + ex.Message);
                }
                catch (Exception ex)
                {
                    Log.WriteLogEntry("General program error " + ex.Message);
                }
            }
            Log.WriteLogEntry("Inserted users " + result);
            Log.WriteLogEntry("End InsertAllUsers.");
            return result;
        }

        public int UpdateAllDepartment()
        {
            Log.WriteLogEntry("Begin UpdateAllDepartment...");
            int result = 0;
            string depString = string.Format(@"update {0}.dbo.user_account set department = @depName where employee_id = @empID", dbName);
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                try
                {
                    conn.Open();
                    Log.WriteLogEntry("Open SQL connection successful.");
                    foreach (User ADUser in Users)
                    {
                        if (ADUser.EmployeeID != null)
                        {
                            if (ADUser.Department != null)
                            {
                                using (SqlCommand depCmd = new SqlCommand(depString, conn))
                                {
                                    depCmd.Parameters.AddWithValue("@depName", ADUser.Department);
                                    depCmd.Parameters.AddWithValue("@empID", ADUser.EmployeeID);
                                    depCmd.ExecuteNonQuery();
                                    result++;
                                }
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Log.WriteLogEntry("SQL error " + ex.Message);
                }
                catch (Exception ex)
                {
                    Log.WriteLogEntry("General program error " + ex.Message);
                }
            }
            Log.WriteLogEntry("Updated user departments " + result);
            Log.WriteLogEntry("End UpdateAllDepartment.");
            return result;
        }

        public int UpdateAllSupervisor()
        {
            Log.WriteLogEntry("Begin UpdateAllSupervisor...");
            int result = 0;
            string depString = string.Format(@"update {0}.dbo.user_account set supervisor = @supName where employee_id = @empID", dbName);
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                try
                {
                    conn.Open();
                    Log.WriteLogEntry("Open SQL connection successful.");
                    foreach (User ADUser in Users)
                    {
                        if (ADUser.EmployeeID != null)
                        {
                            if (ADUser.SupervisorName != null)
                            {
                                using (SqlCommand depCmd = new SqlCommand(depString, conn))
                                {
                                    depCmd.Parameters.AddWithValue("@supName", ADUser.SupervisorName);
                                    depCmd.Parameters.AddWithValue("@empID", ADUser.EmployeeID);
                                    depCmd.ExecuteNonQuery();
                                    result++;
                                }
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Log.WriteLogEntry("SQL error " + ex.Message);
                }
                catch (Exception ex)
                {
                    Log.WriteLogEntry("General program error " + ex.Message);
                }
            }
            Log.WriteLogEntry("Updated user supervisors " + result);
            Log.WriteLogEntry("End UpdateAllSupervisor.");
            return result;
        }
    }
}