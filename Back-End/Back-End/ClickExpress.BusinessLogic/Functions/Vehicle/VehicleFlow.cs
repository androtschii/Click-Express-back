using ClickExpress.BusinessLogic.Core.Vehicle;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.Domain.Models.Vehicle;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Functions.Vehicle
{
    public class VehicleFlow : VehicleActions, IVehicleActions
    {
        public List<VehicleDTO> GetAllVehiclesAction(string? type, bool? available) => ExecuteGetAllVehiclesAction(type, available);
        public PagedResult<VehicleDTO> GetVehiclesPagedAction(QueryOptions opts, string? type, bool? available) => ExecuteGetVehiclesPagedAction(opts, type, available);
        public VehicleDTO? GetVehicleByIdAction(int id) => ExecuteGetVehicleByIdAction(id);
        public ResponseAction ResponseCreateVehicleAction(CreateVehicleDTO dto) => ExecuteCreateVehicleAction(dto);
        public ResponseMsg ResponseUpdateVehicleAction(int id, UpdateVehicleDTO dto) => ExecuteUpdateVehicleAction(id, dto);
        public ResponseMsg ResponseToggleAvailabilityAction(int id) => ExecuteToggleAvailabilityAction(id);
        public ResponseMsg ResponseDeleteVehicleAction(int id) => ExecuteDeleteVehicleAction(id);
    }
}
