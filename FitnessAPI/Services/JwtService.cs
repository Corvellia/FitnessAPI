using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Permissions;
using System.Text;
using FitnessAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace FitnessAPI.Services
{
    public class JwtService
    {
        private readonly FitnessDevContext _fitnessDevContext;
        private readonly IConfiguration _configuration;

        public JwtService(FitnessDevContext fitnessDevContext, IConfiguration configuration)
        {
            _fitnessDevContext = fitnessDevContext;
            _configuration = configuration;
        }

        public async Task<LoginResponseModel?> Authenticate(LoginModel request)
        {
            if(request.Username is null || request.Password is null)
            {
                return null;
            }

            var userAccount = await _fitnessDevContext.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username && u.Password == request.Password);

            var issuer = _configuration["JwtConfig:Issuer"];
            var audience = _configuration["JwtConfig:Audience"];
            var key = _configuration["JwtConfig:Key"];
            var expirationInMinutes = Convert.ToInt32(_configuration["JwtConfig:TokenValidityInMinutes"]);
            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(expirationInMinutes);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Name, request.Username)
                }),
                Expires = tokenExpiryTimeStamp,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);

            return new LoginResponseModel
            {
                AccessToken = accessToken,
                ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalSeconds,
                UserName = request.Username

            };
        }
    }
}
