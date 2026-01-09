using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CatalogAndContracts.Domain.Aggregates;
using CatalogAndContracts.Infrastructure.Persistence;
using CatalogAndContracts.Domain.Repositories;

namespace CatalogAndContracts.Infrastructure.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly CatalogDbContext _db;

        public ContractRepository(CatalogDbContext db)
        {
            _db = db;
        }

        public async Task<Contract?> GetByIdAsync(Guid id)
        {
            // Return the aggregate only (DDD clean)
            return await _db.Contracts
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Contract contract)
        {
            await _db.Contracts.AddAsync(contract);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Contract contract)
        {
            _db.Contracts.Update(contract);
            await _db.SaveChangesAsync();
        }
    }
}
