

public interface IWarehouseRepository
{
    public Task<bool> DoesWarehouseExist(int id);
}