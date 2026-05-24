using ClickExpress.BusinessLogic.Core.Review;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.Domain.Models.Review;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Functions.Review
{
    public class ReviewFlow : ReviewActions, IReviewActions
    {
        public List<ReviewDTO> GetAllReviewsAction(bool onlyApproved) => ExecuteGetAllReviewsAction(onlyApproved);
        public ReviewDTO? GetReviewByIdAction(int id) => ExecuteGetReviewByIdAction(id);
        public ResponseAction ResponseCreateReviewAction(int userId, CreateReviewDTO dto) => ExecuteCreateReviewAction(userId, dto);
        public ResponseMsg ResponseApproveReviewAction(int id) => ExecuteApproveReviewAction(id);
        public ResponseMsg ResponseRejectReviewAction(int id) => ExecuteRejectReviewAction(id);
        public int GetPendingCountAction() => ExecuteGetPendingCountAction();
        public ResponseMsg ResponseDeleteReviewAction(int id) => ExecuteDeleteReviewAction(id);
    }
}
