using back_end.Domain;

namespace back_end.DAL.Repositories
{
    public interface IVehicleRepository
    {
        List<Vehicle> GetAll(string? type, bool? available);
        Vehicle? GetById(int id);
        Vehicle Create(Vehicle vehicle);
        Vehicle? Update(int id, Action<Vehicle> apply);
        Vehicle? ToggleAvailability(int id);
        bool Delete(int id);
    }
}
