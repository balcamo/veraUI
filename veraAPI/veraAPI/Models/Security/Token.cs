using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IdentityModel.Tokens.Jwt;

namespace VeraAPI.Models.Security
{
    public class Token : JwtSecurityToken
    {
        public string Signature { get; set; }
        public string Secret { get; set; }
        public string UserEmail { get; private set; }
        public string UserType { get; private set; }
    }
}