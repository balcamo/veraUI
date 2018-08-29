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
        public string UserEmail { get; private set; }
        public string SessionKey { get; private set; }
        public int UserType { get; private set; }

        public Token(string email, string sessionKey, int userType)
        {
            this.UserEmail = email;
            this.SessionKey = sessionKey;
            this.UserType = userType;
        }
    }
}