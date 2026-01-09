using CatalogAndContracts.Application.Commands;
using CatalogAndContracts.Application.DTOs;
using CatalogAndContracts.Domain.Aggregates;
using CatalogAndContracts.Domain.Entities;
using CatalogAndContracts.Domain.Repositories;

namespace CatalogAndContracts.Application.Handlers
{
    public class CreateContractCommandHandler
    {
        private readonly IContractRepository _contractRepository;

        public CreateContractCommandHandler(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        public async Task<ContractDto> Handle(CreateContractCommand command)
        {
            var product = new Product(command.ProductName, command.BasePrice);

            var contract = new Contract(
                product,
                command.Price,
                command.BuyerId,
                command.FarmerId
            );

            await _contractRepository.AddAsync(contract);

            return new ContractDto
            {
                Id = contract.Id,
                ProductName = contract.Product?.Name ?? string.Empty,
                Price = contract.Price,
                BuyerId = contract.BuyerId,
                FarmerId = contract.FarmerId
            };
        }
    }
}