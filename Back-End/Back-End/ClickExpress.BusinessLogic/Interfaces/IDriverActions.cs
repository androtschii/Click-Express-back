using ClickExpress.Domain.Models.Driver;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Interfaces
{
    public interface IDriverActions
    {
        List<DriverDTO> GetAllDriversAction(string? status);
        PagedResult<DriverDTO> GetDriversPagedAction(QueryOptions opts, string? status);
        DriverDTO? GetDriverByIdAction(int id);
        ResponseAction ResponseCreateDriverAction(CreateDriverDTO dto);
        ResponseMsg ResponseUpdateDriverAction(int id, UpdateDriverDTO dto);
        ResponseMsg ResponsePatchDriverStatusAction(int id, string status);
        ResponseMsg ResponseDeleteDriverAction(int id);
        List<DriverDTO> GetDeletedDriversAction();
        ResponseMsg RestoreDriverAction(int id);
    }
}
