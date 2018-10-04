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

        private static Scribe log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "UserDataHandler_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");

        public UserDataHandler(User user, string dbServer, string dbName) : base(dbServer)
        {
            this.dbServer = dbServer;
            this.dbName = dbName;
            this.dataConnectionString = GetDataConnectionString();
            CurrentUser = user;
        }

        public UserDataHandler(string dbServer, string dbName) : base(dbServer)
        {
            this.dbServer = dbServer;
            this.dbName = dbName;
            this.dataConnectionString = GetDataConnectionString();
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
                                    log.WriteLogEntry("Company number " + rdr["company_number"].ToString());
                                    user.CompanyNumber = (int)rdr["company_number"];
                                    log.WriteLogEntry("Department number " + rdr["department_number"].ToString());
                                    user.CompanyNumber = (int)rdr["department_number"];
                                    log.WriteLogEntry("Position number " + rdr["position_number"].ToString());
                                    user.CompanyNumber = (int)rdr["position_number"];
                                    log.WriteLogEntry("Role number " + rdr["role_number"].ToString());
                                    user.CompanyNumber = (int)rdr["role_number"];
                                    log.WriteLogEntry("Domain number " + rdr["domain_number"].ToString());
                                    user.CompanyNumber = (int)rdr["domain_number"];
                                    log.WriteLogEntry("UserName " + rdr["username"].ToString());
                                    user.UserName = rdr["username"].ToString();
                                    log.WriteLogEntry("First Name " + rdr["first_name"].ToString());
                                    user.FirstName = rdr["first_name"].ToString();
                                    log.WriteLogEntry("Last Name " + rdr["last_name"].ToString());
                                    user.LastName = rdr["last_name"].ToString();
                                    log.WriteLogEntry("User Email " + rdr["user_email"].ToString());
                                    user.UserEmail = rdr["user_email"].ToString();
                                    log.WriteLogEntry("Employee ID " + rdr["user_employee_id"].ToString());
                                    user.EmployeeID = rdr["user_employee_id"].ToString();
                                    log.WriteLogEntry("Department " + rdr["dept_name"].ToString());
                                    user.Department.DeptName = rdr["dept_name"].ToString();
                                    log.WriteLogEntry("Department head " + rdr["dept_head_name"].ToString());
                                    user.Department.DeptHeadName = rdr["dept_head_name"].ToString();
                                    log.WriteLogEntry("Department head email" + rdr["dept_head_email"].ToString());
                                    user.Department.DeptHeadEmail = rdr["dept_head_email"].ToString();
                                    log.WriteLogEntry("Supervisor " + rdr["supervisor"].ToString());
                                    user.SupervisorName = rdr["supervisor"].ToString();
                                    log.WriteLogEntry("Supervisor " + rdr["supervisor_email"].ToString());
                                    user.SupervisorEmail = rdr["supervisor_email"].ToString();
                                    log.WriteLogEntry("Domain UPN " + rdr["domain_upn"].ToString());
                                    user.DomainUpn = rdr["domain_upn"].ToString();
                                    log.WriteLogEntry("Domain UserName " + rdr["domain_username"].ToString());
                                    user.DomainUserName = rdr["domain_username"].ToString();
                                    log.WriteLogEntry("Session key " + rdr["session_key"].ToString());
                                    user.Token.SessionKey = rdr["session_key"].ToString();
                                    log.WriteLogEntry("Authenticated " + rdr["authenticated"].ToString());
                                    user.Authenicated = (bool)rdr["authenticated"];
                                    user.Token.UserID = user.UserID;
                                    user.Token.AccessKey[0] = user.CompanyNumber;
                                    user.Token.AccessKey[1] = user.DeptNumber;
                                    user.Token.AccessKey[2] = user.PositionNumber;
                                    user.Token.AccessKey[3] = user.SecurityRoles.FirstOrDefault().RoleNumber;
                                    user.Token.AccessKey[4] = user.SecurityAccess.FirstOrDefault().AccessNumber;
                                    result = true;
                                }
                                else
                                {
                                    log.WriteLogEntry("User type is public user.");
                                    User user = CurrentUser;
                                    log.WriteLogEntry("User ID " + rdr["user_id"].ToString());
                                    user.UserID = (int)rdr["user_id"];
                                    log.WriteLogEntry("Company number " + rdr["company_number"].ToString());
                                    user.CompanyNumber = (int)rdr["company_number"];
                                    log.WriteLogEntry("Department number " + rdr["department_number"].ToString());
                                    user.CompanyNumber = (int)rdr["department_number"];
                                    log.WriteLogEntry("Position number " + rdr["position_number"].ToString());
                                    user.CompanyNumber = (int)rdr["position_number"];
                                    log.WriteLogEntry("Role number " + rdr["role_number"].ToString());
                                    user.CompanyNumber = (int)rdr["role_number"];
                                    log.WriteLogEntry("Domain number " + rdr["domain_number"].ToString());
                                    user.CompanyNumber = (int)rdr["domain_number"];
                                    log.WriteLogEntry("UserName " + rdr["username"].ToString());
                                    user.UserName = rdr["username"].ToString();
                                    log.WriteLogEntry("First Name " + rdr["first_name"].ToString());
                                    user.FirstName = rdr["first_name"].ToString();
                                    log.WriteLogEntry("Last Name " + rdr["last_name"].ToString());
                                    user.LastName = rdr["last_name"].ToString();
                                    log.WriteLogEntry("User Email " + rdr["user_email"].ToString());
                                    user.UserEmail = rdr["user_email"].ToString();
                                    log.WriteLogEntry("Session key " + rdr["session_key"].ToString());
                                    user.Token.SessionKey = rdr["session_key"].ToString();
                                    log.WriteLogEntry("Authenticated " + rdr["authenticated"].ToString());
                                    user.Authenicated = (bool)rdr["authenticated"];
                                    user.Token.UserID = user.UserID;
                                    user.Token.AccessKey[0] = user.CompanyNumber;
                                    user.Token.AccessKey[1] = user.DeptNumber;
                                    user.Token.AccessKey[2] = user.PositionNumber;
                                    user.Token.AccessKey[3] = 0;
                                    user.Token.AccessKey[4] = 0;
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
            log.WriteLogEntry(string.Format("Current User {0} {1} {2}", CurrentUser.UserID, CurrentUser.UserEmail, CurrentUser.Authenicated));
            log.WriteLogEntry("End LoadLoginUser.");
            return result;
        }

        public bool LoadUserData()
        {
            log.WriteLogEntry("Begin LoadUserData...");
            bool result = false;
            User user = CurrentUser;
            string email = user.UserEmail;
            string cmdString = string.Format(@"select * from {0}.dbo.user_header where user_email = @email", dbName);
            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@email", email);
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                        {
                            if (rdr.Read())
                            {
                                log.WriteLogEntry("User ID " + rdr["user_id"].ToString());
                                user.UserID = (int)rdr["user_id"];
                                log.WriteLogEntry("Company number " + rdr["company_number"].ToString());
                                user.CompanyNumber = (int)rdr["company_number"];
                                log.WriteLogEntry("Department number " + rdr["dept_number"].ToString());
                                user.DeptNumber = (int)rdr["dept_number"];
                                log.WriteLogEntry("Position number " + rdr["position_number"].ToString());
                                user.PositionNumber = (int)rdr["position_number"];
                                log.WriteLogEntry("UserName " + rdr["username"].ToString());
                                user.UserName = rdr["username"].ToString();
                                log.WriteLogEntry("User Email " + rdr["user_email"].ToString());
                                user.UserEmail = rdr["user_email"].ToString();
                                log.WriteLogEntry("First Name " + rdr["first_name"].ToString());
                                user.FirstName = rdr["first_name"].ToString();
                                log.WriteLogEntry("Last Name " + rdr["last_name"].ToString());
                                user.LastName = rdr["last_name"].ToString();
                                user.Token.UserID = user.UserID;
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
                int dept = user.DeptNumber;
                log.WriteLogEntry("Loading department number " + dept);
                string cmdString = string.Format(@"select * from {0}.dbo.department where dept_number = @deptNumber", dbName);

                using (SqlConnection conn = new SqlConnection(dataConnectionString))
                {
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                        {
                            cmd.Parameters.AddWithValue("@deptNumber", dept);
                            using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                            {
                                if (rdr.Read())
                                {
                                    user.Department.DeptNumber = (int)rdr["dept_number"];
                                    user.Department.DeptName = rdr["dept_name"].ToString();
                                    user.Department.DeptHeadEmpID = rdr["dept_head_emp_id"].ToString();
                                    user.Department.DeptEmail = rdr["dept_email"].ToString();
                                    log.WriteLogEntry(string.Format("Department loaded {0} {1} {2} {3}", user.Department.DeptNumber, user.Department.DeptName, user.Department.DeptHeadEmpID, user.Department.DeptEmail));
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

        public bool LoadCompany()
        {
            log.WriteLogEntry("Begin LoadCompany...");
            bool result = false;
            User user = CurrentUser;
            int company = user.CompanyNumber;
            log.WriteLogEntry("Loading company " + company);
            string cmdString = string.Format(@"select * from {0}.dbo.company where company_number = @companyNumber", dbName);

            using (SqlConnection conn = new SqlConnection(dataConnectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                    {
                        cmd.Parameters.AddWithValue("@companyNumber", company);
                        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                        {
                            if (rdr.Read())
                            {
                                user.Company.CompanyNumber = (int)rdr["company_number"];
                                user.Company.CompanyName = rdr["company_name"].ToString();
                                user.Company.CompanyEmail = rdr["company_email"].ToString();
                                user.Company.GeneralManagerEmpID = rdr["general_manager_emp_id"].ToString();
                                log.WriteLogEntry(string.Format("Company loaded {0} {1} {2} {3}", user.Company.CompanyNumber, user.Company.CompanyName, user.Company.CompanyEmail,  user.Company.GeneralManagerEmpID));
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

        public bool LoadSecurityRoles()
        {
            log.WriteLogEntry("Begin LoadSecurityRoles...");
            bool result = false;
            if (CurrentUser.GetType() == typeof(DomainUser))
            {
                DomainUser user = (DomainUser)CurrentUser;
                user.SecurityRoles = new List<Role>();
                log.WriteLogEntry("Loading roles for user id " + user.UserID);
                string cmdString = string.Format(@"select rol.* from {0}.dbo.user_role as urol left join Valhalla.dbo.security_role as rol on urol.role_number = rol.role_number where urol.user_id = @userID", dbName);

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
                                        RoleNumber = (int)rdr["rol.role_number"],
                                        RoleTitle = rdr["rol.role_title"].ToString(),
                                        RoleDescription = rdr["rol.role_description"].ToString()
                                    };
                                    log.WriteLogEntry(string.Format("Role loaded {0} {1} {2}", role.RoleNumber, role.RoleTitle, role.RoleDescription));
                                    user.SecurityRoles.Add(role);
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
                int position = user.PositionNumber;
                log.WriteLogEntry("Loading position number " + position);
                string cmdString = string.Format(@"select * from {0}.dbo.position where position_number = @positionNumber", dbName);

                using (SqlConnection conn = new SqlConnection(dataConnectionString))
                {
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                        {
                            cmd.Parameters.AddWithValue("@positionNumber", position);
                            using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                            {
                                if (rdr.Read())
                                {
                                    user.Position.PositionNumber = (int)rdr["position_number"];
                                    user.Position.PostiionTitle = rdr["position_title"].ToString();
                                    user.Position.PositionDescription = rdr["position_description"].ToString();
                                    user.Position.PositionEmail = rdr["position_email"].ToString();
                                    log.WriteLogEntry(string.Format("Position loaded {0} {1} {2} {3}", user.Position.PositionNumber, user.Position.PostiionTitle, user.Position.PositionDescription, user.Position.PositionEmail));
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
                        string cmdString = string.Format(@"insert into {0}.dbo.user_session (user_id, company_number, dept_number, position_number, role_number, domain_number, 
	                                                        username, user_email, first_name, last_name, user_employee_id, dept_name, dept_head_name, dept_head_email, 
	                                                        supervisor, supervisor_email, domain_upn, domain_username, session_key, authenticated)
                                                            values (@userID, @companyNumber, @deptNumber, @positionNumber, @roleNumber, @domainNumber, 
	                                                        @userName, @userEmail, @firstName, @lastName, @userEmpID, @deptName, @deptHead, @deptHeadEmail, 
	                                                        @supervisorName, @supervisorEmail, @domainUpn, @domainUserName, @sessionKey, @authenticated)", dbName);
                        using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                        {
                            log.WriteLogEntry(string.Format("Domain user {0} {1} {2} {3}", user.FirstName, user.LastName, user.DomainUserName, user.DomainUpn));
                            cmd.Parameters.AddWithValue("@userID", user.UserID);
                            cmd.Parameters.AddWithValue("@companyNumber", user.CompanyNumber);
                            cmd.Parameters.AddWithValue("@deptNumber", user.DeptNumber);
                            cmd.Parameters.AddWithValue("@positionNumber", user.PositionNumber);
                            cmd.Parameters.AddWithValue("@roleNumber", user.SecurityRoles.FirstOrDefault().RoleNumber);
                            cmd.Parameters.AddWithValue("@domainNumber", user.DeptNumber);
                            cmd.Parameters.AddWithValue("@userName", user.UserName);
                            cmd.Parameters.AddWithValue("@userEmail", user.UserEmail);
                            cmd.Parameters.AddWithValue("@firstName", user.FirstName);
                            cmd.Parameters.AddWithValue("@lastName", user.LastName);
                            cmd.Parameters.AddWithValue("@userEmpID", user.EmployeeID);
                            cmd.Parameters.AddWithValue("@dept", user.Department.DeptName);
                            cmd.Parameters.AddWithValue("@deptHead", user.Department.DeptHeadName);
                            cmd.Parameters.AddWithValue("@deptHeadEmail", user.Department.DeptHeadEmail);
                            cmd.Parameters.AddWithValue("@supervisorName", user.SupervisorName);
                            cmd.Parameters.AddWithValue("@supervisorEmail", user.SupervisorEmail);
                            cmd.Parameters.AddWithValue("@domainUpn", user.DomainUpn);
                            cmd.Parameters.AddWithValue("@domainUserName", user.DomainUserName);
                            cmd.Parameters.AddWithValue("@sessionKey", user.Token.SessionKey);
                            cmd.Parameters.AddWithValue("@authenticated", user.Authenicated);
                            if (cmd.ExecuteNonQuery() > 0)
                                result = true;
                        }
                    }
                    else
                    {
                        User user = CurrentUser;
                        string cmdString = string.Format(@"insert into {0}.dbo.user_session (user_id, company_number, dept_number, position_number, role_number, domain_number, 
                                                            username, user_email, first_name, last_name, session_key, authenticated)
                                                            values (@userID, @companyNumber, @deptNumber, @positionNumber, @roleNumber, @domainNumber, 
                                                            @username, @userEmail, @firstName, @lastName, @sessionKey, @authenticated)", dbName);
                        using (SqlCommand cmd = new SqlCommand(cmdString, conn))
                        {
                            log.WriteLogEntry(string.Format("User {0} {1} {2} {3}", user.FirstName, user.LastName, user.UserName, user.UserEmail));
                            cmd.Parameters.AddWithValue("@userID", user.UserID);
                            cmd.Parameters.AddWithValue("@companyNumber", user.CompanyNumber);
                            cmd.Parameters.AddWithValue("@deptNumber", user.DeptNumber);
                            cmd.Parameters.AddWithValue("@positionNumber", user.PositionNumber);
                            cmd.Parameters.AddWithValue("@domainNumber", user.DeptNumber);
                            cmd.Parameters.AddWithValue("@userName", user.UserName);
                            cmd.Parameters.AddWithValue("@userEmail", user.UserEmail);
                            cmd.Parameters.AddWithValue("@firstName", user.FirstName);
                            cmd.Parameters.AddWithValue("@lastName", user.LastName);
                            cmd.Parameters.AddWithValue("@sessionKey", user.Token.SessionKey);
                            cmd.Parameters.AddWithValue("@authenticated", user.Authenicated);
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
    }
}