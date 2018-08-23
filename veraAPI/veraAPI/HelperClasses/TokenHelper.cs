using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IdentityModel.Tokens.Jwt;
using VeraAPI.Models.Security;

namespace VeraAPI.HelperClasses
{
    public class TokenHelper
    {
        public string TokenBody { get; set; }

        private Token LoginToken;


    }
}