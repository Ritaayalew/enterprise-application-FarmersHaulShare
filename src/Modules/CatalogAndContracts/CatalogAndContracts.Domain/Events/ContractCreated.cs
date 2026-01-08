namespace CatalogAndContracts.Domain.Events
{
    public class ContractCreated
    {
        public Guid ContractId { get; }
        public string ProductName { get; }
        public string BuyerId { get; }
        public string FarmerId { get; }

        public ContractCreated(Guid contractId, string productName, string buyerId, string farmerId)
        {
            ContractId = contractId;
            ProductName = productName;
            BuyerId = buyerId;
            FarmerId = farmerId;
        }
    }
}