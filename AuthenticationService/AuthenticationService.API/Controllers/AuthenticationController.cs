using AuthenticationService.API.Models;
using AuthenticationService.Domain.Exceptions;
using AuthenticationService.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthenticationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ITokenHandler _tokenHandler;
        private readonly IAuthService _authService;

        public AuthenticationController(ITokenHandler tokenHandler, IAuthService authService)
        {
            _tokenHandler = tokenHandler;
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPatch("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO employee)
        {
            try
            {
                var basicUserInfo = await _authService.Login(employee.Username, employee.Password);

                var token = _tokenHandler.GenerateTokenForRole(basicUserInfo.Role);
                switch (basicUserInfo.Role)
                {
                    case "Administrator":
                        Response.Cookies.Append("X-Access-Token-Admin", token, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.None, Secure = true, Path = "/", Expires = DateTimeOffset.Now.AddMinutes(30) });
                        break;
                    case "Employee":
                        Response.Cookies.Append("X-Access-Token-Employee", token, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.None, Secure = true, Path = "/", Expires = DateTimeOffset.Now.AddMinutes(30) });
                        break;
                    case "Client":
                        Response.Cookies.Append("X-Access-Token-Client", token, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.None, Secure = true, Path = "/", Expires = DateTimeOffset.Now.AddMinutes(30) });
                        break;
                    default:
                        throw new Exception($"User has unsupported role: {basicUserInfo.Role}");
                }
         
                return Ok(new { accessToken = token, Id = basicUserInfo.Id, FirstName = basicUserInfo.FirstName, Role = basicUserInfo.Role });
            }
            catch (NotFoundException)
            {
                return NotFound("Username not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }   

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpPatch("{id}/logout")]
        public async Task<IActionResult> Logout(long id)
        {
            try
            {
                var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

                var token = _tokenHandler.ExtractTokenFromRequest(Request, role.Value);

                await _authService.Logout(id, role.Value, token);

                switch (role.Value)
                {
                    case "Administrator":
                        Response.Cookies.Append("X-Access-Token-Admin", "", new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.None, Secure = true, Path = "/", Expires = DateTimeOffset.Now.AddMinutes(-30) });
                        break;
                    case "Employee":
                        Response.Cookies.Append("X-Access-Token-Employee", "", new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.None, Secure = true, Path = "/", Expires = DateTimeOffset.Now.AddMinutes(-30) });
                        break;
                    case "Client":
                        Response.Cookies.Append("X-Access-Token-Client", "", new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.None, Secure = true, Path = "/", Expires = DateTimeOffset.Now.AddMinutes(-30) });
                        break;
                    default:
                        throw new Exception($"User has unsupported role: {role.Value}");
                }

                return Ok();

            }
            catch (NotFoundException)
            {
                return NotFound("User not found");
            }           
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
