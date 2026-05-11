using back_end.Domain;

namespace back_end.DAL.Repositories
{
    public interface ILeadRepository
    {
        Lead Submit(Lead lead);
        List<Lead> GetAll(string? status);
        Lead? GetById(int id);
        Lead? UpdateStatus(int id, string status);
        bool Delete(int id);
    }
}
