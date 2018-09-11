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
        public User CurrentUser { get; set; }

        private Scribe log;

        public TokenHandler(User user)
        {
            this.log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "TokenHandler_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            CurrentUser = user;
        }

        public bool GenerateDomainToken()
        {
            log.WriteLogEntry("Starting GenerateDomainToken...");
            bool result = false;

            if (CurrentUser.GetType() == typeof(DomainUser))
            {
                DomainUser user = (DomainUser)CurrentUser;
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
                        Issuer = "https://bigfoot.verawp.local",
                        Audience = "https://bermuda.verawp.local",
                        Subject = claimsID,
                        SigningCredentials = signatureCreds,
                    };
                    log.WriteLogEntry("Success create security token descriptor.");
                    var plainToken = CreateToken(TokenDescriptor);
                    log.WriteLogEntry("Plain token " + plainToken.ToString());
                    string token = WriteToken(plainToken);
                    log.WriteLogEntry("Encoded session token " + token);

                    Token SessionToken = new Token(token, user.UserType);
                    string JsonToken = JsonConvert.SerializeObject(SessionToken);
                    if (JsonToken != null)
                    {
                        log.WriteLogEntry("Success converting token string to json string");
                        user.SessionToken = JsonToken;
                        result = true;
                    }
                    else
                        log.WriteLogEntry("Failed to convert token string to json string!");
                }
                catch (Exception ex)
                {
                    log.WriteLogEntry("ERROR generating JWT " + ex.Message);
                    result = false;
                }
            }
            else
                log.WriteLogEntry("Failed current user is not a domain user!");
            log.WriteLogEntry("End GenerateDomainToken.");
            return result;
        }

        public bool TokenToString()
        {
            log.WriteLogEntry("Starting TokenToString.");
            bool result = false;
            try
            {
                Token token = JsonConvert.DeserializeObject<Token>(CurrentUser.SessionToken);
                CurrentUser.SessionKey = token.SessionKey;
                CurrentUser.UserType = token.UserType;
                if (token != null)
                    result = true;
                else
                    log.WriteLogEntry("");
            }
            catch (Exception ex)
            {
                log.WriteLogEntry(ex.Message);
            }
            log.WriteLogEntry("End TokenToString.");
            return result;
        }
    }
}