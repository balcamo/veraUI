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

namespace VeraAPI.Models.Security
{
    public class TokenHandler : JwtSecurityTokenHandler
    {
        public User CurrentUser { get; set; }
        public Token SessionToken { get; private set; }

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
                try
                {
                    Log.WriteLogEntry("Success current user is a domain user type.");
                    DomainUser user = (DomainUser)CurrentUser;
                    RandomNumberGenerator rng = RandomNumberGenerator.Create();
                    byte[] rngKey = new byte[32];
                    rng.GetBytes(rngKey);
                    //string sharedSecret = "Wendigo";
                    var signatureKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(rngKey);
                    Log.WriteLogEntry("Success create security key.");
                    var signatureCreds = new Microsoft.IdentityModel.Tokens.SigningCredentials(signatureKey, SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);
                    Log.WriteLogEntry("Success create signing credentials.");
                    var claimsID = new ClaimsIdentity(new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.DomainUpn),
                    new Claim(ClaimTypes.Email, user.UserEmail),
                    new Claim(ClaimTypes.Role, "DomainUser")
                }, "DomainUser");
                    Log.WriteLogEntry("Success create claims identity.");
                    var TokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor()
                    {
                        Issuer = "https://bigfoot.verawp.local",
                        Audience = "https://bermuda.verawp.local",
                        Subject = claimsID,
                        SigningCredentials = signatureCreds,
                    };
                    Log.WriteLogEntry("Success create security token descriptor.");
                    //var jwtHandler = new JwtSecurityTokenHandler();
                    var plainToken = CreateToken(TokenDescriptor);
                    Log.WriteLogEntry("Plain token " + plainToken.ToString());
                    string token = WriteToken(plainToken);
                    Log.WriteLogEntry("Encoded session token " + token);
                    SessionToken = new Token(token, CurrentUser.UserType.ToString());
                    result = true;
                }
                catch (Exception ex)
                {
                    Log.WriteLogEntry("ERROR generating JWT " + ex.Message);
                    result = false;
                }
            }
            else
                Log.WriteLogEntry("Failed current user not a domain user type!");
            Log.WriteLogEntry("End GenerateToken.");
            return result;
        }
    }
}