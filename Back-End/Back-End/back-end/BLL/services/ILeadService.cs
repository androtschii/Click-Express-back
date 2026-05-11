using back_end.BLL.DTOs;

namespace back_end.BLL.Services
{
    public interface ILeadService
    {
        LeadDto Submit(CreateLeadDto dto);
        List<LeadDto> GetAll(string? status);
        LeadDto? GetById(int id);
        LeadDto? UpdateStatus(int id, string status);
        bool Delete(int id);
    }
}
