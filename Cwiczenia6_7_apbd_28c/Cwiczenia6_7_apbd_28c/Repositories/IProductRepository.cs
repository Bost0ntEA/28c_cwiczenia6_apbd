

public interface IProductRepository
{
    Task<bool> DoesProductExist(int id);
}