using ClickExpress.Domain.Models.SavedLoad;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Interfaces
{
    public interface ISavedLoadActions
    {
        List<SavedLoadDTO> GetSavedLoadsAction(int userId);
        ResponseAction ResponseAddSavedLoadAction(int userId, int productId);
        ResponseMsg ResponseRemoveSavedLoadAction(int userId, int productId);
    }
}
