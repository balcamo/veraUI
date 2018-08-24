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
        public string SessionToken { get; private set; }
        public string UserType { get; private set; }

        public Token(string sessionToken, string userType)
        {
            this.SessionToken = sessionToken;
            this.UserType = userType;
        }
    }
}