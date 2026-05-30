using ClickExpress.BusinessLogic.Core.JobApplication;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.Domain.Models.JobApplication;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Functions.JobApplication
{
    public class JobApplicationFlow : JobApplicationActions, IJobApplicationActions
    {
        public ResponseAction ResponseSubmitJobApplicationAction(CreateJobApplicationDTO dto) => ExecuteSubmitJobApplicationAction(dto);
        public List<JobApplicationDTO> GetAllJobApplicationsAction(string? status) => ExecuteGetAllJobApplicationsAction(status);
        public JobApplicationDTO? GetJobApplicationByIdAction(int id) => ExecuteGetJobApplicationByIdAction(id);
        public ResponseMsg ResponseUpdateJobApplicationStatusAction(int id, string status) => ExecuteUpdateJobApplicationStatusAction(id, status);
        public ResponseMsg ResponseDeleteJobApplicationAction(int id) => ExecuteDeleteJobApplicationAction(id);
        public PagedResult<JobApplicationDTO> GetJobApplicationsPagedAction(string? status, int page, int pageSize) => ExecuteGetJobApplicationsPagedAction(status, page, pageSize);
    }
}
