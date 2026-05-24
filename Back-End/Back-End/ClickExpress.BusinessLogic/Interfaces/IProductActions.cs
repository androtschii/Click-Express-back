using ClickExpress.Domain.Models.Product;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Interfaces
{
    public interface IProductActions
    {
        (List<ProductDTO> Items, int Total) GetAllProductsAction(string? search, string? category, int page, int pageSize);
        ProductDTO? GetProductByIdAction(int id);
        ResponseAction ResponseCreateProductAction(CreateProductDTO dto);
        ResponseMsg ResponseUpdateProductAction(int id, UpdateProductDTO dto);
        ResponseMsg ResponseDeleteProductAction(int id);
        ResponseMsg ResponseUpdatePriceAction(int id, decimal price);
        ResponseMsg ResponseUpdateImageAction(int id, string imageUrl);
        ResponseMsg ResponseToggleActiveAction(int id);
        ResponseMsg ResponseUpdateStockAction(int id, int quantity);
        object GetProductStatsAction();
        ResponseMsg ResponseIncrementViewAction(int id);
    }
}
