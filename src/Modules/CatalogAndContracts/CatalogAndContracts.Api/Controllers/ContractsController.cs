using Microsoft.AspNetCore.Mvc;
using CatalogAndContracts.Application.Commands;
using CatalogAndContracts.Application.DTOs;
using CatalogAndContracts.Application.Handlers;

namespace CatalogAndContracts.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractsController : ControllerBase
    {
        private readonly CreateContractCommandHandler _handler;

        public ContractsController(CreateContractCommandHandler handler)
        {
            _handler = handler;
        }

        [HttpPost]
        public async Task<ActionResult<ContractDto>> Create([FromBody] CreateContractCommand command)
        {
            var result = await _handler.Handle(command);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetAll()
        {
            // Placeholder until repository query is implemented
            return Ok(new List<ContractDto>());
        }
    }
}