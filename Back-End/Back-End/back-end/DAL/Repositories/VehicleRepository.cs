using back_end.Domain;

namespace back_end.DAL.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly AppDbContext _db;
        public VehicleRepository(AppDbContext db) => _db = db;

        public List<Vehicle> GetAll(string? type, bool? available)
        {
            var query = _db.Vehicles.AsQueryable();
            if (!string.IsNullOrEmpty(type)) query = query.Where(v => v.Type == type);
            if (available.HasValue) query = query.Where(v => v.IsAvailable == available.Value);
            return query.OrderBy(v => v.Model).ToList();
        }

        public Vehicle? GetById(int id) => _db.Vehicles.Find(id);

        public Vehicle Create(Vehicle vehicle)
        {
            _db.Vehicles.Add(vehicle);
            _db.SaveChanges();
            return vehicle;
        }

        public Vehicle? Update(int id, Action<Vehicle> apply)
        {
            var vehicle = _db.Vehicles.Find(id);
            if (vehicle == null) return null;
            apply(vehicle);
            _db.SaveChanges();
            return vehicle;
        }

        public Vehicle? ToggleAvailability(int id)
        {
            var vehicle = _db.Vehicles.Find(id);
            if (vehicle == null) return null;
            vehicle.IsAvailable = !vehicle.IsAvailable;
            _db.SaveChanges();
            return vehicle;
        }

        public bool Delete(int id)
        {
            var vehicle = _db.Vehicles.Find(id);
            if (vehicle == null) return false;
            _db.Vehicles.Remove(vehicle);
            _db.SaveChanges();
            return true;
        }
    }
}
