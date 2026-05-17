using ClickExpress.BusinessLogic.Core.User;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.Domain.Models.User;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Functions.User
{
    public class UserFlow : UserActions, IUserActions
    {
        public List<UserDTO> GetAllUsersAction() => ExecuteGetAllUsersAction();
        public UserDTO? GetUserByIdAction(int id) => ExecuteGetUserByIdAction(id);
        public UserDTO? GetUserByUsernameAction(string username) => ExecuteGetUserByUsernameAction(username);
        public ResponseAction ResponseUserCreateAction(UserRegDTO user) => ExecuteUserCreateAction(user);
        public ResponseMsg ResponseUserUpdateAction(int id, UpdateUserDTO user) => ExecuteUserUpdateAction(id, user);
        public ResponseMsg ResponseUserUpdateProfileAction(string username, UpdateProfileDTO dto) => ExecuteUserUpdateProfileAction(username, dto);
        public ResponseMsg ResponseUserActivateAction(UserActivateDTO user) => ExecuteUserActivateAction(user);
        public ResponseMsg ResponseUserDeleteAction(int id) => ExecuteUserDeleteAction(id);
        public ResponseAction ResponseValidateLoginAction(UserAuthDTO user) => ExecuteValidateLoginAction(user);
        public ResponseMsg ResponseSaveRefreshTokenAction(int userId, string token, DateTime expiry) => ExecuteSaveRefreshTokenAction(userId, token, expiry);
        public UserDTO? GetUserByRefreshTokenAction(string token) => ExecuteGetUserByRefreshTokenAction(token);
    }
}
