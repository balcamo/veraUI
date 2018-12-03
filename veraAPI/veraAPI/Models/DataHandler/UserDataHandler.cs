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
        public UserSession CurrentSession { get; set; }

        private static Scribe log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UserDataHandler_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");

        public UserDataHandler(User user, string dbServer, string dbName) : base(dbServer)
        {
            this.dbServer = dbServer;
            this.dbName = dbName;
            this.dataConnectionString = GetDataConnectionString();
            this.CurrentUser = user;
        }

        public UserDataHandler(UserSession session, string dbServer, string dbName) : base(dbServer)
        {
            this.dbServer = dbServer;
            this.dbName = dbName;
            this.dataConnectionString = GetDataConnectionString();
            this.CurrentSession = session;
        }

        public UserDataHandler(string dbServer, string dbName) : base(dbServer)
        {
            this.dbServer = dbServer;
            this.dbName = dbName;
            this.dataConnectionString = GetDataConnectionString();
            this.CurrentUser = new User();
            this.CurrentSession = new UserSession();
        }

        public bool LoadUserSession(int userID)
        {
            log.WriteLogEntry("Begin LoadUserSession...");
            bool result = false;
            string cmdString = string.Format(@"select * from {0}.dbo.user_session where user_id = @userID", dbName);
            UserSession session = this.CurrentSession;

            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@userID", userID);
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                        {
                            if (rdr.Read())
                            {
                                log.WriteLogEntry("UserID " + rdr["user_id"].ToString());
                                session.UserID = (int)rdr["user_id"];
                                log.WriteLogEntry("Company number " + rdr["company_number"].ToString());
                                session.CompanyNumber = (int)rdr["company_number"];
                                log.WriteLogEntry("Department number " + rdr["dept_number"].ToString());
                                session.CompanyNumber = (int)rdr["dept_number"];
                                log.WriteLogEntry("Position number " + rdr["position_number"].ToString());
                                session.CompanyNumber = (int)rdr["position_number"];
                                log.WriteLogEntry("Role number " + rdr["role_number"].ToString());
                                session.CompanyNumber = (int)rdr["role_number"];
                                log.WriteLogEntry("Domain number " + rdr["domain_number"].ToString());
                                session.CompanyNumber = (int)rdr["domain_number"];
                                log.WriteLogEntry("UserName " + rdr["username"].ToString());
                                session.UserName = rdr["username"].ToString();
                                log.WriteLogEntry("User Email " + rdr["user_email"].ToString());
                                session.UserEmail = rdr["user_email"].ToString();
                                log.WriteLogEntry("First Name " + rdr["first_name"].ToString());
                                session.FirstName = rdr["first_name"].ToString();
                                log.WriteLogEntry("Last Name " + rdr["last_name"].ToString());
                                session.LastName = rdr["last_name"].ToString();
                                log.WriteLogEntry("Employee ID " + rdr["user_employee_id"].ToString());
                                session.EmployeeID = rdr["user_employee_id"].ToString();
                                log.WriteLogEntry("Department name " + rdr["dept_name"].ToString());
                                session.DeptName = rdr["dept_name"].ToString();
                                log.WriteLogEntry("Department head email" + rdr["dept_head_email"].ToString());
                                session.DeptHeadEmail = rdr["dept_head_email"].ToString();
                                log.WriteLogEntry("Domain UPN " + rdr["domain_upn"].ToString());
                                session.DomainUpn = rdr["domain_upn"].ToString();
                                log.WriteLogEntry("Domain UserName " + rdr["domain_username"].ToString());
                                session.DomainUserName = rdr["domain_username"].ToString();
                                log.WriteLogEntry("Session key " + rdr["session_key"].ToString());
                                session.SessionKey = rdr["session_key"].ToString();
                                log.WriteLogEntry("Authenticated " + rdr["authenticated"].ToString());
                                session.Authenicated = (bool)rdr["authenticated"];
                                result = true;
                            }
                            else
                                log.WriteLogEntry("FAILED no user session found in database!");
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
            log.WriteLogEntry(string.Format("Current Session {0} {1} {2}", session.UserID, session.UserEmail, session.Authenicated));
            log.WriteLogEntry("End LoadUserSession.");
            return result;
        }

        public bool LoadUserData(int userID)
        {
            log.WriteLogEntry("Begin LoadUserData...");
            bool result = false;
            string cmdString = string.Format(@"select * from {0}.dbo.user_header where user_id = @userID", dbName);
            User user = this.CurrentUser;

            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@userID", userID);
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                        {
                            if (rdr.Read())
                            {
                                log.WriteLogEntry("User ID " + rdr["user_id"].ToString());
                                user.UserID = (int)rdr["user_id"];
                                log.WriteLogEntry("Company number " + rdr["company_number"].ToString());
                                user.CompanyNumber = (int)rdr["company_number"];
                                log.WriteLogEntry("Department number " + rdr["dept_number"].ToString());
                                user.DepartmentNumber = (int)rdr["dept_number"];
                                log.WriteLogEntry("Position number " + rdr["position_number"].ToString());
                                user.PositionNumber = (int)rdr["position_number"];
                                log.WriteLogEntry("Username " + rdr["username"].ToString());
                                user.UserName = rdr["username"].ToString();
                                log.WriteLogEntry("User Email " + rdr["user_email"].ToString());
                                user.UserEmail = rdr["user_email"].ToString();
                                log.WriteLogEntry("First Name " + rdr["first_name"].ToString());
                                user.FirstName = rdr["first_name"].ToString();
                                log.WriteLogEntry("Last Name " + rdr["last_name"].ToString());
                                user.LastName = rdr["last_name"].ToString();
                                log.WriteLogEntry("Employee ID " + rdr["employee_id"]);
                                user.EmployeeID = rdr["employee_id"].ToString();
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

        public bool LoadDepartment()
        {
            log.WriteLogEntry("Begin LoadDepartment...");
            bool result = false;
            if (CurrentUser.GetType() == typeof(DomainUser))
            {
                DomainUser user = (DomainUser)CurrentUser;
                log.WriteLogEntry("Loading department number " + user.DepartmentNumber);
                string cmdString = string.Format(@"select * from {0}.dbo.department where dept_number = @deptNumber", dbName);

                using (SqlConnection conn = new SqlConnection(dataConnectionString))
                {
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                        {
                            cmd.Parameters.AddWithValue("@deptNumber", user.DepartmentNumber);
                            using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                            {
                                if (rdr.Read())
                                {
                                    user.Department.DeptNumber = (int)rdr["dept_number"];
                                    user.Department.DeptName = rdr["dept_name"].ToString();
                                    user.Department.DeptHeadUserID = (int)rdr["dept_head_user_id"];
                                    user.Department.DeptEmail = rdr["dept_email"].ToString();
                                    user.Department.DeptHeadEmail = rdr["dept_head_email"].ToString();
                                    log.WriteLogEntry(string.Format("Department loaded {0} {1} {2} {3}", user.Department.DeptNumber, user.Department.DeptName, user.Department.DeptHeadUserID, user.Department.DeptEmail));
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
                log.WriteLogEntry("FAILED not a domain user!");
            log.WriteLogEntry("End LoadDepartment.");
            return result;
        }

        public Department GetDepartment(int deptNumber)
        {
            log.WriteLogEntry("Begin LoadDepartment...");
            Department department = new Department();
            log.WriteLogEntry("Loading department number " + deptNumber);
            string cmdString = string.Format(@"select * from {0}.dbo.department where dept_number = @deptNumber", dbName);

            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                    {
                        cmd.Parameters.AddWithValue("@deptNumber", deptNumber);
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                        {
                            if (rdr.Read())
                            {
                                department.DeptNumber = (int)rdr["dept_number"];
                                department.DeptName = rdr["dept_name"].ToString();
                                department.DeptHeadUserID = (int)rdr["dept_head_user_id"];
                                department.DeptEmail = rdr["dept_email"].ToString();
                                department.DeptHeadEmail = rdr["dept_head_email"].ToString();
                                log.WriteLogEntry(string.Format("Department loaded {0} {1} {2} {3}", department.DeptNumber, department.DeptName, department.DeptHeadUserID, department.DeptEmail));
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
            log.WriteLogEntry("End LoadDepartment.");
            return department;
        }

        public bool LoadCompany()
        {
            log.WriteLogEntry("Begin LoadCompany...");
            bool result = false;
            User user = CurrentUser;
            string cmdString = string.Format(@"select * from {0}.dbo.company where company_number = @companyNumber", dbName);

            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                    {
                        cmd.Parameters.AddWithValue("@companyNumber", user.CompanyNumber);
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                        {
                            if (rdr.Read())
                            {
                                user.Company.CompanyNumber = (int)rdr["company_number"];
                                user.Company.CompanyName = rdr["company_name"].ToString();
                                user.Company.CompanyEmail = rdr["company_email"].ToString();
                                user.Company.CompanyAccessLevel = (int)rdr["company_access"];
                                user.Company.GeneralManagerUserID = (int)rdr["general_manager_user_id"];
                                user.Company.GeneralManagerEmail = rdr["general_manager_email"].ToString();
                                log.WriteLogEntry(string.Format("Company loaded {0} {1} {2} {3} {4} {5}", user.Company.CompanyNumber, user.Company.CompanyName, user.Company.CompanyEmail, user.Company.CompanyAccessLevel, user.Company.GeneralManagerUserID, user.Company.GeneralManagerEmail));
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
            log.WriteLogEntry("End LoadCompany.");
            return result;
        }

        public int LoadSecurityRoles()
        {
            log.WriteLogEntry("Begin LoadSecurityRoles...");
            int result = 0;
            if (CurrentUser.GetType() == typeof(DomainUser))
            {
                DomainUser user = (DomainUser)CurrentUser;
                user.SecurityRoles = new List<Role>();
                log.WriteLogEntry("Loading roles for user id " + user.UserID);
                string cmdString = string.Format(@"select srol.* from {0}.dbo.user_role as urol left join {0}.dbo.security_role as srol on urol.role_number = srol.role_number where urol.user_id = @userID", dbName);

                using (SqlConnection conn = new SqlConnection(dataConnectionString))
                {
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                        {
                            cmd.Parameters.AddWithValue("@userID", user.UserID);
                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                                while (rdr.Read())
                                {
                                    Role role = new Role
                                    {
                                        RoleNumber = (int)rdr["role_number"],
                                        RoleTitle = rdr["role_title"].ToString(),
                                        RoleDescription = rdr["role_description"].ToString()
                                    };
                                    log.WriteLogEntry(string.Format("Role loaded {0} {1} {2}", role.RoleNumber, role.RoleTitle, role.RoleDescription));
                                    user.SecurityRoles.Add(role);
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
                result = user.SecurityRoles.Count;
                log.WriteLogEntry("Loaded security roles " + result);
            }
            else
                log.WriteLogEntry("FAILED not a domain user!");
            log.WriteLogEntry("End LoadSecurityRoles.");
            return result;
        }

        public bool LoadPosition()
        {
            log.WriteLogEntry("Begin LoadPosition...");
            bool result = false;
            if (CurrentUser.GetType() == typeof(DomainUser))
            {
                DomainUser user = (DomainUser)CurrentUser;
                log.WriteLogEntry("Loading position number " + user.PositionNumber);
                string cmdString = string.Format(@"select * from {0}.dbo.position where position_number = @positionNumber", dbName);

                using (SqlConnection conn = new SqlConnection(dataConnectionString))
                {
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                        {
                            cmd.Parameters.AddWithValue("@positionNumber", user.PositionNumber);
                            using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                            {
                                if (rdr.Read())
                                {
                                    user.Position.PositionNumber = (int)rdr["position_number"];
                                    user.Position.PostiionTitle = rdr["position_title"].ToString();
                                    user.Position.PositionDescription = rdr["position_description"].ToString();
                                    log.WriteLogEntry(string.Format("Position loaded {0} {1} {2}", user.Position.PositionNumber, user.Position.PostiionTitle, user.Position.PositionDescription));
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
                log.WriteLogEntry("FAILED not a domain user!");
            log.WriteLogEntry("End LoadPosition.");
            return result;
        }

        public bool InsertUserSession(UserSession session)
        {
            log.WriteLogEntry("Begin InsertLoginUser...");
            bool result = false;

            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                try
                {
                    conn.Open();
                    string cmdString = string.Format(@"insert into {0}.dbo.user_session (user_id, company_number, dept_number, position_number, role_number, domain_number, 
	                                                    username, user_email, first_name, last_name, user_employee_id, dept_name, dept_head_email, 
	                                                    domain_upn, domain_username, session_key, authenticated, start_time)
                                                        values (@userID, @companyNumber, @deptNumber, @positionNumber, @roleNumber, @domainNumber, 
	                                                    @userName, @userEmail, @firstName, @lastName, @userEmpID, @deptName, @deptHeadEmail, 
	                                                    @domainUpn, @domainUserName, @sessionKey, @authenticated, @startTime)", dbName);
                    using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                    {
                        log.WriteLogEntry(string.Format("Domain user {0} {1} {2} {3}", session.FirstName, session.LastName, session.DomainUserName, session.DomainUpn));
                        cmd.Parameters.AddWithValue("@userID", session.UserID);
                        cmd.Parameters.AddWithValue("@companyNumber", session.CompanyNumber);
                        cmd.Parameters.AddWithValue("@deptNumber", session.DeptNumber);
                        cmd.Parameters.AddWithValue("@positionNumber", session.PositionNumber);
                        cmd.Parameters.AddWithValue("@roleNumber", session.RoleNumber);
                        cmd.Parameters.AddWithValue("@domainNumber", session.DeptNumber);
                        cmd.Parameters.AddWithValue("@userName", session.UserName);
                        cmd.Parameters.AddWithValue("@userEmail", session.UserEmail);
                        cmd.Parameters.AddWithValue("@firstName", session.FirstName);
                        cmd.Parameters.AddWithValue("@lastName", session.LastName);
                        cmd.Parameters.AddWithValue("@userEmpID", session.EmployeeID);
                        cmd.Parameters.AddWithValue("@deptName", session.DeptName);
                        //cmd.Parameters.AddWithValue("@deptHeadName", session.DeptHeadName);
                        cmd.Parameters.AddWithValue("@deptHeadEmail", session.DeptHeadEmail);
                        cmd.Parameters.AddWithValue("@domainUpn", session.DomainUpn);
                        cmd.Parameters.AddWithValue("@domainUserName", session.DomainUserName);
                        cmd.Parameters.AddWithValue("@sessionKey", session.SessionKey);
                        cmd.Parameters.AddWithValue("@authenticated", session.Authenicated);
                        cmd.Parameters.AddWithValue("@startTime", session.StartTime);
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

        public int GetUserID(string email)
        {
            log.WriteLogEntry("Begin GetUserID...");
            int result = 0;
            string cmdString = string.Format(@"select user_id from {0}.dbo.user_header where user_email = @email", dbName);

            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@email", email);
                        result = (int)cmd.ExecuteScalar();
                        log.WriteLogEntry("User id " + result);
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
            log.WriteLogEntry("End GetUserID.");
            return result;
        }
    }
}