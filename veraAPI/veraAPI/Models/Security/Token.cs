using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace VeraAPI.Models.Security
{
    public class Token
    {
        public int UserID { get; set; }
        public string SessionKey { get; set; }
        public int[] AccessKey { get; set; } = { 0, 0, 0, 0, 0 };
    }
}