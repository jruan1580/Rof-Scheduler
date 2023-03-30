using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientManagementService.API.DTO;
using ClientManagementService.API.DTOMapper;
using ClientManagementService.API.Filters;
using ClientManagementService.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RofShared.Exceptions;

namespace ClientManagementService.API.Controllers
{    
    [CookieActionFilter]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [AllowAnonymous]
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

        [Authorize(Roles = "Administrator,Employee")]
        [HttpGet]
        public async Task<IActionResult> GetAllClients([FromQuery] int page, [FromQuery] int offset, [FromQuery] string keyword)
        {
            try
            {
                var clientList = new List<ClientDTO>();

                var result = await _clientService.GetAllClientsByKeyword(page, offset, keyword);

                foreach (var client in result.Clients)
                {
                    clientList.Add(ClientDTOMapper.ToDTOClient(client));
                }

                return Ok(new { clients = clientList, totalPages = result.TotalPages });
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClientById(long id)
        {
            try
            {
                var client = await _clientService.GetClientById(id);

                return Ok(ClientDTOMapper.ToDTOClient(client));
            }
            catch (EntityNotFoundException notFound)
            {
                return NotFound(notFound.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Administrator,Employee,Client")]
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

        [Authorize(Roles = "Internal")]
        [HttpPatch("login")]
        public async Task<IActionResult> ClientLogin([FromBody] ClientDTO client)
        {
            try
            {
                var clientLogIn = await _clientService.ClientLogin(client.Username, client.Password);

                return Ok(new { Id = clientLogIn.Id, FirstName = clientLogIn.FirstName, LastName = clientLogIn.LastName});
            }
            catch (EntityNotFoundException notFound)
            {
                return NotFound(notFound.Message);
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

        [Authorize(Roles = "Client,Internal")]
        [HttpPatch("{id}/logout")]
        public async Task<IActionResult> ClientLogout(long id)
        {
            try
            {
                await _clientService.ClientLogout(id);

                return Ok();
            }
            catch (EntityNotFoundException notFound)
            {
                return NotFound(notFound.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Administrator,Employee")]
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

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpPatch("password")]
        public async Task<IActionResult> UpdatePassword([FromBody] PasswordDTO newPassword)
        {
            try
            {
                await _clientService.UpdatePassword(newPassword.Id, newPassword.NewPassword);

                return Ok();
            }
            catch (EntityNotFoundException notFound)
            {
                return NotFound(notFound.Message);
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

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpPut("info")]
        public async Task<IActionResult> UpdateClientInfo([FromBody] ClientDTO client)
        {
            try
            {
                await _clientService.UpdateClientInfo(ClientDTOMapper.FromDTOClient(client));

                return Ok();
            }
            catch (EntityNotFoundException notFound)
            {
                return NotFound(notFound.Message);
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

        [Authorize(Roles = "Administrator,Employee")]
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