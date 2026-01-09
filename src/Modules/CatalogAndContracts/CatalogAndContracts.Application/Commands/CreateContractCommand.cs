public class CreateContractCommand
{
    public Guid ProductId { get; }
    public decimal Price { get; }
    public string BuyerId { get; }
    public string FarmerId { get; }

    public CreateContractCommand(Guid productId, decimal price, string buyerId, string farmerId)
    {
        ProductId = productId;
        Price = price;
        BuyerId = buyerId;
        FarmerId = farmerId;
    }
}
