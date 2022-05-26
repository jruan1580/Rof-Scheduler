using System;
using System.Threading.Tasks;
using ClientManagementService.API.DTO;
using ClientManagementService.API.DTOMapper;
using ClientManagementService.Domain.Exceptions;
using ClientManagementService.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientManagementService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] ClientDTO client)
        {
            try
            {
                await _clientService.CreateClient(ClientDTOMapper.FromDTOClient(client), client.Password);

                return StatusCode(201);
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClientById(long id)
        {
            try
            {
                var client = await _clientService.GetClientById(id);

                return Ok(ClientDTOMapper.ToDTOClient(client));
            }
            catch (ClientNotFoundException)
            {
                return NotFound($"Client with id {id} not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{email}/email")]
        public async Task<IActionResult> GetClientByEmail(string email)
        {
            try
            {
                var client = await _clientService.GetClientByEmail(email);

                if (client == null)
                {
                    return NotFound($"Client with email, {email}, was not found");
                }

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
                var clientLogIn = await _clientService.ClientLogin(client.Username, client.Password);

                return Ok(new { Id = clientLogIn.Id, FirstName = clientLogIn.FirstName, LastName = clientLogIn.LastName});
            }
            catch (ClientNotFoundException)
            {
                return NotFound($"Client with username: {client} was not found");
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

        [HttpPatch("{id}/logout")]
        public async Task<IActionResult> ClientLogout(long id)
        {
            try
            {
                await _clientService.ClientLogout(id);

                return Ok();
            }
            catch (ClientNotFoundException)
            {
                return NotFound($"Client with id {id} not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch("{id}/locked")]
        public async Task<IActionResult> ResetClientLockedStatus(long id)
        {
            try
            {
                await _clientService.ResetClientFailedLoginAttempts(id);

                return Ok();
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

        [HttpPatch("password")]
        public async Task<IActionResult> UpdatePassword([FromBody] PasswordDTO newPassword)
        {
            try
            {
                await _clientService.UpdatePassword(newPassword.Id, newPassword.NewPassword);

                return Ok();
            }
            catch (ClientNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("info")]
        public async Task<IActionResult> UpdateClientInfo([FromBody] ClientDTO client)
        {
            try
            {
                await _clientService.UpdateClientInfo(ClientDTOMapper.FromDTOClient(client));

                return Ok();
            }
            catch (ClientNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClientById(long id)
        {
            try
            {
                await _clientService.DeleteClientById(id);

                return Ok();
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}