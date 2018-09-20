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
        public string SessionKey { get; private set; }
        public int UserType { get; private set; }

        public Token(int userID, string sessionKey, int userType)
        {
            this.UserID = userID;
            this.SessionKey = sessionKey;
            this.UserType = userType;
        }
    }
}