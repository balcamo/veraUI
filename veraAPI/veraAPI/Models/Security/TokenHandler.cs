using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IdentityModel.Tokens;
using System.IdentityModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel;
using Newtonsoft.Json;
using VeraAPI.Models.Tools;

namespace VeraAPI.Models.Security
{
    public class TokenHandler : JwtSecurityTokenHandler
    {
        public Token SessionToken { get; private set; }

        private static Scribe log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "TokenHandler_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");

        public TokenHandler() { }

        public bool GenerateDomainToken(DomainUser user)
        {
            log.WriteLogEntry("Starting GenerateDomainToken...");
            bool result = false;

            log.WriteLogEntry(string.Format("Current user {0} {1}", user.DomainUpn, user.UserEmail));
            try
            {
                RandomNumberGenerator rng = RandomNumberGenerator.Create();
                byte[] rngKey = new byte[32];
                rng.GetBytes(rngKey);
                var signatureKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(rngKey);
                log.WriteLogEntry("Success create security key.");
                var signatureCreds = new Microsoft.IdentityModel.Tokens.SigningCredentials(signatureKey, SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);
                log.WriteLogEntry("Success create signing credentials.");
                var claimsID = new ClaimsIdentity(new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.DomainUpn),
                    new Claim(ClaimTypes.Email, user.UserEmail),
                    new Claim(ClaimTypes.Role, "DomainUser")
                }, "DomainUser");
                log.WriteLogEntry("Success create claims identity.");
                var TokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor()
                {
                    Issuer = Constants.Issuer,
                    Audience = Constants.Audience,
                    Subject = claimsID,
                    SigningCredentials = signatureCreds,
                };
                log.WriteLogEntry("Success create security token descriptor.");
                var plainToken = CreateToken(TokenDescriptor);
                log.WriteLogEntry("Plain token " + plainToken.ToString());
                string token = WriteToken(plainToken);
                log.WriteLogEntry("Encoded session token " + token);
                user.Token.SessionKey = token;
                result = true;
            }
            catch (Exception ex)
            {
                log.WriteLogEntry("ERROR generating JWT " + ex.Message);
                result = false;
            }
            log.WriteLogEntry("End GenerateDomainToken.");
            return result;
        }

        public bool VerifyToken()
        {
            log.WriteLogEntry("Starting VerifyToken...");
            bool result = false;
            
            // Code goes here

            log.WriteLogEntry("End VerifyToken.");
            return result;
        }
    }
}