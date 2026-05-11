using AutoMapper;
using back_end.BLL.DTOs;
using back_end.DAL.Repositories;
using back_end.Domain;

namespace back_end.BLL.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _repo;
        private readonly IMapper _mapper;

        public VehicleService(IVehicleRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public List<VehicleDto> GetAll(string? type, bool? available)
            => _mapper.Map<List<VehicleDto>>(_repo.GetAll(type, available));

        public VehicleDto? GetById(int id)
        {
            var vehicle = _repo.GetById(id);
            return vehicle == null ? null : _mapper.Map<VehicleDto>(vehicle);
        }

        public VehicleDto Create(CreateVehicleDto dto)
        {
            var vehicle = _mapper.Map<Vehicle>(dto);
            return _mapper.Map<VehicleDto>(_repo.Create(vehicle));
        }

        public VehicleDto? Update(int id, UpdateVehicleDto dto)
        {
            var updated = _repo.Update(id, v => _mapper.Map(dto, v));
            return updated == null ? null : _mapper.Map<VehicleDto>(updated);
        }

        public VehicleDto? ToggleAvailability(int id)
        {
            var updated = _repo.ToggleAvailability(id);
            return updated == null ? null : _mapper.Map<VehicleDto>(updated);
        }

        public bool Delete(int id) => _repo.Delete(id);
    }
}
