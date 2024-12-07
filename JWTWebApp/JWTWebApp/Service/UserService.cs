using JWTWebApp.Entities;
using JWTWebApp.Models;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using JWTWebApp.Helper;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace JWTWebApp.Service
{
    public class UserService : IUserService
    {

        private readonly AppSettings _appSettings;
       

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        private List<User> users = new List<User>
        {
            new User{Id = 1, Name="Chinmay", Email = "c@g.com", Password = "1234"},
            new User{Id = 2, Name="Swapnil", Email = "s@g.com", Password = "1234"},
        };
       
        public AuthenticateResponse Authenticate(AuthenticateRequest request)
        {
            var user = users.SingleOrDefault(x => x.Email == request.Email && x.Password == request.Password);
            if (user == null)
            {
                return null;
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenStr = tokenHandler.WriteToken(token);
            return new AuthenticateResponse(user, tokenStr);

        }

        
        public List<User> GetAll()
        {
            return users;
        }

        
    }
}
