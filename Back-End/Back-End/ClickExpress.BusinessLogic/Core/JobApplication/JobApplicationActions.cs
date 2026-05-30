using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Entities.JobApplication;
using ClickExpress.Domain.Models.JobApplication;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Core.JobApplication
{
    public class JobApplicationActions
    {
        protected ResponseAction ExecuteSubmitJobApplicationAction(CreateJobApplicationDTO dto)
        {
            using var db = new OrderContext();

            var app = new JobApplicationData
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                Position = dto.Position,
                Message = dto.Message,
                Status = "New",
                CreatedAt = DateTime.UtcNow
            };

            db.JobApplications.Add(app);
            db.SaveChanges();

            return new ResponseAction { IsSuccess = true, Message = "Application submitted!", Id = app.Id };
        }

        protected List<JobApplicationDTO> ExecuteGetAllJobApplicationsAction(string? status)
        {
            using var db = new OrderContext();

            return db.JobApplications
                .AsNoTracking()
                .Where(a => string.IsNullOrWhiteSpace(status) || a.Status == status)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new JobApplicationDTO
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    Email = a.Email,
                    Phone = a.Phone,
                    Position = a.Position,
                    Message = a.Message,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt
                })
                .ToList();
        }

        protected JobApplicationDTO? ExecuteGetJobApplicationByIdAction(int id)
        {
            using var db = new OrderContext();

            return db.JobApplications
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new JobApplicationDTO
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    Email = a.Email,
                    Phone = a.Phone,
                    Position = a.Position,
                    Message = a.Message,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt
                })
                .FirstOrDefault();
        }

        protected PagedResult<JobApplicationDTO> ExecuteGetJobApplicationsPagedAction(string? status, int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize is < 1 or > 100 ? 25 : pageSize;

            using var db = new OrderContext();

            var query = db.JobApplications
                .AsNoTracking()
                .Where(a => string.IsNullOrWhiteSpace(status) || a.Status == status);

            var total = query.Count();
            var items = query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new JobApplicationDTO
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    Email = a.Email,
                    Phone = a.Phone,
                    Position = a.Position,
                    Message = a.Message,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt
                })
                .ToList();

            return new PagedResult<JobApplicationDTO>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        protected ResponseMsg ExecuteUpdateJobApplicationStatusAction(int id, string status)
        {
            using var db = new OrderContext();

            var app = db.JobApplications.FirstOrDefault(a => a.Id == id);
            if (app == null)
                return new ResponseMsg { IsSuccess = false, Message = "Application not found!" };

            app.Status = status;
            db.SaveChanges();

            return new ResponseMsg { IsSuccess = true, Message = "Status updated!" };
        }

        protected ResponseMsg ExecuteDeleteJobApplicationAction(int id)
        {
            using var db = new OrderContext();

            var app = db.JobApplications.FirstOrDefault(a => a.Id == id);
            if (app == null)
                return new ResponseMsg { IsSuccess = false, Message = "Application not found!" };

            db.JobApplications.Remove(app);
            db.SaveChanges();

            return new ResponseMsg { IsSuccess = true, Message = "Application deleted!" };
        }
    }
}
