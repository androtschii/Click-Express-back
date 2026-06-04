using ClickExpress.Domain.Models.Vehicle;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Interfaces
{
    public interface IVehicleActions
    {
        List<VehicleDTO> GetAllVehiclesAction(string? type, bool? available);
        PagedResult<VehicleDTO> GetVehiclesPagedAction(QueryOptions opts, string? type, bool? available);
        VehicleDTO? GetVehicleByIdAction(int id);
        ResponseAction ResponseCreateVehicleAction(CreateVehicleDTO dto);
        ResponseMsg ResponseUpdateVehicleAction(int id, UpdateVehicleDTO dto);
        ResponseMsg ResponseToggleAvailabilityAction(int id);
        ResponseMsg ResponseDeleteVehicleAction(int id);
    }
}
