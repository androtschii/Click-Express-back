using ClickExpress.BusinessLogic.Core.SavedLoad;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.Domain.Models.SavedLoad;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Functions.SavedLoad
{
    public class SavedLoadFlow : SavedLoadActions, ISavedLoadActions
    {
        public List<SavedLoadDTO> GetSavedLoadsAction(int userId) => ExecuteGetSavedLoadsAction(userId);
        public ResponseAction ResponseAddSavedLoadAction(int userId, int productId) => ExecuteAddSavedLoadAction(userId, productId);
        public ResponseMsg ResponseRemoveSavedLoadAction(int userId, int productId) => ExecuteRemoveSavedLoadAction(userId, productId);
    }
}
