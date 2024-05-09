

public interface IProductWarehouse
{
    public Task<bool> DoesOrderExistInProductWarehouse(int idOrder);
    public Task AddRecordToProduct_Warehouse(int idProduct, int idWarehouse, int amount, DateTime createdAt, int idOrder);
}