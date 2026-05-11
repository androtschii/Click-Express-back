using Microsoft.EntityFrameworkCore;
using back_end.Domain;

namespace back_end.DAL.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _db;
        public ReviewRepository(AppDbContext db) => _db = db;

        public List<Review> GetAll(bool onlyApproved)
        {
            var query = _db.Reviews.Include(r => r.User).AsQueryable();
            if (onlyApproved) query = query.Where(r => r.IsApproved);
            return query.OrderByDescending(r => r.CreatedAt).ToList();
        }

        public Review? GetById(int id)
            => _db.Reviews.Include(r => r.User).FirstOrDefault(r => r.Id == id);

        public Review Create(Review review)
        {
            _db.Reviews.Add(review);
            _db.SaveChanges();
            return _db.Reviews.Include(r => r.User).First(r => r.Id == review.Id);
        }

        public Review? Approve(int id)
        {
            var review = _db.Reviews.Find(id);
            if (review == null) return null;
            review.IsApproved = true;
            _db.SaveChanges();
            return _db.Reviews.Include(r => r.User).First(r => r.Id == id);
        }

        public bool Delete(int id)
        {
            var review = _db.Reviews.Find(id);
            if (review == null) return false;
            _db.Reviews.Remove(review);
            _db.SaveChanges();
            return true;
        }
    }
}
