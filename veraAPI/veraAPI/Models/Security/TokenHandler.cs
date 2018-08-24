using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace VeraAPI.Models.Security
{
    public class TokenHandler : JwtSecurityTokenHandler
    {
        public DomainUser CurrentUser { get; set; }

        private Token SessionToken;
        private SecurityTokenDescriptor TokenDescriptor;
        private Scribe Log;

        public TokenHandler()
        {
            this.Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "TokenHandler_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            SessionToken = new Token();
            TokenDescriptor = new SecurityTokenDescriptor();
            CurrentUser = new DomainUser();
        }

        public bool GenerateToken()
        {
            Log.WriteLogEntry("Begin GenerateToken...");
            bool result = false;
            string sharedSecret = "Wendigo";
            InMemorySymmetricSecurityKey signatureKey = new InMemorySymmetricSecurityKey(Encoding.UTF8.GetBytes(sharedSecret));
            SigningCredentials signatureCreds = new SigningCredentials(signatureKey, SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);
            ClaimsIdentity claimsID = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, CurrentUser.DomainUpn),
            });
            Log.WriteLogEntry("End GenerateToken.");
            return result;
        }
    }
}