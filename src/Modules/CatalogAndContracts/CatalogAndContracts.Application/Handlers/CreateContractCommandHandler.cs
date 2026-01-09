using CatalogAndContracts.Application.Commands;
using CatalogAndContracts.Application.DTOs;
using CatalogAndContracts.Domain.Aggregates;
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
            var contract = new Contract(
                command.ProductId,
                command.Price,
                command.BuyerId,
                command.FarmerId
            );

            await _contractRepository.AddAsync(contract);

            return new ContractDto
            {
                Id = contract.Id,
                ProductId = contract.ProductId,
                Price = contract.Price,
                BuyerId = contract.BuyerId,
                FarmerId = contract.FarmerId
            };
        }
    }
}
