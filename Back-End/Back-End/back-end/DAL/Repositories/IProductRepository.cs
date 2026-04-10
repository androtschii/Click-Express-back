using back_end.Domain;

namespace back_end.DAL.Repositories
{
    public interface IProductRepository
    {
        List<Product> GetAll(string? search, string? category);
        Product? GetById(int id);
        Product Create(Product product);
        Product? Update(int id, Product product);
        bool Delete(int id);
        Product? UpdatePrice(int id, decimal price);
        Product? UpdateImage(int id, string imageUrl);
        Product? ToggleActive(int id);
        Product? UpdateStock(int id, int quantity);
        object GetStats();
    }
}
