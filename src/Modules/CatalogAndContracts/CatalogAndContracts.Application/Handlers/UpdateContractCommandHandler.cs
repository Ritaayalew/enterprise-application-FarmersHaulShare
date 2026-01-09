using System.Threading.Tasks;
using CatalogAndContracts.Application.Commands;
using CatalogAndContracts.Domain.Repositories;
using CatalogAndContracts.Domain.Aggregates;

namespace CatalogAndContracts.Application.Handlers
{
    public class UpdateContractCommandHandler
    {
        private readonly IContractRepository _contractRepository;

        public UpdateContractCommandHandler(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        public async Task Handle(UpdateContractCommand command)
        {
            var contract = await _contractRepository.GetByIdAsync(command.ContractId);
            if (contract == null)
                throw new KeyNotFoundException($"Contract with ID {command.ContractId} not found.");

            if (command.NewPrice.HasValue)
            {
                contract.UpdatePrice(command.NewPrice.Value);
            }

            if (command.NewTerms != null)
            {
                contract.SetAgreementTerms(command.NewTerms);
            }

            await _contractRepository.UpdateAsync(contract);
        }
    }
}