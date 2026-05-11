using back_end.Domain;

namespace back_end.DAL.Repositories
{
    public interface IReviewRepository
    {
        List<Review> GetAll(bool onlyApproved);
        Review? GetById(int id);
        Review Create(Review review);
        Review? Approve(int id);
        bool Delete(int id);
    }
}
