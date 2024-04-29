using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLib
{
    public class ServiceJWT
    {
        static string key = "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201"+
                            "d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";

        public static string GenerateToken(Guid AuthId)
        {
            string Res = "";
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                string rdStr = Guid.NewGuid().ToString();
                var secToken = new JwtSecurityToken(
                    signingCredentials: credentials,
                    issuer: "Sample",
                    audience: "Sample",//DateTime.Now.ToString("yyMMddHHmmss") + rdStr.ToUpper().Substring(rdStr.Length - 6),
                    claims: new[]
                    {
                    new Claim(JwtRegisteredClaimNames.Sub, "meziantou")
                    },
                    expires: DateTime.UtcNow.AddHours(24));

                var handler = new JwtSecurityTokenHandler();
                Res = handler.WriteToken(secToken); 
            }
            catch (Exception ex)
            { }
            return Res;
        }

        public static bool ValidateToken(string authToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();

            SecurityToken validatedToken;
            IPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
            return true;
        }

        public static TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateLifetime = false, // Because there is no expiration in the generated token
                ValidateAudience = false, // Because there is no audiance in the generated token
                ValidateIssuer = false,   // Because there is no issuer in the generated token
                ValidIssuer = "Sample",
                ValidAudience = "Sample",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)) // The same key as the one that generate the token
            };
        }

        
    }
}
