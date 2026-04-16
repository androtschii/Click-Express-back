using back_end.BLL.DTOs;

namespace back_end.BLL.Services
{
    public interface IProductService
    {
        List<ProductDto> GetAll(string? search, string? category);
        ProductDto? GetById(int id);
        ProductDto Create(CreateProductDto dto);
        ProductDto? Update(int id, UpdateProductDto dto);
        bool Delete(int id);
        ProductDto? UpdatePrice(int id, decimal price);
        ProductDto? UpdateImage(int id, string imageUrl);
        ProductDto? ToggleActive(int id);
        ProductDto? UpdateStock(int id, int quantity);
        object GetStats();
    }
}
