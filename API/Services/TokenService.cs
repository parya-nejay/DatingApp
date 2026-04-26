using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using API.Enttities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService(IConfiguration config) : ITokenService
    {
        public string CreateToken(AppUser user) //Create a token for a user and return it as string
        {
            var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot get token key"); // "TokenKey" is already stored in appsettings.json  //it is our secret key//go to appsettings and GET the secret key (not create it)
             // if the key does not exist → stop the program with error
       if (tokenKey.Length < 64)
       throw new Exception("Token key needs to be >= 64 characters");
       var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)); //create a symmetric security key with the token key
       var claims = new List<Claim>
       {
           new(ClaimTypes.Email, user.Email), //create a claim with the user's email
           new(ClaimTypes.NameIdentifier, user.Id) //create a claim with the user's id
       };
         var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); //create signing credentials with the key and the HMAC SHA512 algorithm
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), //set the subject of the token to the claims we created
                Expires = DateTime.UtcNow.AddDays(7), //set the expiration of the token to 7 days from now
                SigningCredentials = creds //set the signing credentials to the credentials we created
            };
            var tokenHandler = new JwtSecurityTokenHandler(); //create a token handler
            var token = tokenHandler.CreateToken(tokenDescriptor); //create the token
            return tokenHandler.WriteToken(token); //return the token as a string
        }
    }
}