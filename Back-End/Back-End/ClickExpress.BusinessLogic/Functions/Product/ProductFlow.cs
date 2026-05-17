using ClickExpress.BusinessLogic.Core.Product;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.Domain.Models.Product;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Functions.Product
{
    public class ProductFlow : ProductActions, IProductActions
    {
        public (List<ProductDTO> Items, int Total) GetAllProductsAction(string? search, string? category, int page, int pageSize)
            => ExecuteGetAllProductsAction(search, category, page, pageSize);
        public ProductDTO? GetProductByIdAction(int id) => ExecuteGetProductByIdAction(id);
        public ResponseAction ResponseCreateProductAction(CreateProductDTO dto) => ExecuteCreateProductAction(dto);
        public ResponseMsg ResponseUpdateProductAction(int id, UpdateProductDTO dto) => ExecuteUpdateProductAction(id, dto);
        public ResponseMsg ResponseDeleteProductAction(int id) => ExecuteDeleteProductAction(id);
        public ResponseMsg ResponseUpdatePriceAction(int id, decimal price) => ExecuteUpdatePriceAction(id, price);
        public ResponseMsg ResponseUpdateImageAction(int id, string imageUrl) => ExecuteUpdateImageAction(id, imageUrl);
        public ResponseMsg ResponseToggleActiveAction(int id) => ExecuteToggleActiveAction(id);
        public ResponseMsg ResponseUpdateStockAction(int id, int quantity) => ExecuteUpdateStockAction(id, quantity);
        public object GetProductStatsAction() => ExecuteGetProductStatsAction();
    }
}
