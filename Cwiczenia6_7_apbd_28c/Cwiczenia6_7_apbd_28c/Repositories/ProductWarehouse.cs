using Microsoft.Data.SqlClient;


public class ProductWarehouse : IProductWarehouse
{
    private readonly IConfiguration _configuration;
    public ProductWarehouse(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<bool> DoesOrderExistInProductWarehouse(int idOrder)
    {
        var query = "SELECT 1 FROM Product_Warehouse WHERE IdOrder = @IdOrder";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdOrder", idOrder);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return res is not null;
    }

    public async Task AddRecordToProduct_Warehouse(int idProduct, int idWarehouse, int amount, DateTime createdAt, int idOrder)
    {
        var query = "SELECT Price FROM Product WHERE IdProduct = @IdProduct";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdProduct", idProduct);

        await connection.OpenAsync();
        
        var reader = await command.ExecuteReaderAsync();
        decimal price = 0;
        if (await reader.ReadAsync())
        {
            var priceOrdinal = reader.GetOrdinal("Price");
            price = reader.GetDecimal(priceOrdinal);
        }
        
        await connection.CloseAsync();
        price *= amount;
        
        var query2 = "INSERT INTO Product_Warehouse VALUES(@idWarehouse, @idProduct, @idOrder, @amount, @price, @createdAt)";

        await using SqlConnection connectionInsert = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand commandInsert = new SqlCommand(); 
        
        commandInsert.Connection = connectionInsert;
        commandInsert.CommandText = query2;
        commandInsert.Parameters.AddWithValue("@price", price);
        commandInsert.Parameters.AddWithValue("@idProduct", idProduct);
        commandInsert.Parameters.AddWithValue("@idWarehouse", idWarehouse);
        commandInsert.Parameters.AddWithValue("@amount", amount);
        commandInsert.Parameters.AddWithValue("@createdAt", createdAt);
        commandInsert.Parameters.AddWithValue("@idOrder", idOrder);

        await connectionInsert.OpenAsync();
        await commandInsert.ExecuteNonQueryAsync();
        await connectionInsert.CloseAsync();
    }
}