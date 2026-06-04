using ClickExpress.BusinessLogic.Core.Driver;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.Domain.Models.Driver;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Functions.Driver
{
    public class DriverFlow : DriverActions, IDriverActions
    {
        public List<DriverDTO> GetAllDriversAction(string? status) => ExecuteGetAllDriversAction(status);
        public PagedResult<DriverDTO> GetDriversPagedAction(QueryOptions opts, string? status) => ExecuteGetDriversPagedAction(opts, status);
        public DriverDTO? GetDriverByIdAction(int id) => ExecuteGetDriverByIdAction(id);
        public ResponseAction ResponseCreateDriverAction(CreateDriverDTO dto) => ExecuteCreateDriverAction(dto);
        public ResponseMsg ResponseUpdateDriverAction(int id, UpdateDriverDTO dto) => ExecuteUpdateDriverAction(id, dto);
        public ResponseMsg ResponsePatchDriverStatusAction(int id, string status) => ExecutePatchDriverStatusAction(id, status);
        public ResponseMsg ResponseDeleteDriverAction(int id) => ExecuteDeleteDriverAction(id);
        public List<DriverDTO> GetDeletedDriversAction() => ExecuteGetDeletedDriversAction();
        public ResponseMsg RestoreDriverAction(int id) => ExecuteRestoreDriverAction(id);
    }
}
