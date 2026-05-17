using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Entities.Vehicle;
using ClickExpress.Domain.Models.Vehicle;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Core.Vehicle
{
    public class VehicleActions
    {
        protected List<VehicleDTO> ExecuteGetAllVehiclesAction(string? type, bool? available)
        {
            using (var db = new OrderContext())
            {
                var query = db.Vehicles.AsQueryable();
                if (!string.IsNullOrWhiteSpace(type)) query = query.Where(v => v.Type == type);
                if (available.HasValue) query = query.Where(v => v.IsAvailable == available.Value);

                return query.OrderBy(v => v.Model).Select(v => MapToDTO(v)).ToList();
            }
        }

        protected VehicleDTO? ExecuteGetVehicleByIdAction(int id)
        {
            using (var db = new OrderContext())
            {
                var v = db.Vehicles.FirstOrDefault(v => v.Id == id);
                return v == null ? null : MapToDTO(v);
            }
        }

        protected ResponseAction ExecuteCreateVehicleAction(CreateVehicleDTO dto)
        {
            using (var db = new OrderContext())
            {
                var vehicle = new VehicleData
                {
                    Model = dto.Model, PlateNumber = dto.PlateNumber, Type = dto.Type,
                    Capacity = dto.Capacity, Year = dto.Year, IsAvailable = dto.IsAvailable,
                    ImageUrl = dto.ImageUrl, CreatedAt = DateTime.Now
                };
                db.Vehicles.Add(vehicle);
                db.SaveChanges();
                return new ResponseAction { IsSuccess = true, Message = "Vehicle created!", Id = vehicle.Id };
            }
        }

        protected ResponseMsg ExecuteUpdateVehicleAction(int id, UpdateVehicleDTO dto)
        {
            using (var db = new OrderContext())
            {
                var vehicle = db.Vehicles.FirstOrDefault(v => v.Id == id);
                if (vehicle == null)
                    return new ResponseMsg { IsSuccess = false, Message = "Vehicle not found!" };

                vehicle.Model = dto.Model; vehicle.PlateNumber = dto.PlateNumber;
                vehicle.Type = dto.Type; vehicle.Capacity = dto.Capacity;
                vehicle.Year = dto.Year; vehicle.IsAvailable = dto.IsAvailable;
                vehicle.ImageUrl = dto.ImageUrl;
                db.SaveChanges();
                return new ResponseMsg { IsSuccess = true, Message = "Vehicle updated!" };
            }
        }

        protected ResponseMsg ExecuteToggleAvailabilityAction(int id)
        {
            using (var db = new OrderContext())
            {
                var vehicle = db.Vehicles.FirstOrDefault(v => v.Id == id);
                if (vehicle == null)
                    return new ResponseMsg { IsSuccess = false, Message = "Vehicle not found!" };

                vehicle.IsAvailable = !vehicle.IsAvailable;
                db.SaveChanges();
                return new ResponseMsg { IsSuccess = true, Message = vehicle.IsAvailable ? "Vehicle available!" : "Vehicle unavailable!" };
            }
        }

        protected ResponseMsg ExecuteDeleteVehicleAction(int id)
        {
            using (var db = new OrderContext())
            {
                var vehicle = db.Vehicles.FirstOrDefault(v => v.Id == id);
                if (vehicle == null)
                    return new ResponseMsg { IsSuccess = false, Message = "Vehicle not found!" };

                db.Vehicles.Remove(vehicle);
                db.SaveChanges();
                return new ResponseMsg { IsSuccess = true, Message = "Vehicle deleted!" };
            }
        }

        private static VehicleDTO MapToDTO(VehicleData v) => new VehicleDTO
        {
            Id = v.Id, Model = v.Model, PlateNumber = v.PlateNumber, Type = v.Type,
            Capacity = v.Capacity, Year = v.Year, IsAvailable = v.IsAvailable,
            ImageUrl = v.ImageUrl, CreatedAt = v.CreatedAt
        };
    }
}
