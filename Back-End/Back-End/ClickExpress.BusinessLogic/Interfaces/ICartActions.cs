using ClickExpress.Domain.Models.Cart;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Interfaces
{
    public interface ICartActions
    {
        CartDTO GetOrCreateCartAction(int userId);
        ResponseAction ResponseAddCartItemAction(int userId, int productId, int quantity);
        ResponseMsg ResponseRemoveCartItemAction(int userId, int itemId);
        ResponseMsg ResponseClearCartAction(int userId);
    }
}
