using back_end.Domain;

namespace back_end.DAL.Repositories
{
    public class LeadRepository : ILeadRepository
    {
        private readonly AppDbContext _db;
        public LeadRepository(AppDbContext db) => _db = db;

        public Lead Submit(Lead lead)
        {
            _db.Leads.Add(lead);
            _db.SaveChanges();
            return lead;
        }

        public List<Lead> GetAll(string? status)
        {
            var query = _db.Leads.AsQueryable();
            if (!string.IsNullOrEmpty(status)) query = query.Where(l => l.Status == status);
            return query.OrderByDescending(l => l.CreatedAt).ToList();
        }

        public Lead? GetById(int id) => _db.Leads.Find(id);

        public Lead? UpdateStatus(int id, string status)
        {
            var lead = _db.Leads.Find(id);
            if (lead == null) return null;
            lead.Status = status;
            _db.SaveChanges();
            return lead;
        }

        public bool Delete(int id)
        {
            var lead = _db.Leads.Find(id);
            if (lead == null) return false;
            _db.Leads.Remove(lead);
            _db.SaveChanges();
            return true;
        }
    }
}
