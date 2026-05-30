using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Entities.User;
using ClickExpress.Domain.Models.User;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Core.User
{
    public class UserActions
    {
        private string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
        private bool VerifyPassword(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);

        protected List<UserDTO> ExecuteGetAllUsersAction()
        {
            using (var db = new UserContext())
            {
                return db.Users.Where(u => u.IsActive).Select(u => new UserDTO
                {
                    Id = u.Id, Username = u.Username, Email = u.Email,
                    Role = u.Role, CreatedAt = u.CreatedAt, IsActive = u.IsActive,
                    FullName = u.FullName, Phone = u.Phone, Company = u.Company, Address = u.Address
                }).ToList();
            }
        }

        protected UserDTO? ExecuteGetUserByIdAction(int id)
        {
            using (var db = new UserContext())
            {
                var u = db.Users.FirstOrDefault(u => u.Id == id);
                if (u == null) return null;
                return new UserDTO
                {
                    Id = u.Id, Username = u.Username, Email = u.Email,
                    Role = u.Role, CreatedAt = u.CreatedAt, IsActive = u.IsActive,
                    FullName = u.FullName, Phone = u.Phone, Company = u.Company, Address = u.Address
                };
            }
        }

        protected UserDTO? ExecuteGetUserByUsernameAction(string username)
        {
            using (var db = new UserContext())
            {
                var u = db.Users.FirstOrDefault(u => u.Username == username);
                if (u == null) return null;
                return new UserDTO
                {
                    Id = u.Id, Username = u.Username, Email = u.Email,
                    Role = u.Role, CreatedAt = u.CreatedAt, IsActive = u.IsActive,
                    FullName = u.FullName, Phone = u.Phone, Company = u.Company, Address = u.Address
                };
            }
        }

        protected ResponseAction ExecuteUserCreateAction(UserRegDTO user)
        {
            if (string.IsNullOrWhiteSpace(user.Username))
                return new ResponseAction { IsSuccess = false, Message = "Username can't be empty!" };

            if (string.IsNullOrWhiteSpace(user.Email) || !user.Email.Contains("@"))
                return new ResponseAction { IsSuccess = false, Message = "Invalid Email format!" };

            if (string.IsNullOrWhiteSpace(user.Password) || user.Password.Length < 8)
                return new ResponseAction { IsSuccess = false, Message = "Password must be at least 8 characters!" };

            using (var db = new UserContext())
            {
                var email = user.Email.ToLower();
                var existing = db.Users.FirstOrDefault(u => u.Email.ToLower() == email || u.Username == user.Username);
                if (existing != null)
                    return new ResponseAction { IsSuccess = false, Message = "User with this email or username already exists!", Id = existing.Id };

                var newUser = new UserData
                {
                    Username = user.Username,
                    Email = user.Email,
                    PasswordHash = HashPassword(user.Password),
                    Role = user.Role ?? "User",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                db.Users.Add(newUser);
                db.SaveChanges();
                return new ResponseAction { IsSuccess = true, Message = "User created successfully!", Id = newUser.Id };
            }
        }

        protected ResponseMsg ExecuteUserUpdateAction(int id, UpdateUserDTO user)
        {
            using (var db = new UserContext())
            {
                var existing = db.Users.FirstOrDefault(u => u.Id == id);
                if (existing == null)
                    return new ResponseMsg { IsSuccess = false, Message = "User not found!" };

                if (string.IsNullOrWhiteSpace(user.Username))
                    return new ResponseMsg { IsSuccess = false, Message = "Username can't be empty!" };

                if (string.IsNullOrWhiteSpace(user.Email) || !user.Email.Contains("@"))
                    return new ResponseMsg { IsSuccess = false, Message = "Invalid Email format!" };

                var emailLower = user.Email.ToLower();
                var emailTaken = db.Users.FirstOrDefault(u => u.Email.ToLower() == emailLower && u.Id != id);
                if (emailTaken != null)
                    return new ResponseMsg { IsSuccess = false, Message = "Email is already in use!" };

                existing.Username = user.Username;
                existing.Email = user.Email;
                existing.Role = user.Role;
                existing.IsActive = user.IsActive;
                db.SaveChanges();
                return new ResponseMsg { IsSuccess = true, Message = "User updated successfully!" };
            }
        }

        protected ResponseMsg ExecuteUserUpdateProfileAction(string username, UpdateProfileDTO dto)
        {
            using (var db = new UserContext())
            {
                var existing = db.Users.FirstOrDefault(u => u.Username == username);
                if (existing == null)
                    return new ResponseMsg { IsSuccess = false, Message = "User not found!" };

                if (dto.FullName != null) existing.FullName = dto.FullName;
                if (dto.Phone != null)    existing.Phone    = dto.Phone;
                if (dto.Company != null)  existing.Company  = dto.Company;
                if (dto.Address != null)  existing.Address  = dto.Address;
                if (dto.Email != null)    existing.Email    = dto.Email;

                db.SaveChanges();
                return new ResponseMsg { IsSuccess = true, Message = "Profile updated successfully!" };
            }
        }

        protected ResponseMsg ExecuteUserActivateAction(UserActivateDTO user)
        {
            using (var db = new UserContext())
            {
                var existing = db.Users.FirstOrDefault(u => u.Id == user.Id);
                if (existing == null)
                    return new ResponseMsg { IsSuccess = false, Message = "User not found!" };

                existing.IsActive = user.IsActive;
                db.SaveChanges();
                return new ResponseMsg { IsSuccess = true, Message = "User activation status updated!" };
            }
        }

        protected ResponseMsg ExecuteUserDeleteAction(int id)
        {
            using (var db = new UserContext())
            {
                var existing = db.Users.FirstOrDefault(u => u.Id == id);
                if (existing == null)
                    return new ResponseMsg { IsSuccess = false, Message = "User not found!" };

                existing.IsActive = false;
                db.SaveChanges();
                return new ResponseMsg { IsSuccess = true, Message = "User deactivated!" };
            }
        }

        protected ResponseAction ExecuteValidateLoginAction(UserAuthDTO user)
        {
            if (string.IsNullOrWhiteSpace(user.Username))
                return new ResponseAction { IsSuccess = false, Message = "Username is required!" };

            if (string.IsNullOrWhiteSpace(user.Password))
                return new ResponseAction { IsSuccess = false, Message = "Password is required!" };

            using (var db = new UserContext())
            {
                var existing = db.Users.FirstOrDefault(u => u.Username == user.Username && u.IsActive);
                if (existing == null || !VerifyPassword(user.Password, existing.PasswordHash))
                    return new ResponseAction { IsSuccess = false, Message = "Invalid username or password!" };

                return new ResponseAction { IsSuccess = true, Message = "Login successful!", Id = existing.Id };
            }
        }

        protected ResponseMsg ExecuteSaveRefreshTokenAction(int userId, string token, DateTime expiry)
        {
            using (var db = new UserContext())
            {
                var existing = db.Users.FirstOrDefault(u => u.Id == userId);
                if (existing == null)
                    return new ResponseMsg { IsSuccess = false, Message = "User not found!" };

                existing.RefreshToken = token;
                existing.RefreshTokenExpiry = expiry;
                db.SaveChanges();
                return new ResponseMsg { IsSuccess = true, Message = "Token saved!" };
            }
        }

        protected UserDTO? ExecuteGetUserByRefreshTokenAction(string token)
        {
            using (var db = new UserContext())
            {
                var u = db.Users.FirstOrDefault(u => u.RefreshToken == token);
                if (u == null) return null;
                return new UserDTO
                {
                    Id = u.Id, Username = u.Username, Email = u.Email,
                    Role = u.Role, CreatedAt = u.CreatedAt, IsActive = u.IsActive,
                    FullName = u.FullName, Phone = u.Phone, Company = u.Company, Address = u.Address
                };
            }
        }
    }
}
