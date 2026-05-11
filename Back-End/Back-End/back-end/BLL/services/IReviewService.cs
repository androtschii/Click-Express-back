using back_end.BLL.DTOs;

namespace back_end.BLL.Services
{
    public interface IReviewService
    {
        List<ReviewDto> GetAll(bool onlyApproved);
        ReviewDto? GetById(int id);
        ReviewDto Create(int userId, CreateReviewDto dto);
        ReviewDto? Approve(int id);
        bool Delete(int id);
    }
}
