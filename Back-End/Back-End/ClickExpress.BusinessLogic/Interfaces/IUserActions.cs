using ClickExpress.Domain.Models.User;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Interfaces
{
    public interface IUserActions
    {
        List<UserDTO> GetAllUsersAction();
        UserDTO? GetUserByIdAction(int id);
        UserDTO? GetUserByUsernameAction(string username);
        ResponseAction ResponseUserCreateAction(UserRegDTO user);
        ResponseMsg ResponseUserUpdateAction(int id, UpdateUserDTO user);
        ResponseMsg ResponseUserUpdateProfileAction(string username, UpdateProfileDTO dto);
        ResponseMsg ResponseUserActivateAction(UserActivateDTO user);
        ResponseMsg ResponseUserDeleteAction(int id);
        ResponseAction ResponseValidateLoginAction(UserAuthDTO user);
        ResponseMsg ResponseSaveRefreshTokenAction(int userId, string token, DateTime expiry);
        UserDTO? GetUserByRefreshTokenAction(string token);
    }
}
