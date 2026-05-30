using ClickExpress.BusinessLogic.Core.Lead;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.Domain.Models.Lead;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Functions.Lead
{
    public class LeadFlow : LeadActions, ILeadActions
    {
        public ResponseAction ResponseSubmitLeadAction(CreateLeadDTO dto) => ExecuteSubmitLeadAction(dto);
        public List<LeadDTO> GetAllLeadsAction(string? status) => ExecuteGetAllLeadsAction(status);
        public LeadDTO? GetLeadByIdAction(int id) => ExecuteGetLeadByIdAction(id);
        public ResponseMsg ResponseUpdateLeadStatusAction(int id, string status) => ExecuteUpdateLeadStatusAction(id, status);
        public ResponseMsg ResponseDeleteLeadAction(int id) => ExecuteDeleteLeadAction(id);
        public PagedResult<LeadDTO> GetLeadsPagedAction(string? status, string? search, int page, int pageSize) => ExecuteGetLeadsPagedAction(status, search, page, pageSize);
    }
}
