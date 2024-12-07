using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using JWTWebApp.Service;
using JWTWebApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace JWTWebApp.Controllers
{
  
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _usersvc;
        public UserController(IUserService usersvc)
        {
            _usersvc = usersvc;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticateRequest creds)
        {
          
            var res = _usersvc.Authenticate(creds);
            if(res == null)
            {
                return BadRequest(new { msg = "Unauthorized" });
            }
            return Ok(res);

        }

      
        [HttpGet]
        [Authorize]

        public IActionResult GetAll()
        {
            return  Ok(_usersvc.GetAll());
        }
    }
}
