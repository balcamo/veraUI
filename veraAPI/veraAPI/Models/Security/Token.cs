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
        public int[] UserType { get; set; } = new int[4];

        public Token()
        {

        }
    }
}