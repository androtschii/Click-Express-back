using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Entities.Review;
using ClickExpress.Domain.Models.Review;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Core.Review
{
    public class ReviewActions
    {
        protected List<ReviewDTO> ExecuteGetAllReviewsAction(bool onlyApproved)
        {
            using var db = new OrderContext();

            return db.Reviews
                .AsNoTracking()
                .Where(r => !onlyApproved || r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewDTO
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    Username = r.User.Username,
                    ProductId = r.ProductId,
                    Rating = r.Rating,
                    Text = r.Text,
                    CreatedAt = r.CreatedAt,
                    IsApproved = r.IsApproved,
                    Role = r.Role,
                    Location = r.Location
                })
                .ToList();
        }

        protected ReviewDTO? ExecuteGetReviewByIdAction(int id)
        {
            using var db = new OrderContext();

            return db.Reviews
                .AsNoTracking()
                .Where(r => r.Id == id)
                .Select(r => new ReviewDTO
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    Username = r.User.Username,
                    ProductId = r.ProductId,
                    Rating = r.Rating,
                    Text = r.Text,
                    CreatedAt = r.CreatedAt,
                    IsApproved = r.IsApproved,
                    Role = r.Role,
                    Location = r.Location
                })
                .FirstOrDefault();
        }

        protected ResponseAction ExecuteCreateReviewAction(int userId, CreateReviewDTO dto)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
                return new ResponseAction { IsSuccess = false, Message = "Rating must be between 1 and 5!" };

            if (string.IsNullOrWhiteSpace(dto.Text))
                return new ResponseAction { IsSuccess = false, Message = "Text is required!" };

            using var db = new OrderContext();

            var review = new ReviewData
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Rating = dto.Rating,
                Text = dto.Text,
                IsApproved = false,
                CreatedAt = DateTime.UtcNow,
                Role = dto.Role,
                Location = dto.Location
            };

            db.Reviews.Add(review);
            db.SaveChanges();

            return new ResponseAction { IsSuccess = true, Message = "Review submitted!", Id = review.Id };
        }

        protected ResponseMsg ExecuteApproveReviewAction(int id)
        {
            using var db = new OrderContext();

            var review = db.Reviews.FirstOrDefault(r => r.Id == id);
            if (review == null)
                return new ResponseMsg { IsSuccess = false, Message = "Review not found!" };

            review.IsApproved = true;
            db.SaveChanges();

            return new ResponseMsg { IsSuccess = true, Message = "Review approved!" };
        }

        protected ResponseMsg ExecuteRejectReviewAction(int id)
        {
            using var db = new OrderContext();

            var review = db.Reviews.FirstOrDefault(r => r.Id == id);
            if (review == null)
                return new ResponseMsg { IsSuccess = false, Message = "Review not found!" };

            review.IsApproved = false;
            db.SaveChanges();

            return new ResponseMsg { IsSuccess = true, Message = "Review rejected!" };
        }

        protected int ExecuteGetPendingCountAction()
        {
            using var db = new OrderContext();
            return db.Reviews.Count(r => !r.IsApproved);
        }

        protected ResponseMsg ExecuteDeleteReviewAction(int id)
        {
            using var db = new OrderContext();

            var review = db.Reviews.FirstOrDefault(r => r.Id == id);
            if (review == null)
                return new ResponseMsg { IsSuccess = false, Message = "Review not found!" };

            db.Reviews.Remove(review);
            db.SaveChanges();

            return new ResponseMsg { IsSuccess = true, Message = "Review deleted!" };
        }

        protected ResponseMsg ExecuteUpdateReviewAction(int id, int userId, UpdateReviewDTO dto)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
                return new ResponseMsg { IsSuccess = false, Message = "Rating must be between 1 and 5!" };

            if (string.IsNullOrWhiteSpace(dto.Text))
                return new ResponseMsg { IsSuccess = false, Message = "Text is required!" };

            using var db = new OrderContext();

            var review = db.Reviews.FirstOrDefault(r => r.Id == id && r.UserId == userId);
            if (review == null)
                return new ResponseMsg { IsSuccess = false, Message = "Review not found!" };

            review.Rating = dto.Rating;
            review.Text = dto.Text;
            review.Role = dto.Role;
            review.Location = dto.Location;
            review.IsApproved = false;
            db.SaveChanges();

            return new ResponseMsg { IsSuccess = true, Message = "Review updated!" };
        }
    }
}
