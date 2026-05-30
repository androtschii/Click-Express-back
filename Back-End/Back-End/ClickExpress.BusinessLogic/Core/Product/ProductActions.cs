using Microsoft.EntityFrameworkCore;
using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Entities.Product;
using ClickExpress.Domain.Models.Product;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Core.Product
{
    public class ProductActions
    {
        protected (List<ProductDTO> Items, int Total) ExecuteGetAllProductsAction(
            string? search, string? category, decimal? minPrice, decimal? maxPrice, string? sortBy, int page, int pageSize)
        {
            using var db = new ProductContext();

            var query = db.Products.AsNoTracking().Where(p => p.IsActive && !p.IsDeleted).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(p => p.Category == category);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            int total = query.Count();

            query = sortBy switch
            {
                "price_asc"  => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "oldest"     => query.OrderBy(p => p.CreatedAt),
                "popular"    => query.OrderByDescending(p => p.ViewCount),
                _            => query.OrderByDescending(p => p.CreatedAt),
            };

            var items = query
                .Skip((page - 1) * pageSize).Take(pageSize)
                .Select(p => new ProductDTO
                {
                    Id = p.Id, Name = p.Name, Description = p.Description,
                    Price = p.Price, ImageUrl = p.ImageUrl, Category = p.Category,
                    Stock = p.Stock, IsActive = p.IsActive, CreatedAt = p.CreatedAt, ViewCount = p.ViewCount
                }).ToList();

            return (items, total);
        }

        protected ProductDTO? ExecuteGetProductByIdAction(int id)
        {
            using var db = new ProductContext();

            var p = db.Products.AsNoTracking().FirstOrDefault(p => p.Id == id && !p.IsDeleted);
            if (p == null) return null;
            return new ProductDTO
            {
                Id = p.Id, Name = p.Name, Description = p.Description,
                Price = p.Price, ImageUrl = p.ImageUrl, Category = p.Category,
                Stock = p.Stock, IsActive = p.IsActive, CreatedAt = p.CreatedAt
            };
        }

        protected ResponseAction ExecuteCreateProductAction(CreateProductDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return new ResponseAction { IsSuccess = false, Message = "Name is required!" };

            if (dto.Price <= 0)
                return new ResponseAction { IsSuccess = false, Message = "Price must be greater than 0!" };

            using var db = new ProductContext();

            var product = new ProductData
            {
                Name = dto.Name, Description = dto.Description, Price = dto.Price,
                ImageUrl = dto.ImageUrl, Category = dto.Category, Stock = dto.Stock,
                IsActive = true, CreatedAt = DateTime.UtcNow
            };
            db.Products.Add(product);
            db.SaveChanges();
            return new ResponseAction { IsSuccess = true, Message = "Product created!", Id = product.Id };
        }

        protected ResponseMsg ExecuteUpdateProductAction(int id, UpdateProductDTO dto)
        {
            using var db = new ProductContext();

            var product = db.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return new ResponseMsg { IsSuccess = false, Message = "Product not found!" };

            product.Name = dto.Name; product.Description = dto.Description;
            product.Price = dto.Price; product.ImageUrl = dto.ImageUrl;
            product.Category = dto.Category; product.Stock = dto.Stock;
            product.IsActive = dto.IsActive;
            db.SaveChanges();
            return new ResponseMsg { IsSuccess = true, Message = "Product updated!" };
        }

        protected ResponseMsg ExecuteDeleteProductAction(int id)
        {
            using var db = new ProductContext();

            var product = db.Products.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
            if (product == null)
                return new ResponseMsg { IsSuccess = false, Message = "Product not found!" };

            product.IsDeleted = true;
            product.DeletedAt = DateTime.UtcNow;
            product.IsActive = false;
            db.SaveChanges();
            return new ResponseMsg { IsSuccess = true, Message = "Product soft-deleted!" };
        }

        protected ResponseMsg ExecuteRestoreProductAction(int id)
        {
            using var db = new ProductContext();

            var product = db.Products.FirstOrDefault(p => p.Id == id && p.IsDeleted);
            if (product == null)
                return new ResponseMsg { IsSuccess = false, Message = "Deleted product not found!" };

            product.IsDeleted = false;
            product.DeletedAt = null;
            product.IsActive = true;
            db.SaveChanges();
            return new ResponseMsg { IsSuccess = true, Message = "Product restored!" };
        }

        protected List<ProductDTO> ExecuteGetDeletedProductsAction()
        {
            using var db = new ProductContext();

            return db.Products.AsNoTracking().Where(p => p.IsDeleted)
                .OrderByDescending(p => p.DeletedAt)
                .Select(p => new ProductDTO
                {
                    Id = p.Id, Name = p.Name, Description = p.Description,
                    Price = p.Price, ImageUrl = p.ImageUrl, Category = p.Category,
                    Stock = p.Stock, IsActive = p.IsActive, CreatedAt = p.CreatedAt, ViewCount = p.ViewCount
                }).ToList();
        }

        protected ResponseMsg ExecuteUpdatePriceAction(int id, decimal price)
        {
            if (price <= 0)
                return new ResponseMsg { IsSuccess = false, Message = "Price must be greater than 0!" };

            using var db = new ProductContext();

            var product = db.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return new ResponseMsg { IsSuccess = false, Message = "Product not found!" };

            product.Price = price;
            db.SaveChanges();
            return new ResponseMsg { IsSuccess = true, Message = "Price updated!" };
        }

        protected ResponseMsg ExecuteUpdateImageAction(int id, string imageUrl)
        {
            using var db = new ProductContext();

            var product = db.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return new ResponseMsg { IsSuccess = false, Message = "Product not found!" };

            product.ImageUrl = imageUrl;
            db.SaveChanges();
            return new ResponseMsg { IsSuccess = true, Message = "Image updated!" };
        }

        protected ResponseMsg ExecuteToggleActiveAction(int id)
        {
            using var db = new ProductContext();

            var product = db.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return new ResponseMsg { IsSuccess = false, Message = "Product not found!" };

            product.IsActive = !product.IsActive;
            db.SaveChanges();
            return new ResponseMsg { IsSuccess = true, Message = product.IsActive ? "Product activated!" : "Product deactivated!" };
        }

        protected ResponseMsg ExecuteUpdateStockAction(int id, int quantity)
        {
            if (quantity < 0)
                return new ResponseMsg { IsSuccess = false, Message = "Quantity cannot be negative!" };

            using var db = new ProductContext();

            var product = db.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return new ResponseMsg { IsSuccess = false, Message = "Product not found!" };

            product.Stock = quantity;
            db.SaveChanges();
            return new ResponseMsg { IsSuccess = true, Message = "Stock updated!" };
        }

        protected object ExecuteGetProductStatsAction()
        {
            using var db = new ProductContext();

            var products = db.Products.AsNoTracking().ToList();
            return new
            {
                Total      = products.Count,
                Active     = products.Count(p => p.IsActive),
                OutOfStock = products.Count(p => p.Stock == 0 && p.IsActive),
                Categories = products.Select(p => p.Category).Distinct().Count(),
                TotalValue = products.Where(p => p.IsActive).Sum(p => p.Price * p.Stock)
            };
        }

        protected ResponseMsg ExecuteIncrementViewAction(int id)
        {
            using var db = new ProductContext();

            var p = db.Products.FirstOrDefault(p => p.Id == id);
            if (p == null) return new ResponseMsg { IsSuccess = false, Message = "Not found" };
            p.ViewCount++;
            db.SaveChanges();
            return new ResponseMsg { IsSuccess = true, Message = "ok" };
        }
    }
}
