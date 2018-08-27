using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VeraAPI.Models.Security
{
    public class User
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string UserPwd { get; set; }
        public string UserEmail { get; set; }
        public int UserType { get; set; }
        public bool Authenicated { get; set; }
        public string LoginToken { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public static readonly int DomainUser = 1;
        public static readonly int FieldUser = 2;
        public static readonly int PublicUser = 3;
    }
}