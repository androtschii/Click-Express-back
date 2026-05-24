using ClickExpress.Domain.Models.Review;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Interfaces
{
    public interface IReviewActions
    {
        List<ReviewDTO> GetAllReviewsAction(bool onlyApproved);
        ReviewDTO? GetReviewByIdAction(int id);
        ResponseAction ResponseCreateReviewAction(int userId, CreateReviewDTO dto);
        ResponseMsg ResponseApproveReviewAction(int id);
        ResponseMsg ResponseRejectReviewAction(int id);
        int GetPendingCountAction();
        ResponseMsg ResponseDeleteReviewAction(int id);
    }
}
