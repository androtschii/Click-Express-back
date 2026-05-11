using back_end.BLL.DTOs;

namespace back_end.BLL.Services
{
    public interface IJobApplicationService
    {
        JobApplicationDto Submit(CreateJobApplicationDto dto);
        List<JobApplicationDto> GetAll(string? status);
        JobApplicationDto? GetById(int id);
        JobApplicationDto? UpdateStatus(int id, string status);
        bool Delete(int id);
    }
}
