using Microsoft.Data.SqlClient;


public class OrderRepository : IOrderRepository
{
    private readonly IConfiguration _configuration;
    public OrderRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<int> DoesOrderWithAmountProductExist(int idProduct, int amount, DateTime createdAt)
    {
        var query = "SELECT TOP 1 IdOrder FROM [Order] WHERE IdProduct = @IdProduct AND Amount = @Amount AND CreatedAt < @createdAt";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdProduct", idProduct);
        command.Parameters.AddWithValue("@Amount", amount);
        command.Parameters.AddWithValue("@createdAt", createdAt);

        await connection.OpenAsync();
        
        var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            int idOrderOrdinal = reader.GetOrdinal("IdOrder");
            if (!reader.IsDBNull(idOrderOrdinal))
            {
                int idOrder = reader.GetInt32(idOrderOrdinal);
                return idOrder;
            }
        }
        await connection.CloseAsync();
        
        
        return -1;
    }

    public async Task<bool> DoesFulfilledAt(int idOrder)
    {
        var query = "SELECT 1 FROM [Order] WHERE IdOrder = @IdOrder AND FulfilledAt IS NULL";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdOrder", idOrder);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        await connection.CloseAsync();
        return res is not null;
    }

    public async Task UpdateFulfilledAt(int idOrder)
    {
        var query = "UPDATE [Order] SET FulfilledAt = GETDATE() WHERE @IdOrder = IdOrder";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdOrder", idOrder);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        await connection.CloseAsync();
    }
}