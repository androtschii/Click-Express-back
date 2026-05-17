using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Entities.Lead;
using ClickExpress.Domain.Models.Lead;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Core.Lead
{
    public class LeadActions
    {
        protected ResponseAction ExecuteSubmitLeadAction(CreateLeadDTO dto)
        {
            using (var db = new OrderContext())
            {
                var lead = new LeadData
                {
                    FullName = dto.FullName, Email = dto.Email, Phone = dto.Phone,
                    Company = dto.Company, Origin = dto.Origin, Destination = dto.Destination,
                    Equipment = dto.Equipment, Weight = dto.Weight, PickupDate = dto.PickupDate,
                    Message = dto.Message, Status = "New", CreatedAt = DateTime.Now
                };
                db.Leads.Add(lead);
                db.SaveChanges();
                return new ResponseAction { IsSuccess = true, Message = "Lead submitted!", Id = lead.Id };
            }
        }

        protected List<LeadDTO> ExecuteGetAllLeadsAction(string? status)
        {
            using (var db = new OrderContext())
            {
                var query = db.Leads.AsQueryable();
                if (!string.IsNullOrWhiteSpace(status)) query = query.Where(l => l.Status == status);

                return query.OrderByDescending(l => l.CreatedAt).Select(l => new LeadDTO
                {
                    Id = l.Id, FullName = l.FullName, Email = l.Email, Phone = l.Phone,
                    Company = l.Company, Origin = l.Origin, Destination = l.Destination,
                    Equipment = l.Equipment, Weight = l.Weight, PickupDate = l.PickupDate,
                    Message = l.Message, Status = l.Status, CreatedAt = l.CreatedAt
                }).ToList();
            }
        }

        protected LeadDTO? ExecuteGetLeadByIdAction(int id)
        {
            using (var db = new OrderContext())
            {
                var l = db.Leads.FirstOrDefault(l => l.Id == id);
                if (l == null) return null;
                return new LeadDTO
                {
                    Id = l.Id, FullName = l.FullName, Email = l.Email, Phone = l.Phone,
                    Company = l.Company, Origin = l.Origin, Destination = l.Destination,
                    Equipment = l.Equipment, Weight = l.Weight, PickupDate = l.PickupDate,
                    Message = l.Message, Status = l.Status, CreatedAt = l.CreatedAt
                };
            }
        }

        protected ResponseMsg ExecuteUpdateLeadStatusAction(int id, string status)
        {
            using (var db = new OrderContext())
            {
                var lead = db.Leads.FirstOrDefault(l => l.Id == id);
                if (lead == null)
                    return new ResponseMsg { IsSuccess = false, Message = "Lead not found!" };

                lead.Status = status;
                db.SaveChanges();
                return new ResponseMsg { IsSuccess = true, Message = "Status updated!" };
            }
        }

        protected ResponseMsg ExecuteDeleteLeadAction(int id)
        {
            using (var db = new OrderContext())
            {
                var lead = db.Leads.FirstOrDefault(l => l.Id == id);
                if (lead == null)
                    return new ResponseMsg { IsSuccess = false, Message = "Lead not found!" };

                db.Leads.Remove(lead);
                db.SaveChanges();
                return new ResponseMsg { IsSuccess = true, Message = "Lead deleted!" };
            }
        }
    }
}
