using System;

namespace CatalogAndContracts.Domain.Events
{
    public class ContractCreated
    {
        public Guid ContractId { get; }
        public Guid ProductId { get; }
        public string BuyerId { get; }
        public string FarmerId { get; }

        public ContractCreated(Guid contractId, Guid productId, string buyerId, string farmerId)
        {
            ContractId = contractId;
            ProductId = productId;
            BuyerId = buyerId;
            FarmerId = farmerId;
        }
    }
}
