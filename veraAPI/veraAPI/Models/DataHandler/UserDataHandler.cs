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

        public UserDataHandler(string dbServer, string dbName) : base(dbServer)
        {
            this.dbServer = dbServer;
            this.dbName = dbName;
            this.dataConnectionString = GetDataConnectionString();
            this.log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UserDataHandler_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            CurrentUser = new User();
        }

        public bool LoadLoginUser(string email)
        {
            log.WriteLogEntry("Begin LoadLoginUser...");
            bool result = false;
            string cmdString = string.Format(@"select * from {0}.dbo.user_session where user_email = @email", dbName);

            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@email", email);
                        log.WriteLogEntry(cmdString);
                        log.WriteLogEntry("Email " + email);
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                        {
                            if (rdr.Read())
                            {
                                if (CurrentUser.GetType() == typeof(DomainUser))
                                {
                                    log.WriteLogEntry("User type is domain user.");
                                    DomainUser user = (DomainUser)CurrentUser;
                                    log.WriteLogEntry("User ID " + rdr["user_id"].ToString());
                                    user.UserID = (int)rdr["user_id"];
                                    log.WriteLogEntry("UserName " + rdr["username"].ToString());
                                    user.UserName = rdr["username"].ToString();
                                    log.WriteLogEntry("First Name " + rdr["first_name"].ToString());
                                    user.FirstName = rdr["first_name"].ToString();
                                    log.WriteLogEntry("Last Name " + rdr["last_name"].ToString());
                                    user.LastName = rdr["last_name"].ToString();
                                    log.WriteLogEntry("User Email " + rdr["user_email"].ToString());
                                    user.UserEmail = rdr["user_email"].ToString();
                                    log.WriteLogEntry("User Type " + rdr["user_type"].ToString());
                                    user.UserType = (int)rdr["user_type"];
                                    log.WriteLogEntry("Employee ID " + rdr["user_employee_id"].ToString());
                                    user.EmployeeID = rdr["user_employee_id"].ToString();
                                    log.WriteLogEntry("Department " + rdr["user_department"].ToString());
                                    user.Department = rdr["user_department"].ToString();
                                    log.WriteLogEntry("Dept Head " + rdr["dept_head_name"].ToString());
                                    user.DepartmentHead = rdr["dept_head_name"].ToString();
                                    log.WriteLogEntry("Dept Head Email " + rdr["dept_head_email"].ToString());
                                    user.DepartmentHeadEmail = rdr["dept_head_email"].ToString();
                                    log.WriteLogEntry("Supervisor " + rdr["user_supervisor"].ToString());
                                    user.SupervisorName = rdr["user_supervisor"].ToString();
                                    log.WriteLogEntry("Authenticated " + rdr["authenticated"].ToString());
                                    user.Authenicated = (bool)rdr["authenticated"];
                                    log.WriteLogEntry("Domain UPN " + rdr["domain_upn"].ToString());
                                    user.DomainUpn = rdr["domain_upn"].ToString();
                                    log.WriteLogEntry("Domain UserName " + rdr["domain_username"].ToString());
                                    user.DomainUserName = rdr["domain_username"].ToString();
                                    result = true;
                                }
                                else
                                {
                                    log.WriteLogEntry("User type is public user.");
                                    User user = CurrentUser;
                                    log.WriteLogEntry("User ID " + rdr["user_id"].ToString());
                                    user.UserID = (int)rdr["user_id"];
                                    log.WriteLogEntry("UserName " + rdr["username"].ToString());
                                    user.UserName = rdr["username"].ToString();
                                    log.WriteLogEntry("First Name " + rdr["first_name"].ToString());
                                    user.FirstName = rdr["first_name"].ToString();
                                    log.WriteLogEntry("Last Name " + rdr["last_name"].ToString());
                                    user.LastName = rdr["last_name"].ToString();
                                    log.WriteLogEntry("User Email " + rdr["user_email"].ToString());
                                    user.UserEmail = rdr["user_email"].ToString();
                                    log.WriteLogEntry("User Type " + rdr["user_type"].ToString());
                                    user.UserType = (int)rdr["user_type"];
                                    result = true;
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
            log.WriteLogEntry(string.Format("Current User {0} {1} {2}", CurrentUser.UserID, CurrentUser.UserEmail, CurrentUser.UserType));
            log.WriteLogEntry("End LoadLoginUser.");
            return result;
        }

        public bool LoadUserData(string email)
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
                        cmd.Parameters.AddWithValue("@email", email);
                        log.WriteLogEntry(cmdString);
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                        {
                            if (rdr.Read())
                            {
                                User user = CurrentUser;
                                log.WriteLogEntry("User ID " + rdr["user_id"].ToString());
                                user.UserID = (int)rdr["user_id"];
                                log.WriteLogEntry("UserName " + rdr["username"].ToString());
                                user.UserName = rdr["username"].ToString();
                                log.WriteLogEntry("First Name " + rdr["first_name"].ToString());
                                user.FirstName = rdr["first_name"].ToString();
                                log.WriteLogEntry("Last Name " + rdr["last_name"].ToString());
                                user.LastName = rdr["last_name"].ToString();
                                log.WriteLogEntry("User Email " + rdr["user_email"].ToString());
                                user.UserEmail = rdr["user_email"].ToString();
                                log.WriteLogEntry("User Type " + rdr["user_type"].ToString());
                                user.UserType = (int)rdr["user_type"];
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

        public int LoadAllUserData()
        {
            log.WriteLogEntry("Begin LoadAllUserData...");
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
                                User user = new User();
                                log.WriteLogEntry("User ID " + rdr["user_id"].ToString());
                                user.UserID = (int)rdr["user_id"];
                                log.WriteLogEntry("UserName " + rdr["username"].ToString());
                                user.UserName = rdr["username"].ToString();
                                log.WriteLogEntry("First Name " + rdr["first_name"].ToString());
                                user.FirstName = rdr["first_name"].ToString();
                                log.WriteLogEntry("Last Name " + rdr["last_name"].ToString());
                                user.LastName = rdr["last_name"].ToString();
                                log.WriteLogEntry("User Email " + rdr["user_email"].ToString());
                                user.UserEmail = rdr["user_email"].ToString();
                                log.WriteLogEntry("User Type " + rdr["user_type"].ToString());
                                user.UserType = (int)rdr["user_type"];
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
            log.WriteLogEntry("End LoadAllUserData.");
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
                string depString = string.Format(@"select dept_head_emp_id, dept_head_name, dept_head_email from {0}.dbo.department where dept_name = @deptName", dbName);
                // SINCE WE CHANGED VARIABLE NAMES DO WE NEED TO CHANGE THESE???
                // Database column names will remain descriptive for now and use schema field mapping
                using (SqlConnection conn = new SqlConnection(dataConnectionString))
                {
                    try
                    {
                        conn.Open();
                        using (SqlCommand deptCmd = new SqlCommand(depString, conn))
                        {
                            deptCmd.Parameters.AddWithValue("@deptName", user.Department);
                            using (SqlDataReader rdr = deptCmd.ExecuteReader(CommandBehavior.SingleResult))
                            {
                                if (rdr.Read())
                                {
                                    user.DepartmentHeadEmpID = rdr["dept_head_emp_id"].ToString();
                                    user.DepartmentHead = rdr["dept_head_name"].ToString();
                                    user.DepartmentHeadEmail = rdr["dept_head_email"].ToString();
                                    result = true;
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
            string cmdString = string.Format(@"select user_id from {0}.dbo.user_header where user_email = @email", dbName);
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@email", CurrentUser.UserEmail);
                        log.WriteLogEntry(cmdString);
                        int userID = (int)cmd.ExecuteScalar();
                        if (userID > 0)
                        {
                            User user = CurrentUser;
                            user.UserID = userID;
                            result = true;
                        }
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

            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                try
                {
                    conn.Open();
                    if (CurrentUser.GetType() == typeof(DomainUser))
                    {
                        DomainUser user = (DomainUser)CurrentUser;
                        string cmdString = string.Format(@"insert into {0}.dbo.user_session (user_id, domain_username, domain_upn, user_employee_id, first_name, last_name, user_email, user_department, dept_head_name, dept_head_email, authenticated, user_type, username, session_key)
                                            values (@userID, @domainUser, @upn, @empID, @firstName, @lastName, @email, @dept, @deptHead, @deptHeadEmail, @auth, @userType, @userName, @key)", dbName);
                        using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                        {
                            log.WriteLogEntry(string.Format("Domain user {0} {1} {2} {3}", user.FirstName, user.LastName, user.DomainUserName, user.DomainUpn));
                            cmd.Parameters.AddWithValue("@userID", user.UserID);
                            cmd.Parameters.AddWithValue("@domainUser", user.DomainUserName);
                            cmd.Parameters.AddWithValue("@upn", user.DomainUpn);
                            cmd.Parameters.AddWithValue("@empID", user.EmployeeID);
                            cmd.Parameters.AddWithValue("@firstName", user.FirstName);
                            cmd.Parameters.AddWithValue("@lastName", user.LastName);
                            cmd.Parameters.AddWithValue("@email", user.UserEmail);
                            cmd.Parameters.AddWithValue("@dept", user.Department);
                            cmd.Parameters.AddWithValue("@deptHead", user.DepartmentHead);
                            cmd.Parameters.AddWithValue("@deptHeadEmail", user.DepartmentHeadEmail);
                            cmd.Parameters.AddWithValue("@auth", user.Authenicated);
                            cmd.Parameters.AddWithValue("@userType", user.UserType);
                            cmd.Parameters.AddWithValue("@userName", user.UserName);
                            cmd.Parameters.AddWithValue("@key", user.Token.SessionKey);
                            if (cmd.ExecuteNonQuery() > 0)
                                result = true;
                        }
                    }
                    else
                    {
                        User user = CurrentUser;
                        string cmdString = string.Format(@"insert into {0}.dbo.user_session (user_id, first_name, last_name, user_email, authenticated, user_type, username, session_key)
                                            values (@userID, @firstName, @lastName, @email, @auth, @userType, @userName, @key)", dbName);
                        using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                        {
                            log.WriteLogEntry(string.Format("User {0} {1} {2} {3}", user.FirstName, user.LastName, user.UserName, user.UserEmail));
                            cmd.Parameters.AddWithValue("@userID", user.UserID);
                            cmd.Parameters.AddWithValue("@firstName", user.FirstName);
                            cmd.Parameters.AddWithValue("@lastName", user.LastName);
                            cmd.Parameters.AddWithValue("@email", user.UserEmail);
                            cmd.Parameters.AddWithValue("@auth", user.Authenicated);
                            cmd.Parameters.AddWithValue("@userType", user.UserType);
                            cmd.Parameters.AddWithValue("@userName", user.UserName);
                            cmd.Parameters.AddWithValue("@key", user.Token.SessionKey);
                            if (cmd.ExecuteNonQuery() > 0)
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
            log.WriteLogEntry("End InsertLoginUser.");
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