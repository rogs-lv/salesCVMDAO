using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using salesCVM.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Configuration;

namespace salesCVM.Token
{
    public static class TokenGenerator
    {
        public static string GenerateTokenJwt(User user) {
            //appsetting for token JWT
            string secretKey = ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
            string audienceToken = ConfigurationManager.AppSettings["JWT_AUDIENCE_TOKEN"];
            string issuerToken = ConfigurationManager.AppSettings["JWT_ISSUER_TOKEN"];
            string expireTime = ConfigurationManager.AppSettings["JWT_EXPIRE_MINUTES"];

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(secretKey));
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            
            //create a claimsIdentity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[] { new Claim("Code", user.Code), new Claim("Name", user.Name), new Claim("CardCode", user.U_CardCode), new Claim("CardName", user.U_CardName), new Claim("SlpCode", user.U_SlpCode), new Claim("SlpName", user.U_SlpName), new Claim("CambioPrecio", user.U_CambioPrecio.ToString()), new Claim("CambioSN", user.U_CambioSN.ToString()), new Claim("PDescuento", user.U_PrcntjDescMax.ToString()), new Claim("TaxCode", user.TaxCode), new Claim("Rate", user.Rate.ToString()), new Claim("ListNum", user.ListNum.ToString()), new Claim("WhsCode", user.WhsCode), new Claim("Sucursal", user.U_Sucursal)});

            //create token to the user
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(
                audience: audienceToken,
                issuer: issuerToken,
                subject: claimsIdentity,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(expireTime)),
                signingCredentials: signingCredentials
            );

            return tokenHandler.WriteToken(jwtSecurityToken); ;
        }
    }
}
