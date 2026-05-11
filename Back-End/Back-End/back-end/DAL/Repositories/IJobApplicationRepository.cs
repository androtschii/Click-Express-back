using back_end.Domain;

namespace back_end.DAL.Repositories
{
    public interface IJobApplicationRepository
    {
        JobApplication Submit(JobApplication application);
        List<JobApplication> GetAll(string? status);
        JobApplication? GetById(int id);
        JobApplication? UpdateStatus(int id, string status);
        bool Delete(int id);
    }
}
