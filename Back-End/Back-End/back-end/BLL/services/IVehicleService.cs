using back_end.BLL.DTOs;

namespace back_end.BLL.Services
{
    public interface IVehicleService
    {
        List<VehicleDto> GetAll(string? type, bool? available);
        VehicleDto? GetById(int id);
        VehicleDto Create(CreateVehicleDto dto);
        VehicleDto? Update(int id, UpdateVehicleDto dto);
        VehicleDto? ToggleAvailability(int id);
        bool Delete(int id);
    }
}
