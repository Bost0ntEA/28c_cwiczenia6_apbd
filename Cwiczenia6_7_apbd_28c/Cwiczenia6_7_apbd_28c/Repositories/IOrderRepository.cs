public interface IOrderRepository
{
    public Task<Int32> DoesOrderWithAmountProductExist(int idProduct, int amount, DateTime createdAt);
    public Task<bool> DoesFulfilledAt(int idOrder);
    public Task UpdateFulfilledAt(int idOrder);
}