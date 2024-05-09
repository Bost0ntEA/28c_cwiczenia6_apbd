using Microsoft.Data.SqlClient;


public class ProductRepository : IProductRepository
{
    private readonly IConfiguration _configuration;
    public ProductRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<bool> DoesProductExist(int id)
    {
        var query = "SELECT 1 FROM Product WHERE IdProduct = @IdProduct";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdProduct", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        await connection.CloseAsync();
        return res is not null;
    }
}