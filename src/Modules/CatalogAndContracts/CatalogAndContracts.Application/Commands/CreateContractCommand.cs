namespace CatalogAndContracts.Application.Commands
{
    public class CreateContractCommand
    {
        public string ProductName { get; }
        public decimal BasePrice { get; } 
         public decimal Price { get; }
        public string BuyerId { get; }
        public string FarmerId { get; }

        public CreateContractCommand(string productName, decimal basePrice,decimal price, string buyerId, string farmerId)
        {
            ProductName = productName;
            BasePrice = basePrice;  
            Price = price;
            BuyerId = buyerId;
            FarmerId = farmerId;
        }
    }
}