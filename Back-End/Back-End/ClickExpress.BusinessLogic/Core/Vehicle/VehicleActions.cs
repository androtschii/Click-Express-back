using Microsoft.EntityFrameworkCore;
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
            using var db = new OrderContext();

            return db.Vehicles
                .AsNoTracking()
                .Where(v => (string.IsNullOrWhiteSpace(type) || v.Type == type)
                         && (!available.HasValue || v.IsAvailable == available.Value))
                .OrderBy(v => v.Model)
                .Select(v => new VehicleDTO
                {
                    Id = v.Id,
                    Model = v.Model,
                    PlateNumber = v.PlateNumber,
                    Type = v.Type,
                    Capacity = v.Capacity,
                    Year = v.Year,
                    IsAvailable = v.IsAvailable,
                    ImageUrl = v.ImageUrl,
                    CreatedAt = v.CreatedAt
                })
                .ToList();
        }

        protected VehicleDTO? ExecuteGetVehicleByIdAction(int id)
        {
            using var db = new OrderContext();

            return db.Vehicles
                .AsNoTracking()
                .Where(v => v.Id == id)
                .Select(v => new VehicleDTO
                {
                    Id = v.Id,
                    Model = v.Model,
                    PlateNumber = v.PlateNumber,
                    Type = v.Type,
                    Capacity = v.Capacity,
                    Year = v.Year,
                    IsAvailable = v.IsAvailable,
                    ImageUrl = v.ImageUrl,
                    CreatedAt = v.CreatedAt
                })
                .FirstOrDefault();
        }

        protected ResponseAction ExecuteCreateVehicleAction(CreateVehicleDTO dto)
        {
            using var db = new OrderContext();

            var vehicle = new VehicleData
            {
                Model = dto.Model,
                PlateNumber = dto.PlateNumber,
                Type = dto.Type,
                Capacity = dto.Capacity,
                Year = dto.Year,
                IsAvailable = dto.IsAvailable,
                ImageUrl = dto.ImageUrl,
                CreatedAt = DateTime.UtcNow
            };

            db.Vehicles.Add(vehicle);
            db.SaveChanges();

            return new ResponseAction { IsSuccess = true, Message = "Vehicle created!", Id = vehicle.Id };
        }

        protected ResponseMsg ExecuteUpdateVehicleAction(int id, UpdateVehicleDTO dto)
        {
            using var db = new OrderContext();

            var vehicle = db.Vehicles.FirstOrDefault(v => v.Id == id);
            if (vehicle == null)
                return new ResponseMsg { IsSuccess = false, Message = "Vehicle not found!" };

            vehicle.Model = dto.Model;
            vehicle.PlateNumber = dto.PlateNumber;
            vehicle.Type = dto.Type;
            vehicle.Capacity = dto.Capacity;
            vehicle.Year = dto.Year;
            vehicle.IsAvailable = dto.IsAvailable;
            vehicle.ImageUrl = dto.ImageUrl;
            db.SaveChanges();

            return new ResponseMsg { IsSuccess = true, Message = "Vehicle updated!" };
        }

        protected ResponseMsg ExecuteToggleAvailabilityAction(int id)
        {
            using var db = new OrderContext();

            var vehicle = db.Vehicles.FirstOrDefault(v => v.Id == id);
            if (vehicle == null)
                return new ResponseMsg { IsSuccess = false, Message = "Vehicle not found!" };

            vehicle.IsAvailable = !vehicle.IsAvailable;
            db.SaveChanges();

            return new ResponseMsg
            {
                IsSuccess = true,
                Message = vehicle.IsAvailable ? "Vehicle available!" : "Vehicle unavailable!"
            };
        }

        protected ResponseMsg ExecuteDeleteVehicleAction(int id)
        {
            using var db = new OrderContext();

            var vehicle = db.Vehicles.FirstOrDefault(v => v.Id == id);
            if (vehicle == null)
                return new ResponseMsg { IsSuccess = false, Message = "Vehicle not found!" };

            db.Vehicles.Remove(vehicle);
            db.SaveChanges();

            return new ResponseMsg { IsSuccess = true, Message = "Vehicle deleted!" };
        }

        protected PagedResult<VehicleDTO> ExecuteGetVehiclesPagedAction(QueryOptions opts, string? type, bool? available)
        {
            using var db = new OrderContext();

            var query = db.Vehicles
                .AsNoTracking()
                .Where(v => (string.IsNullOrWhiteSpace(type) || v.Type == type)
                         && (!available.HasValue || v.IsAvailable == available.Value));

            if (!string.IsNullOrWhiteSpace(opts.Search))
                query = query.Where(v => v.Model.Contains(opts.Search) || v.PlateNumber.Contains(opts.Search));

            var total = query.Count();

            var orderedQuery = opts.Sort?.ToLower() switch
            {
                "capacity" => query.OrderBy(v => v.Capacity),
                "year" => query.OrderByDescending(v => v.Year),
                _ => query.OrderBy(v => v.Model)
            };

            var items = orderedQuery
                .Skip((opts.Page - 1) * opts.PageSize)
                .Take(opts.PageSize)
                .Select(v => new VehicleDTO
                {
                    Id = v.Id,
                    Model = v.Model,
                    PlateNumber = v.PlateNumber,
                    Type = v.Type,
                    Capacity = v.Capacity,
                    Year = v.Year,
                    IsAvailable = v.IsAvailable,
                    ImageUrl = v.ImageUrl,
                    CreatedAt = v.CreatedAt
                })
                .ToList();

            return new PagedResult<VehicleDTO>
            {
                Items = items,
                TotalCount = total,
                Page = opts.Page,
                PageSize = opts.PageSize
            };
        }
    }
}
