using ClickExpress.BusinessLogic.Core.Cart;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.Domain.Models.Cart;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Functions.Cart
{
    public class CartFlow : CartActions, ICartActions
    {
        public CartDTO GetOrCreateCartAction(int userId) => ExecuteGetOrCreateCartAction(userId);
        public ResponseAction ResponseAddCartItemAction(int userId, int productId, int quantity) => ExecuteAddCartItemAction(userId, productId, quantity);
        public ResponseMsg ResponseRemoveCartItemAction(int userId, int itemId) => ExecuteRemoveCartItemAction(userId, itemId);
        public ResponseMsg ResponseClearCartAction(int userId) => ExecuteClearCartAction(userId);
    }
}
