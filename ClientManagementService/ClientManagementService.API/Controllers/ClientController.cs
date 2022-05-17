using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientManagementService.API.DTO;
using ClientManagementService.API.DTOMapper;
using ClientManagementService.Domain.Services;
using Microsoft.AspNetCore.Http;
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

        //Task DeleteClientById(long id);
        //Task<Client> GetClientByEmail(string email);
        //Task IncrementClientFailedLoginAttempts(long id);
        //Task ResetClientFailedLoginAttempts(long id);
        //Task UpdateClientInfo(Client client);
        //Task UpdatePassword(long id, string newPassword);
    }
}