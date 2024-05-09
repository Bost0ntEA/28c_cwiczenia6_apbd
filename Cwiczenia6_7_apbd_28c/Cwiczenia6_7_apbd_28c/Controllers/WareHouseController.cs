using System.Transactions;
using Microsoft.AspNetCore.Mvc;

namespace Cwiczenia6_7_apbd_28c.Controllers;

[ApiController]
public class WarehouseController : ControllerBase
{
     private readonly IProductRepository _productRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductWarehouse _productWarehouse;
    public WarehouseController(IProductRepository productRepository, IWarehouseRepository warehouseRepository, IOrderRepository orderRepository, IProductWarehouse productWarehouse)
    {
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
        _orderRepository = orderRepository;
        _productWarehouse = productWarehouse;
    }
    
    [HttpPost]
    [Route("api/scope/{idProduct:int}/{idWarehouse:int}/{amount:int}/{createdAt:datetime}")]
    public async Task<IActionResult> AddRecordToProduct_Warehouse(int idProduct, int idWarehouse, int amount, DateTime createdAt)
    {
        if(! await _productRepository.DoesProductExist(idProduct))
            return NotFound("produkt nie istnieje");

        if (!await _warehouseRepository.DoesWarehouseExist(idWarehouse))
            return NotFound("Warehouse nie istnieje");
        
        var idOrder = await _orderRepository.DoesOrderWithAmountProductExist(idProduct, amount, createdAt);
        if (idOrder == -1)
            return NotFound("zamowienei z taka iloscia nie istnieje");

        if (!await _orderRepository.DoesFulfilledAt(idOrder))
            return NotFound("to zamowienie nie jest skonczone");
        
        if (await _productWarehouse.DoesOrderExistInProductWarehouse(idOrder))
            return NotFound("zamownieniei juz istnieje w warehousie");
        
        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            await _orderRepository.UpdateFulfilledAt(idOrder);
            await _productWarehouse.AddRecordToProduct_Warehouse(idProduct, idWarehouse, amount, createdAt, idOrder);
            
            scope.Complete();
        }
        
        
        return Ok();
    }
}