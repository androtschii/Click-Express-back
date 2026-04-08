using AutoMapper;
using back_end.BLL.DTOs;
using back_end.DAL.Repositories;
using back_end.Domain;

namespace back_end.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public List<UserDto> GetAll()
        {
            var users = _repository.GetAll();
            return _mapper.Map<List<UserDto>>(users);
        }

        public UserDto? GetById(int id)
        {
            var user = _repository.GetById(id);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public UserDto Create(CreateUserDto dto)
        {
            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var created = _repository.Create(user);
            return _mapper.Map<UserDto>(created);
        }

        public UserDto? Update(int id, UpdateUserDto dto)
        {
            var user = _mapper.Map<User>(dto);
            var updated = _repository.Update(id, user);
            return updated == null ? null : _mapper.Map<UserDto>(updated);
        }

        public bool Delete(int id) => _repository.Delete(id);
    }
}
