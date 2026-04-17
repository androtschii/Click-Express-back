using back_end.BLL.DTOs;

namespace back_end.BLL.Services
{
    public interface IProductService
    {
        List<ProductDto> GetAll();
        ProductDto? GetById(int id);
        ProductDto Create(ProductDto dto);
        ProductDto? Update(int id, ProductDto dto);
        bool Delete(int id);
    }
}