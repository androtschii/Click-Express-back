using back_end.Domain;
using Microsoft.EntityFrameworkCore;

namespace back_end.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<User> GetAll() => _context.Users.ToList();

        public User? GetById(int id) => _context.Users.FirstOrDefault(u => u.Id == id);

        public User Create(User user)
        {
            user.CreatedAt = DateTime.Now;
            user.IsActive = true;
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public User? Update(int id, User updated)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return null;
            user.Username = updated.Username;
            user.Email = updated.Email;
            user.IsActive = updated.IsActive;
            _context.SaveChanges();
            return user;
        }

        public bool Delete(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return false;
            _context.Users.Remove(user);
            _context.SaveChanges();
            return true;
        }
    }
}
