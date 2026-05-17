using ClickExpress.Domain.Models.JobApplication;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Interfaces
{
    public interface IJobApplicationActions
    {
        ResponseAction ResponseSubmitJobApplicationAction(CreateJobApplicationDTO dto);
        List<JobApplicationDTO> GetAllJobApplicationsAction(string? status);
        JobApplicationDTO? GetJobApplicationByIdAction(int id);
        ResponseMsg ResponseUpdateJobApplicationStatusAction(int id, string status);
        ResponseMsg ResponseDeleteJobApplicationAction(int id);
    }
}
