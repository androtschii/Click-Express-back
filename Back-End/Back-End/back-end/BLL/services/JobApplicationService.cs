using AutoMapper;
using back_end.BLL.DTOs;
using back_end.DAL.Repositories;
using back_end.Domain;
using Microsoft.Extensions.Logging;

namespace back_end.BLL.Services
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly IJobApplicationRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<JobApplicationService> _logger;

        public JobApplicationService(IJobApplicationRepository repo, IMapper mapper, ILogger<JobApplicationService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public JobApplicationDto Submit(CreateJobApplicationDto dto)
        {
            var application = _mapper.Map<JobApplication>(dto);
            var created = _repo.Submit(application);
            _logger.LogInformation("Job application {Id} submitted by {Email}", created.Id, created.Email);
            return _mapper.Map<JobApplicationDto>(created);
        }

        public List<JobApplicationDto> GetAll(string? status)
            => _mapper.Map<List<JobApplicationDto>>(_repo.GetAll(status));

        public JobApplicationDto? GetById(int id)
        {
            var app = _repo.GetById(id);
            return app == null ? null : _mapper.Map<JobApplicationDto>(app);
        }

        public JobApplicationDto? UpdateStatus(int id, string status)
        {
            var updated = _repo.UpdateStatus(id, status);
            if (updated == null) return null;
            _logger.LogInformation("Job application {Id} status changed to {Status}", id, status);
            return _mapper.Map<JobApplicationDto>(updated);
        }

        public bool Delete(int id)
        {
            var result = _repo.Delete(id);
            if (result) _logger.LogInformation("Job application {Id} deleted", id);
            return result;
        }
    }
}
