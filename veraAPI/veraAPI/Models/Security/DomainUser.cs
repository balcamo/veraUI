using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VeraAPI.Models.Security
{
    public class DomainUser : LoginUser
    {
        public string DomainName { get; set; }
        public string DomainUpn { get; set; }
        public string DomainSam { get; set; }

    }
}