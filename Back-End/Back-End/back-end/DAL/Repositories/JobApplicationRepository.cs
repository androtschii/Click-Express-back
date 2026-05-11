using back_end.Domain;

namespace back_end.DAL.Repositories
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly AppDbContext _db;
        public JobApplicationRepository(AppDbContext db) => _db = db;

        public JobApplication Submit(JobApplication application)
        {
            _db.JobApplications.Add(application);
            _db.SaveChanges();
            return application;
        }

        public List<JobApplication> GetAll(string? status)
        {
            var query = _db.JobApplications.AsQueryable();
            if (!string.IsNullOrEmpty(status)) query = query.Where(a => a.Status == status);
            return query.OrderByDescending(a => a.CreatedAt).ToList();
        }

        public JobApplication? GetById(int id) => _db.JobApplications.Find(id);

        public JobApplication? UpdateStatus(int id, string status)
        {
            var application = _db.JobApplications.Find(id);
            if (application == null) return null;
            application.Status = status;
            _db.SaveChanges();
            return application;
        }

        public bool Delete(int id)
        {
            var application = _db.JobApplications.Find(id);
            if (application == null) return false;
            _db.JobApplications.Remove(application);
            _db.SaveChanges();
            return true;
        }
    }
}
