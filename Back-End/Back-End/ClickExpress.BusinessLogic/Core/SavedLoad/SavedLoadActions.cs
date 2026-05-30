using Microsoft.EntityFrameworkCore;
using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Entities.SavedLoad;
using ClickExpress.Domain.Models.SavedLoad;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Core.SavedLoad
{
    public class SavedLoadActions
    {
        protected List<SavedLoadDTO> ExecuteGetSavedLoadsAction(int userId)
        {
            using (var db = new OrderContext())
            {
                return db.SavedLoads
                    .AsNoTracking()
                    .Where(sl => sl.UserId == userId)
                    .OrderByDescending(sl => sl.CreatedAt)
                    .Select(sl => new SavedLoadDTO
                    {
                        Id = sl.Id, ProductId = sl.ProductId, CreatedAt = sl.CreatedAt,
                        ProductName = sl.Product.Name, ProductPrice = sl.Product.Price,
                        ProductImageUrl = sl.Product.ImageUrl, ProductCategory = sl.Product.Category,
                        ProductDescription = sl.Product.Description
                    }).ToList();
            }
        }

        protected ResponseAction ExecuteAddSavedLoadAction(int userId, int productId)
        {
            using (var db = new OrderContext())
            {
                var product = db.Products.Find(productId);
                if (product == null)
                    return new ResponseAction { IsSuccess = false, Message = "Product not found!" };

                var exists = db.SavedLoads.Any(sl => sl.UserId == userId && sl.ProductId == productId);
                if (exists)
                    return new ResponseAction { IsSuccess = false, Message = "Already in favorites!" };

                var saved = new SavedLoadData { UserId = userId, ProductId = productId, CreatedAt = DateTime.UtcNow };
                db.SavedLoads.Add(saved);
                db.SaveChanges();
                return new ResponseAction { IsSuccess = true, Message = "Added to favorites!", Id = saved.Id };
            }
        }

        protected ResponseMsg ExecuteRemoveSavedLoadAction(int userId, int productId)
        {
            using (var db = new OrderContext())
            {
                var saved = db.SavedLoads.FirstOrDefault(sl => sl.UserId == userId && sl.ProductId == productId);
                if (saved == null)
                    return new ResponseMsg { IsSuccess = false, Message = "Favorite not found!" };

                db.SavedLoads.Remove(saved);
                db.SaveChanges();
                return new ResponseMsg { IsSuccess = true, Message = "Removed from favorites!" };
            }
        }
    }
}
