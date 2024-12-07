using JWTWebApp.Models;
using JWTWebApp.Entities;

namespace JWTWebApp.Service
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest Model);
        List<User> GetAll();
    }
}
