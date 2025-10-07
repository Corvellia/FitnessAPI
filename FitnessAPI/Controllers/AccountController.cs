using FitnessAPI.Models;
using FitnessAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAPI.Controllers
{
    public class AccountController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly IConfiguration _configuration;

        public AccountController(JwtService jwtService, IConfiguration configuration)
        {
            _jwtService = jwtService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("api/Login")]
        public async Task<ActionResult<LoginModel>> AuthorizedLogin(LoginModel loginRequest)
        {
            var response = await _jwtService.Authenticate(loginRequest);
            if (response == null)
            {
                return Unauthorized("Invalid username or password.");
            }
            return Ok(response);
        }
    }
}
