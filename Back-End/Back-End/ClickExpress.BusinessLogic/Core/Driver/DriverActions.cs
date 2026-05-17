using Microsoft.EntityFrameworkCore;
using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Entities.Driver;
using ClickExpress.Domain.Models.Driver;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Core.Driver
{
    public class DriverActions
    {
        protected List<DriverDTO> ExecuteGetAllDriversAction(string? status)
        {
            using (var db = new OrderContext())
            {
                var query = db.Drivers.Include(d => d.Vehicle).AsQueryable();
                if (!string.IsNullOrWhiteSpace(status)) query = query.Where(d => d.Status == status);

                return query.OrderBy(d => d.FullName).Select(d => MapToDTO(d)).ToList();
            }
        }

        protected DriverDTO? ExecuteGetDriverByIdAction(int id)
        {
            using (var db = new OrderContext())
            {
                var d = db.Drivers.Include(d => d.Vehicle).FirstOrDefault(d => d.Id == id);
                return d == null ? null : MapToDTO(d);
            }
        }

        protected ResponseAction ExecuteCreateDriverAction(CreateDriverDTO dto)
        {
            using (var db = new OrderContext())
            {
                if (dto.VehicleId.HasValue && db.Vehicles.Find(dto.VehicleId.Value) == null)
                    return new ResponseAction { IsSuccess = false, Message = "Vehicle not found!" };

                var driver = new DriverData
                {
                    FullName = dto.FullName, Phone = dto.Phone, CdlNumber = dto.CdlNumber,
                    Status = dto.Status, VehicleId = dto.VehicleId, CreatedAt = DateTime.UtcNow
                };
                db.Drivers.Add(driver);
                db.SaveChanges();
                return new ResponseAction { IsSuccess = true, Message = "Driver created!", Id = driver.Id };
            }
        }

        protected ResponseMsg ExecuteUpdateDriverAction(int id, UpdateDriverDTO dto)
        {
            using (var db = new OrderContext())
            {
                var driver = db.Drivers.FirstOrDefault(d => d.Id == id);
                if (driver == null)
                    return new ResponseMsg { IsSuccess = false, Message = "Driver not found!" };

                if (dto.VehicleId.HasValue && db.Vehicles.Find(dto.VehicleId.Value) == null)
                    return new ResponseMsg { IsSuccess = false, Message = "Vehicle not found!" };

                driver.FullName = dto.FullName; driver.Phone = dto.Phone;
                driver.CdlNumber = dto.CdlNumber; driver.Status = dto.Status;
                driver.VehicleId = dto.VehicleId;
                db.SaveChanges();
                return new ResponseMsg { IsSuccess = true, Message = "Driver updated!" };
            }
        }

        protected ResponseMsg ExecutePatchDriverStatusAction(int id, string status)
        {
            using (var db = new OrderContext())
            {
                var driver = db.Drivers.FirstOrDefault(d => d.Id == id);
                if (driver == null)
                    return new ResponseMsg { IsSuccess = false, Message = "Driver not found!" };

                driver.Status = status;
                db.SaveChanges();
                return new ResponseMsg { IsSuccess = true, Message = "Status updated!" };
            }
        }

        protected ResponseMsg ExecuteDeleteDriverAction(int id)
        {
            using (var db = new OrderContext())
            {
                var driver = db.Drivers.FirstOrDefault(d => d.Id == id);
                if (driver == null)
                    return new ResponseMsg { IsSuccess = false, Message = "Driver not found!" };

                db.Drivers.Remove(driver);
                db.SaveChanges();
                return new ResponseMsg { IsSuccess = true, Message = "Driver deleted!" };
            }
        }

        private static DriverDTO MapToDTO(DriverData d) => new DriverDTO
        {
            Id = d.Id, FullName = d.FullName, Phone = d.Phone, CdlNumber = d.CdlNumber,
            Status = d.Status, VehicleId = d.VehicleId, VehicleModel = d.Vehicle?.Model,
            CreatedAt = d.CreatedAt
        };
    }
}
