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
        public User CurrentUser { get; set; }
        public string SessionToken { get; private set; } = string.Empty;

        private Scribe Log;

        public TokenHandler()
        {
            this.Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "TokenHandler_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            CurrentUser = new DomainUser();
        }

        public bool GenerateDomainToken()
        {
            Log.WriteLogEntry("Begin GenerateToken...");
            bool result = false;
            if (CurrentUser.GetType() == typeof(DomainUser))
            {
                DomainUser user = (DomainUser)CurrentUser;
                string sharedSecret = "Wendigo";
                var signatureKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(sharedSecret));
                var signatureCreds = new Microsoft.IdentityModel.Tokens.SigningCredentials(signatureKey, SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);
                var claimsID = new ClaimsIdentity(new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.DomainUpn),
                    new Claim(ClaimTypes.Email, user.UserEmail),
                    new Claim(ClaimTypes.Role, "DomainUser")
                }, "DomainUser");
                var TokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor()
                {
                    Audience = "https://bermuda.verawp.local",
                    Issuer = "https://bigfoot.verawp.local",
                    Subject = claimsID,
                    SigningCredentials = signatureCreds,
                };
                var plainToken = this.CreateToken(TokenDescriptor);
                SessionToken = WriteToken(plainToken);
            }
            Log.WriteLogEntry("End GenerateToken.");
            return result;
        }
    }
}