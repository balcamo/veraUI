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
        public User CurrentUser { get; private set; }

        private Scribe Log;

        public TokenHandler(User user)
        {
            this.Log = new Scribe(System.Web.HttpContext.Current.Server.MapPath("~/logs"), "TokenHandler_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".log");
            CurrentUser = user;
        }

        public bool GenerateDomainToken()
        {
            Log.WriteLogEntry("Begin GenerateToken...");
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

                    Token SessionToken = new Token(CurrentUser.UserID, token, CurrentUser.UserType);
                    string JsonToken = JsonConvert.SerializeObject(SessionToken);
                    if (JsonToken != null)
                    {
                        Log.WriteLogEntry("Success converting token string to json string");
                        CurrentUser.SessionToken = JsonToken;
                        result = true;
                    }
                    else
                        Log.WriteLogEntry("Failed to convert token string to json string!");
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

        public bool TokenToString()
        {
            Log.WriteLogEntry("Starting TokenToString.");
            bool result = false;
            try
            {
                Token token = JsonConvert.DeserializeObject<Token>(CurrentUser.SessionToken);
                CurrentUser.UserID = token.UserID;
                CurrentUser.SessionKey = token.SessionKey;
                CurrentUser.UserType = token.UserType;
                if (token != null)
                    result = true;
                else
                    Log.WriteLogEntry("");
            }
            catch (Exception ex)
            {
                Log.WriteLogEntry(ex.Message);
            }
            Log.WriteLogEntry("End TokenToString.");
            return result;
        }
    }
}