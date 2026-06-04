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
            using var db = new OrderContext();

            return db.Drivers
                .AsNoTracking()
                .Where(d => !d.IsDeleted
                         && (string.IsNullOrWhiteSpace(status) || d.Status == status))
                .OrderBy(d => d.FullName)
                .Select(d => new DriverDTO
                {
                    Id = d.Id,
                    FullName = d.FullName,
                    Phone = d.Phone,
                    CdlNumber = d.CdlNumber,
                    Status = d.Status,
                    VehicleId = d.VehicleId,
                    VehicleModel = d.Vehicle != null ? d.Vehicle.Model : null,
                    CreatedAt = d.CreatedAt
                })
                .ToList();
        }

        protected DriverDTO? ExecuteGetDriverByIdAction(int id)
        {
            using var db = new OrderContext();

            return db.Drivers
                .AsNoTracking()
                .Where(d => d.Id == id && !d.IsDeleted)
                .Select(d => new DriverDTO
                {
                    Id = d.Id,
                    FullName = d.FullName,
                    Phone = d.Phone,
                    CdlNumber = d.CdlNumber,
                    Status = d.Status,
                    VehicleId = d.VehicleId,
                    VehicleModel = d.Vehicle != null ? d.Vehicle.Model : null,
                    CreatedAt = d.CreatedAt
                })
                .FirstOrDefault();
        }

        protected ResponseAction ExecuteCreateDriverAction(CreateDriverDTO dto)
        {
            using var db = new OrderContext();

            if (dto.VehicleId.HasValue && db.Vehicles.Find(dto.VehicleId.Value) == null)
                return new ResponseAction { IsSuccess = false, Message = "Vehicle not found!" };

            var driver = new DriverData
            {
                FullName = dto.FullName,
                Phone = dto.Phone,
                CdlNumber = dto.CdlNumber,
                Status = dto.Status,
                VehicleId = dto.VehicleId,
                CreatedAt = DateTime.UtcNow
            };

            db.Drivers.Add(driver);
            db.SaveChanges();

            return new ResponseAction { IsSuccess = true, Message = "Driver created!", Id = driver.Id };
        }

        protected ResponseMsg ExecuteUpdateDriverAction(int id, UpdateDriverDTO dto)
        {
            using var db = new OrderContext();

            var driver = db.Drivers.FirstOrDefault(d => d.Id == id);
            if (driver == null)
                return new ResponseMsg { IsSuccess = false, Message = "Driver not found!" };

            if (dto.VehicleId.HasValue && db.Vehicles.Find(dto.VehicleId.Value) == null)
                return new ResponseMsg { IsSuccess = false, Message = "Vehicle not found!" };

            driver.FullName = dto.FullName;
            driver.Phone = dto.Phone;
            driver.CdlNumber = dto.CdlNumber;
            driver.Status = dto.Status;
            driver.VehicleId = dto.VehicleId;
            db.SaveChanges();

            return new ResponseMsg { IsSuccess = true, Message = "Driver updated!" };
        }

        protected ResponseMsg ExecutePatchDriverStatusAction(int id, string status)
        {
            using var db = new OrderContext();

            var driver = db.Drivers.FirstOrDefault(d => d.Id == id);
            if (driver == null)
                return new ResponseMsg { IsSuccess = false, Message = "Driver not found!" };

            driver.Status = status;
            db.SaveChanges();

            return new ResponseMsg { IsSuccess = true, Message = "Status updated!" };
        }

        protected ResponseMsg ExecuteDeleteDriverAction(int id)
        {
            using var db = new OrderContext();

            var driver = db.Drivers.FirstOrDefault(d => d.Id == id && !d.IsDeleted);
            if (driver == null)
                return new ResponseMsg { IsSuccess = false, Message = "Driver not found!" };

            driver.IsDeleted = true;
            driver.DeletedAt = DateTime.UtcNow;
            db.SaveChanges();

            return new ResponseMsg { IsSuccess = true, Message = "Driver deleted!" };
        }

        protected ResponseMsg ExecuteRestoreDriverAction(int id)
        {
            using var db = new OrderContext();

            var driver = db.Drivers.FirstOrDefault(d => d.Id == id && d.IsDeleted);
            if (driver == null)
                return new ResponseMsg { IsSuccess = false, Message = "Deleted driver not found!" };

            driver.IsDeleted = false;
            driver.DeletedAt = null;
            db.SaveChanges();

            return new ResponseMsg { IsSuccess = true, Message = "Driver restored!" };
        }

        protected List<DriverDTO> ExecuteGetDeletedDriversAction()
        {
            using var db = new OrderContext();

            return db.Drivers
                .AsNoTracking()
                .Where(d => d.IsDeleted)
                .OrderByDescending(d => d.DeletedAt)
                .Select(d => new DriverDTO
                {
                    Id = d.Id,
                    FullName = d.FullName,
                    Phone = d.Phone,
                    CdlNumber = d.CdlNumber,
                    Status = d.Status,
                    VehicleId = d.VehicleId,
                    VehicleModel = d.Vehicle != null ? d.Vehicle.Model : null,
                    CreatedAt = d.CreatedAt
                })
                .ToList();
        }

        protected PagedResult<DriverDTO> ExecuteGetDriversPagedAction(QueryOptions opts, string? status)
        {
            using var db = new OrderContext();

            var query = db.Drivers
                .AsNoTracking()
                .Where(d => !d.IsDeleted
                         && (string.IsNullOrWhiteSpace(status) || d.Status == status));

            if (!string.IsNullOrWhiteSpace(opts.Search))
                query = query.Where(d => d.FullName.Contains(opts.Search) || d.Phone.Contains(opts.Search));

            var total = query.Count();

            var items = query
                .OrderBy(d => d.FullName)
                .Skip((opts.Page - 1) * opts.PageSize)
                .Take(opts.PageSize)
                .Select(d => new DriverDTO
                {
                    Id = d.Id,
                    FullName = d.FullName,
                    Phone = d.Phone,
                    CdlNumber = d.CdlNumber,
                    Status = d.Status,
                    VehicleId = d.VehicleId,
                    VehicleModel = d.Vehicle != null ? d.Vehicle.Model : null,
                    CreatedAt = d.CreatedAt
                })
                .ToList();

            return new PagedResult<DriverDTO>
            {
                Items = items,
                TotalCount = total,
                Page = opts.Page,
                PageSize = opts.PageSize
            };
        }
    }
}
