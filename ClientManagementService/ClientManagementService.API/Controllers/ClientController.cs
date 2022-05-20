using System;
using System.Threading.Tasks;
using ClientManagementService.API.DTO;
using ClientManagementService.API.DTOMapper;
using ClientManagementService.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientManagementService.API.Controllers
{
    [Route("api/client")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateClient([FromBody] ClientDTO client)
        {
            try
            {
                await _clientService.CreateClient(ClientDTOMapper.FromDTOClient(client), client.Password);

                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClientById(long id)
        {
            try
            {
                var client = await _clientService.GetClientById(id);

                return Ok(ClientDTOMapper.ToDTOClient(client));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetClientByEmail(string email)
        {
            try
            {
                var client = await _clientService.GetClientByEmail(email);

                return Ok(ClientDTOMapper.ToDTOClient(client));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch("login")]
        public async Task<IActionResult> ClientLogin([FromBody] ClientDTO client)
        {
            try
            {
                await _clientService.ClientLogin(client.EmailAddress, client.Password);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch("logout/{id}")]
        public async Task<IActionResult> ClientLogout(long id)
        {
            try
            {
                await _clientService.ClientLogout(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch("reset/locked/{id}")]
        public async Task<IActionResult> ResetClientLockedStatus(long id)
        {
            try
            {
                await _clientService.ResetClientFailedLoginAttempts(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch("update/password")]
        public async Task<IActionResult> UpdatePassword([FromBody] PasswordDTO newPassword)
        {
            try
            {
                await _clientService.UpdatePassword(newPassword.Id, newPassword.NewPassword);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("update/info")]
        public async Task<IActionResult> UpdateClientInfo([FromBody] ClientDTO client)
        {
            try
            {
                await _clientService.UpdateClientInfo(ClientDTOMapper.FromDTOClient(client));

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteClientById(long id)
        {
            try
            {
                await _clientService.DeleteClientById(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}