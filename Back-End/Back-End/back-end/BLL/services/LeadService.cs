using AutoMapper;
using back_end.BLL.DTOs;
using back_end.DAL.Repositories;
using back_end.Domain;
using Microsoft.Extensions.Logging;

namespace back_end.BLL.Services
{
    public class LeadService : ILeadService
    {
        private readonly ILeadRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<LeadService> _logger;

        public LeadService(ILeadRepository repo, IMapper mapper, ILogger<LeadService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public LeadDto Submit(CreateLeadDto dto)
        {
            var lead = _mapper.Map<Lead>(dto);
            var created = _repo.Submit(lead);
            _logger.LogInformation("Lead {Id} submitted from {Email}: {Origin} -> {Destination}", created.Id, created.Email, created.Origin, created.Destination);
            return _mapper.Map<LeadDto>(created);
        }

        public List<LeadDto> GetAll(string? status)
            => _mapper.Map<List<LeadDto>>(_repo.GetAll(status));

        public LeadDto? GetById(int id)
        {
            var lead = _repo.GetById(id);
            return lead == null ? null : _mapper.Map<LeadDto>(lead);
        }

        public LeadDto? UpdateStatus(int id, string status)
        {
            var updated = _repo.UpdateStatus(id, status);
            if (updated == null) return null;
            _logger.LogInformation("Lead {Id} status changed to {Status}", id, status);
            return _mapper.Map<LeadDto>(updated);
        }

        public bool Delete(int id)
        {
            var result = _repo.Delete(id);
            if (result) _logger.LogInformation("Lead {Id} deleted", id);
            return result;
        }
    }
}
