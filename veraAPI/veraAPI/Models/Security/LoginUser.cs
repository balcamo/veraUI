using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VeraAPI.Models.Security
{
    public class LoginUser
    {
        public string LoginToken { get; set; }
        public string UserName { get; set; }
        public string UserPwd { get; set; }
        public string UserEmail { get; set; }
        public int UserType { get; set; }
        public bool Authenicated { get; set; }

        public LoginUser()
        {
            this.Authenicated = false;
        }
    }
}