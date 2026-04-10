using back_end.Domain;
using back_end.DAL;

namespace back_end.DAL.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _db;
        public ProductRepository(AppDbContext db) { _db = db; }

        public List<Product> GetAll(string? search, string? category)
        {
            var query = _db.Products.AsQueryable();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.Category == category);
            return query.ToList();
        }

        public Product? GetById(int id) => _db.Products.Find(id);

        public Product Create(Product product)
        {
            _db.Products.Add(product);
            _db.SaveChanges();
            return product;
        }

        public Product? Update(int id, Product updated)
        {
            var product = _db.Products.Find(id);
            if (product == null) return null;
            product.Name = updated.Name;
            product.Description = updated.Description;
            product.Price = updated.Price;
            product.ImageUrl = updated.ImageUrl;
            product.Category = updated.Category;
            product.Stock = updated.Stock;
            product.IsActive = updated.IsActive;
            _db.SaveChanges();
            return product;
        }

        public bool Delete(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null) return false;
            _db.Products.Remove(product);
            _db.SaveChanges();
            return true;
        }

        public Product? UpdatePrice(int id, decimal price)
        {
            var product = _db.Products.Find(id);
            if (product == null) return null;
            product.Price = price;
            _db.SaveChanges();
            return product;
        }

        public Product? UpdateImage(int id, string imageUrl)
        {
            var product = _db.Products.Find(id);
            if (product == null) return null;
            product.ImageUrl = imageUrl;
            _db.SaveChanges();
            return product;
        }

        public Product? ToggleActive(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null) return null;
            product.IsActive = !product.IsActive;
            _db.SaveChanges();
            return product;
        }

        public Product? UpdateStock(int id, int quantity)
        {
            var product = _db.Products.Find(id);
            if (product == null) return null;
            product.Stock = quantity;
            _db.SaveChanges();
            return product;
        }

        public object GetStats()
        {
            return new
            {
                TotalProducts = _db.Products.Count(),
                ActiveProducts = _db.Products.Count(p => p.IsActive),
                OutOfStock = _db.Products.Count(p => p.Stock == 0),
                TotalValue = _db.Products.Sum(p => p.Price * p.Stock),
                Categories = _db.Products.Select(p => p.Category).Distinct().Count()
            };
        }
    }
}
