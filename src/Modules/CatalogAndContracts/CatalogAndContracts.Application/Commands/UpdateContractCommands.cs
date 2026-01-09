using System;
using CatalogAndContracts.Domain.ValueObjects;

namespace CatalogAndContracts.Application.Commands
{
    public class UpdateContractCommand
    {
        public Guid ContractId { get; }
        public decimal? NewPrice { get; }
        public AgreementTerms? NewTerms { get; }

        public UpdateContractCommand(Guid contractId, decimal? newPrice = null, AgreementTerms? newTerms = null)
        {
            ContractId = contractId;
            NewPrice = newPrice;
            NewTerms = newTerms;
        }
    }
}