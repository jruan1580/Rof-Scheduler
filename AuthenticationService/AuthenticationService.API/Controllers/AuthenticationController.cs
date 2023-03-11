using AuthenticationService.Domain.Services;
using AuthenticationService.DTO.Controllers.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RofShared.Exceptions;
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
                
                SetTokenInResponseCookie(Response, basicUserInfo.Role, token, 30);
                
                return Ok(new { accessToken = token, Id = basicUserInfo.Id, FirstName = basicUserInfo.FirstName, Role = basicUserInfo.Role });
            }
            catch (EntityNotFoundException)
            {
                return NotFound("Username not found");
            }
            catch(ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
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

                await _authService.Logout(id, role.Value);

                SetTokenInResponseCookie(Response, role.Value, string.Empty, -30);               

                return Ok();

            }
            catch (EntityNotFoundException)
            {
                return NotFound("User not found");
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        private void SetTokenInResponseCookie(HttpResponse response, string role, string token, int expirationInMinutes)
        {
            var cookieOption = ConstructCookieOptions(expirationInMinutes);

            switch (role)
            {
                case "Administrator":
                    Response.Cookies.Append("X-Access-Token-Admin", token, cookieOption);
                    break;
                case "Employee":
                    Response.Cookies.Append("X-Access-Token-Employee", token, cookieOption);
                    break;
                case "Client":
                    Response.Cookies.Append("X-Access-Token-Client", token, cookieOption);
                    break;
                default:
                    throw new Exception($"User has unsupported role: {role}");
            }
        }

        private CookieOptions ConstructCookieOptions(int expirationInMinutes)
        {
            var cookieOption = new CookieOptions()
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Path = "/",
                Expires = DateTimeOffset.Now.AddMinutes(expirationInMinutes)
            };

            return cookieOption;
        }

    }
}
