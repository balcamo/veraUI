using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using VeraAPI.Models.Security;
using VeraAPI.Models.Tools;

namespace VeraAPI.Models.DataHandler
{
    public class UserDataHandler : SQLDataHandler
    {
        public User CurrentUser { get; set; }
        public List<User> Users { get; private set; }

        private Scribe log;

        public UserDataHandler(User user, string dbServer, string dbName) : base(dbServer)
        {
            this.dbServer = dbServer;
            this.dbName = dbName;
            this.dataConnectionString = GetDataConnectionString();
            this.log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UserDataHandler_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            CurrentUser = user;
        }

        public bool LoadDomainLoginUser()
        {
            log.WriteLogEntry("Begin LoadUser...");
            bool result = false;
            if (CurrentUser.GetType() == typeof(DomainUser))
            {
                log.WriteLogEntry("Success user type is domain user.");
                DomainUser user = (DomainUser)CurrentUser;
                string cmdString = string.Format(@"select * from {0}.dbo.user_header where user_email = @email", dbName);
                using (SqlConnection conn = new SqlConnection(dataConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                    {
                        try
                        {
                            conn.Open();
                            cmd.Parameters.AddWithValue("@email", user.UserEmail);
                            log.WriteLogEntry(cmdString);
                            using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                            {
                                if (rdr.Read())
                                {
                                    user.UserID = rdr["user_id_token"].ToString();
                                    user.FirstName = rdr["first_name"].ToString();
                                    user.LastName = rdr["last_name"].ToString();
                                    user.UserEmail = rdr["user_email"].ToString();
                                    CurrentUser = user;
                                    result = true;
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            log.WriteLogEntry("SQL error " + ex.Message);
                        }
                        catch (Exception ex)
                        {
                            log.WriteLogEntry("General program error " + ex.Message);
                        }
                    }
                }
            }
            else
                log.WriteLogEntry("Failed user type is not domain user!");
            log.WriteLogEntry("End LoadUser.");
            return result;
        }

        public bool LoadUserData()
        {
            log.WriteLogEntry("Begin LoadUserData...");
            bool result = false;
            string cmdString = string.Format(@"select * from {0}.dbo.user_header where user_email = @email", dbName);
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@email", CurrentUser.UserEmail);
                        log.WriteLogEntry(cmdString);
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                        {
                            if (rdr.Read())
                            {
                                CurrentUser.UserID = rdr["user_id_token"].ToString();
                                CurrentUser.FirstName = rdr["first_name"].ToString();
                                CurrentUser.LastName = rdr["last_name"].ToString();
                                CurrentUser.UserEmail = rdr["email"].ToString();
                                result = true;
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        log.WriteLogEntry("SQL error " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        log.WriteLogEntry("General program error " + ex.Message);
                    }
                }
            }
            log.WriteLogEntry("End LoadUserData.");
            return result;
        }

        public bool LoadPublicLoginUser()
        {
            log.WriteLogEntry("Begin LoadUser...");
            bool result = false;
            if (CurrentUser.GetType() == typeof(DomainUser))
            {
                log.WriteLogEntry("Success user type is domain user.");
                DomainUser user = (DomainUser)CurrentUser;
                string cmdString = string.Format(@"select * from {0}.dbo.user_header where user_upn = @upn", dbName);
                using (SqlConnection conn = new SqlConnection(dataConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                    {
                        try
                        {
                            conn.Open();
                            cmd.Parameters.AddWithValue("@upn", user.UserEmail);
                            log.WriteLogEntry(cmdString);
                            using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                            {
                                if (rdr.Read())
                                {
                                    // SINCE WE CHANGED VARIABLE NAMES DO WE NEED TO CHANGE THESE???
                                    // Database column names will remain descriptive for now and use schema field mapping
                                    user.UserID = rdr["user_id_token"].ToString();
                                    user.FirstName = rdr["first_name"].ToString();
                                    user.LastName = rdr["last_name"].ToString();
                                    user.UserEmail = rdr["email"].ToString();
                                    user.DomainUserName = rdr["user_sam"].ToString();
                                    user.DomainUpn = rdr["user_upn"].ToString();
                                    user.EmployeeID = rdr["employee_id"].ToString();
                                    user.Department = rdr["department"].ToString();
                                    user.SupervisorName = rdr["supervisor"].ToString();
                                    CurrentUser = user;
                                    result = true;
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            log.WriteLogEntry("SQL error " + ex.Message);
                        }
                        catch (Exception ex)
                        {
                            log.WriteLogEntry("General program error " + ex.Message);
                        }
                    }
                }
            }
            else
                log.WriteLogEntry("Failed user type is not domain user!");
            log.WriteLogEntry("End LoadUser.");
            return result;
        }

        public bool LoadFieldUserData()
        {
            log.WriteLogEntry("Begin LoadUser...");
            bool result = false;
            if (CurrentUser.GetType() == typeof(DomainUser))
            {
                log.WriteLogEntry("Success user type is domain user.");
                DomainUser user = (DomainUser)CurrentUser;
                string cmdString = string.Format(@"select * from {0}.dbo.user_header where user_upn = @upn", dbName);
                using (SqlConnection conn = new SqlConnection(dataConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                    {
                        try
                        {
                            conn.Open();
                            cmd.Parameters.AddWithValue("@upn", user.UserEmail);
                            log.WriteLogEntry(cmdString);
                            using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                            {
                                if (rdr.Read())
                                {
                                    user.UserID = rdr["user_id_token"].ToString();
                                    user.FirstName = rdr["first_name"].ToString();
                                    user.LastName = rdr["last_name"].ToString();
                                    user.UserEmail = rdr["email"].ToString();
                                    CurrentUser = user;
                                    result = true;
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            log.WriteLogEntry("SQL error " + ex.Message);
                        }
                        catch (Exception ex)
                        {
                            log.WriteLogEntry("General program error " + ex.Message);
                        }
                    }
                }
            }
            else
                log.WriteLogEntry("Failed user type is not domain user!");
            log.WriteLogEntry("End LoadUser.");
            return result;
        }

        public int LoadAllUserData()
        {
            log.WriteLogEntry("Begin LoadAllUsers...");
            int result = 0;
            string cmdString = string.Format(@"select * from {0}.dbo.user_header", dbName);
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
                                DomainUser user = new DomainUser();
                                user.UserID = rdr["user_id_token"].ToString();
                                user.UserName = rdr["username"].ToString();
                                user.UserType = (int)rdr["user_type"];
                                user.UserEmail = rdr["email"].ToString();
                                user.FirstName = rdr["first_name"].ToString();
                                user.LastName = rdr["last_name"].ToString();
                                Users.Add(user);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        log.WriteLogEntry("SQL error " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        log.WriteLogEntry("General program error " + ex.Message);
                    }
                }
            }
            result = Users.Count;
            log.WriteLogEntry("End LoadAllUsers.");
            return result;
        }

        public bool FillDepartmentHead()
        {
            log.WriteLogEntry("Begin FillDepartmentHead...");
            bool result = false;
            if (CurrentUser.GetType() == typeof(DomainUser))
            {
                log.WriteLogEntry("Success user type is domain user.");
                DomainUser user = (DomainUser)CurrentUser;
                string depString = string.Format(@"select dept_head_emp_id from {0}.dbo.department where dept_name = @deptName", dbName);
                // SINCE WE CHANGED VARIABLE NAMES DO WE NEED TO CHANGE THESE???
                // Database column names will remain descriptive for now and use schema field mapping
                string empString = string.Format(@"select first_name, last_name, user_email from {0}.dbo.user_header where employee_id = @empID", dbName);
                using (SqlConnection conn = new SqlConnection(dataConnectionString))
                {
                    try
                    {
                        conn.Open();
                        using (SqlCommand deptCmd = new SqlCommand(depString, conn))
                        {
                            deptCmd.Parameters.AddWithValue("@deptName", user.Department);
                            var emp = deptCmd.ExecuteScalar();
                            if (emp != null)
                            {
                                user.DepartmentHeadEmpID = emp.ToString();
                                result = true;
                            }
                        }
                        if (result)
                        {
                            using (SqlCommand empCmd = new SqlCommand(empString, conn))
                            {
                                empCmd.Parameters.AddWithValue("@empID", user.DepartmentHeadEmpID);
                                using (SqlDataReader rdr = empCmd.ExecuteReader(CommandBehavior.SingleResult))
                                {
                                    if (rdr.Read())
                                    {
                                        string empName = string.Format("{0} {1}", rdr["first_name"].ToString(), rdr["last_name"].ToString());
                                        user.DepartmentHead = empName;
                                        user.DepartmentHeadEmail = rdr["email"].ToString();
                                        CurrentUser = user;
                                        result = true;
                                    }
                                }
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        log.WriteLogEntry("SQL error " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        log.WriteLogEntry("General program error " + ex.Message);
                    }
                }
            }
            else
                log.WriteLogEntry("Failed not a domain user!");
            log.WriteLogEntry("End FillDepartmentHead.");
            return result;
        }

        public bool FillGeneralManager()
        {
            log.WriteLogEntry("Begin FillGeneralManager...");
            bool result = false;
            if (CurrentUser.GetType() == typeof(DomainUser))
            {
                log.WriteLogEntry("Success user type is domain user.");
                DomainUser user = (DomainUser)CurrentUser;
                string comString = string.Format(@"select general_manager_emp_id from {0}.dbo.company", dbName);
                // SINCE WE CHANGED VARIABLE NAMES DO WE NEED TO CHANGE THESE???
                // Database column names will remain descriptive for now and use schema field mapping
                string empString = string.Format(@"select first_name, last_name, user_email from {0}.dbo.user_header where employee_id = @empID", dbName);
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
                                user.GeneralManagerEmpID = emp.ToString();
                                result = true;
                            }
                        }
                        if (result)
                        {
                            using (SqlCommand empCmd = new SqlCommand(empString, conn))
                            {
                                empCmd.Parameters.AddWithValue("@empID", user.GeneralManagerEmpID);
                                using (SqlDataReader rdr = empCmd.ExecuteReader(CommandBehavior.SingleResult))
                                {
                                    if (rdr.Read())
                                    {
                                        string empName = string.Format("{0} {1}", rdr["first_name"].ToString(), rdr["last_name"].ToString());
                                        user.GeneralManager = empName;
                                        user.GeneralManagerEmail = rdr["email"].ToString();
                                        CurrentUser = user;
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
                        log.WriteLogEntry("SQL error " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        log.WriteLogEntry("General program error " + ex.Message);
                    }
                }
            }
            else
                log.WriteLogEntry("Failed not a domain user!");
            log.WriteLogEntry("End FillGeneralManager.");
            return result;
        }

        public bool FillUserID()
        {
            log.WriteLogEntry("Begin FillUserID...");
            bool result = false;
            string cmdString = string.Format(@"select user_token from {0}.dbo.user_header where user_email = @email", dbName);
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@email", CurrentUser.UserEmail);
                        log.WriteLogEntry(cmdString);
                        CurrentUser.UserID = cmd.ExecuteScalar().ToString();
                        if (CurrentUser.UserID != null)
                            result = true;
                        else
                            log.WriteLogEntry("Failed getting user id from database!");
                    }
                    catch (SqlException ex)
                    {
                        log.WriteLogEntry("SQL error " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        log.WriteLogEntry("General program error " + ex.Message);
                    }
                }
            }
            log.WriteLogEntry("End FillUserID.");
            return result;
        }

        public bool InsertLoginUser()
        {
            log.WriteLogEntry("Begin InsertLoginUser...");
            bool result = false;
            // SINCE WE CHANGED VARIABLE NAMES DO WE NEED TO CHANGE THESE???
            // Database column names will remain descriptive for now and use schema field mapping
            string cmdString = string.Format(@"insert into {0}.dbo.user_session (user_id, first_name, last_name, user_email, authenticated, user_type, login_name, login_token)
                                            values (@userID, @firstName, @lastName, @email, @auth, @userType, @loginName, @token)", dbName);
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                try
                {
                    conn.Open();
                    log.WriteLogEntry("Open SQL connection successful.");
                    using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                    {
                        User user = CurrentUser;
                        log.WriteLogEntry("Domain user " + user.FirstName + " " + user.LastName + " ");
                        cmd.Parameters.AddWithValue("@userID", user.UserID);
                        cmd.Parameters.AddWithValue("@firstName", user.FirstName);
                        cmd.Parameters.AddWithValue("@lastName", user.LastName);
                        cmd.Parameters.AddWithValue("@email", user.UserEmail);
                        cmd.Parameters.AddWithValue("@auth", user.Authenicated);
                        cmd.Parameters.AddWithValue("@userType", user.UserType);
                        cmd.Parameters.AddWithValue("@loginName", user.UserName);
                        cmd.Parameters.AddWithValue("@token", user.SessionToken);
                        if (cmd.ExecuteNonQuery() > 0)
                            result = true;
                    }
                }
                catch (SqlException ex)
                {
                    log.WriteLogEntry("SQL error " + ex.Message);
                }
                catch (Exception ex)
                {
                    log.WriteLogEntry("General program error " + ex.Message);
                }
            }
            log.WriteLogEntry("End InsertLoginUser.");
            return result;
        }

        public bool InsertDomainLoginUser()
        {
            log.WriteLogEntry("Begin InsertDomainLoginUser...");
            bool result = false;
            if (CurrentUser.GetType() == typeof(DomainUser))
            {
                DomainUser user = (DomainUser)CurrentUser;
                string cmdString = string.Format(@"insert into {0}.dbo.user_session (user_id, domain_username, domain_upn, user_employee_id, first_name, last_name, user_email, user_department, authenticated, user_type, login_name, login_token)
                                            values (@userID, @userName, @upn, @empID, @firstName, @lastName, @email, @dept, @auth, @userType, @loginName, @token)", dbName);
                log.WriteLogEntry(string.Format("Data connection string {0}", dataConnectionString));
                using (SqlConnection conn = new SqlConnection(dataConnectionString))
                {
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                        {
                            log.WriteLogEntry(string.Format("Domain user {0} {1} {2} {3}", user.FirstName, user.LastName, user.DomainUserName, user.DomainUpn));
                            cmd.Parameters.AddWithValue("@userID", user.UserID);
                            cmd.Parameters.AddWithValue("@userName", user.DomainUserName);
                            cmd.Parameters.AddWithValue("@upn", user.DomainUpn);
                            cmd.Parameters.AddWithValue("@empID", user.EmployeeID);
                            cmd.Parameters.AddWithValue("@firstName", user.FirstName);
                            cmd.Parameters.AddWithValue("@lastName", user.LastName);
                            cmd.Parameters.AddWithValue("@email", user.UserEmail);
                            cmd.Parameters.AddWithValue("@dept", user.Department);
                            cmd.Parameters.AddWithValue("@auth", user.Authenicated);
                            cmd.Parameters.AddWithValue("@userType", user.UserType);
                            cmd.Parameters.AddWithValue("@loginName", user.UserName);
                            cmd.Parameters.AddWithValue("@token", user.SessionToken);
                            if (cmd.ExecuteNonQuery() > 0)
                                result = true;
                        }
                    }
                    catch (SqlException ex)
                    {
                        log.WriteLogEntry("SQL error " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        log.WriteLogEntry("General program error " + ex.Message);
                    }
                }
            }
            else
                log.WriteLogEntry("Failed user type is not domain user!");
            log.WriteLogEntry("End InsertDomainLoginUser.");
            return result;
        }

        public int InsertAllUsers()
        {
            log.WriteLogEntry("Begin InsertAllUsers...");
            int result = 0;
            string cmdString = string.Format(@"insert into {0}.dbo.user_header (first_name, last_name, user_email, user_sam, user_upn, employee_id)
                                            values (@firstName, @lastName, @email, @sam, @upn, @empID)", dbName);
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                try
                {
                    conn.Open();
                    log.WriteLogEntry("Open SQL connection successful.");
                    foreach (DomainUser user in Users)
                    {
                        if (user.EmployeeID != null)
                        {
                            using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                            {
                                log.WriteLogEntry("AD User " + user.FirstName + " " + user.LastName + " " + user.DomainUpn + " " + user.EmployeeID);
                                cmd.Parameters.AddWithValue("@firstName", user.FirstName);
                                cmd.Parameters.AddWithValue("@lastName", user.LastName);
                                cmd.Parameters.AddWithValue("@email", user.UserEmail);
                                cmd.Parameters.AddWithValue("@sam", user.DomainUserName);
                                cmd.Parameters.AddWithValue("@upn", user.DomainUpn);
                                cmd.Parameters.AddWithValue("@empID", user.EmployeeID);
                                cmd.ExecuteNonQuery();
                                result++;
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    log.WriteLogEntry("SQL error " + ex.Message);
                }
                catch (Exception ex)
                {
                    log.WriteLogEntry("General program error " + ex.Message);
                }
            }
            log.WriteLogEntry("Inserted users " + result);
            log.WriteLogEntry("End InsertAllUsers.");
            return result;
        }

        public int UpdateAllDepartment()
        {
            log.WriteLogEntry("Begin UpdateAllDepartment...");
            int result = 0;
            string depString = string.Format(@"update {0}.dbo.user_header set department = @depName where employee_id = @empID", dbName);
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                try
                {
                    conn.Open();
                    log.WriteLogEntry("Open SQL connection successful.");
                    foreach (DomainUser ADUser in Users)
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
                    log.WriteLogEntry("SQL error " + ex.Message);
                }
                catch (Exception ex)
                {
                    log.WriteLogEntry("General program error " + ex.Message);
                }
            }
            log.WriteLogEntry("Updated user departments " + result);
            log.WriteLogEntry("End UpdateAllDepartment.");
            return result;
        }

        public int UpdateAllSupervisor()
        {
            log.WriteLogEntry("Begin UpdateAllSupervisor...");
            int result = 0;
            string depString = string.Format(@"update {0}.dbo.user_header set supervisor = @supName where employee_id = @empID", dbName);
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                try
                {
                    conn.Open();
                    log.WriteLogEntry("Open SQL connection successful.");
                    foreach (DomainUser ADUser in Users)
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
                    log.WriteLogEntry("SQL error " + ex.Message);
                }
                catch (Exception ex)
                {
                    log.WriteLogEntry("General program error " + ex.Message);
                }
            }
            log.WriteLogEntry("Updated user supervisors " + result);
            log.WriteLogEntry("End UpdateAllSupervisor.");
            return result;
        }
    }
}