using back_end.BLL.DTOs;
namespace back_end.BLL.Services
{
    public interface IUserService
    {
        List<UserDto> GetAll();
        UserDto? GetById(int id);
        UserDto? GetByUsername(string username);
        UserDto Create(CreateUserDto dto);
        UserDto? Update(int id, UpdateUserDto dto);
        bool Delete(int id);
    }
}