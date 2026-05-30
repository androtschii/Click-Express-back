using ClickExpress.Domain.Models.Lead;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Interfaces
{
    public interface ILeadActions
    {
        ResponseAction ResponseSubmitLeadAction(CreateLeadDTO dto);
        List<LeadDTO> GetAllLeadsAction(string? status);
        LeadDTO? GetLeadByIdAction(int id);
        ResponseMsg ResponseUpdateLeadStatusAction(int id, string status);
        ResponseMsg ResponseDeleteLeadAction(int id);
        PagedResult<LeadDTO> GetLeadsPagedAction(string? status, string? search, int page, int pageSize);
    }
}
