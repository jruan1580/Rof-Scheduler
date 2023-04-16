using System.Collections.Generic;
using System.Threading.Tasks;
using ClientManagementService.Domain.Mappers.DTO;
using ClientManagementService.Domain.Services;
using ClientManagementService.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RofShared.FilterAttributes;

namespace ClientManagementService.API.Controllers
{    
    [CookieActionFilter]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientAuthService _clientAuthService;
        private readonly IClientRetrievalService _clientRetrievalService;
        private readonly IClientUpsertService _clientUpsertService;

        public ClientController(IClientService clientService, IClientRetrievalService clientRetrievalService, IClientUpsertService clientUpsertService)
        {
            _clientAuthService = clientAuthService;
            _clientRetrievalService = clientRetrievalService;
            _clientUpsertService = clientUpsertService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] ClientDTO client)
        {
            await _clientUpsertService.CreateClient(ClientDTOMapper.FromDTOClient(client), client.Password);

            return StatusCode(201);
        }

        [Authorize(Roles = "Administrator,Employee")]
        [HttpGet]
        public async Task<IActionResult> GetAllClients([FromQuery] int page, [FromQuery] int offset, [FromQuery] string keyword)
        {
            var clientList = new List<ClientDTO>();

            var result = await _clientRetrievalService.GetAllClientsByKeyword(page, offset, keyword);

            foreach (var client in result.Clients)
            {
                clientList.Add(ClientDTOMapper.ToDTOClient(client));
            }

            return Ok(new { clients = clientList, totalPages = result.TotalPages });
        }

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClientById(long id)
        {
            var client = await _clientRetrievalService.GetClientById(id);

            return Ok(ClientDTOMapper.ToDTOClient(client));
        }

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpGet("{email}/email")]
        public async Task<IActionResult> GetClientByEmail(string email)
        {
            var client = await _clientRetrievalService.GetClientByEmail(email);

            if (client == null)
            {
                return NotFound($"Client with email, {email}, was not found");
            }

            return Ok(ClientDTOMapper.ToDTOClient(client));
        }

        [Authorize(Roles = "Client,Internal")]
        [HttpPatch("login")]
        public async Task<IActionResult> ClientLogin([FromBody] ClientDTO client)
        {
            var clientLogIn = await _clientAuthService.ClientLogin(client.Username, client.Password);

            return Ok(new { Id = clientLogIn.Id, FirstName = clientLogIn.FirstName, LastName = clientLogIn.LastName });
        }

        [Authorize(Roles = "Client,Internal")]
        [HttpPatch("{id}/logout")]
        public async Task<IActionResult> ClientLogout(long id)
        {
            await _clientAuthService.ClientLogout(id);

            return Ok();
        }

        [Authorize(Roles = "Administrator,Employee")]
        [HttpPatch("{id}/locked")]
        public async Task<IActionResult> ResetClientLockedStatus(long id)
        {
            await _clientUpsertService.ResetClientFailedLoginAttempts(id);

            return Ok();
        }

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpPatch("password")]
        public async Task<IActionResult> UpdatePassword([FromBody] PasswordDTO newPassword)
        {
            await _clientUpsertService.UpdatePassword(newPassword.Id, newPassword.NewPassword);

            return Ok();
        }

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpPut("info")]
        public async Task<IActionResult> UpdateClientInfo([FromBody] ClientDTO client)
        {
            await _clientUpsertService.UpdateClientInformation(ClientDTOMapper.FromDTOClient(client));

            return Ok();
        }

        [Authorize(Roles = "Administrator,Employee")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClientById(long id)
        {
            await _clientUpsertService.DeleteClientById(id);

            return Ok();
        }
    }
}