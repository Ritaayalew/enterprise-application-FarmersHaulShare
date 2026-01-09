using CatalogAndContracts.Domain.Aggregates;

namespace CatalogAndContracts.Domain.Repositories
{
    public interface IContractRepository
    {
        Task AddAsync(Contract contract);
        Task<Contract?> GetByIdAsync(Guid id);
    }
}