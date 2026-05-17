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
            using (var db = new OrderContext())
            {
                var app = new JobApplicationData
                {
                    FullName = dto.FullName, Email = dto.Email, Phone = dto.Phone,
                    Position = dto.Position, Message = dto.Message,
                    Status = "New", CreatedAt = DateTime.Now
                };
                db.JobApplications.Add(app);
                db.SaveChanges();
                return new ResponseAction { IsSuccess = true, Message = "Application submitted!", Id = app.Id };
            }
        }

        protected List<JobApplicationDTO> ExecuteGetAllJobApplicationsAction(string? status)
        {
            using (var db = new OrderContext())
            {
                var query = db.JobApplications.AsQueryable();
                if (!string.IsNullOrWhiteSpace(status)) query = query.Where(a => a.Status == status);

                return query.OrderByDescending(a => a.CreatedAt).Select(a => new JobApplicationDTO
                {
                    Id = a.Id, FullName = a.FullName, Email = a.Email, Phone = a.Phone,
                    Position = a.Position, Message = a.Message, Status = a.Status, CreatedAt = a.CreatedAt
                }).ToList();
            }
        }

        protected JobApplicationDTO? ExecuteGetJobApplicationByIdAction(int id)
        {
            using (var db = new OrderContext())
            {
                var a = db.JobApplications.FirstOrDefault(a => a.Id == id);
                if (a == null) return null;
                return new JobApplicationDTO
                {
                    Id = a.Id, FullName = a.FullName, Email = a.Email, Phone = a.Phone,
                    Position = a.Position, Message = a.Message, Status = a.Status, CreatedAt = a.CreatedAt
                };
            }
        }

        protected ResponseMsg ExecuteUpdateJobApplicationStatusAction(int id, string status)
        {
            using (var db = new OrderContext())
            {
                var app = db.JobApplications.FirstOrDefault(a => a.Id == id);
                if (app == null)
                    return new ResponseMsg { IsSuccess = false, Message = "Application not found!" };

                app.Status = status;
                db.SaveChanges();
                return new ResponseMsg { IsSuccess = true, Message = "Status updated!" };
            }
        }

        protected ResponseMsg ExecuteDeleteJobApplicationAction(int id)
        {
            using (var db = new OrderContext())
            {
                var app = db.JobApplications.FirstOrDefault(a => a.Id == id);
                if (app == null)
                    return new ResponseMsg { IsSuccess = false, Message = "Application not found!" };

                db.JobApplications.Remove(app);
                db.SaveChanges();
                return new ResponseMsg { IsSuccess = true, Message = "Application deleted!" };
            }
        }
    }
}
