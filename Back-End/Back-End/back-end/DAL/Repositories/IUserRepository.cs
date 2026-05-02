using back_end.Domain;

namespace back_end.DAL.Repositories
{
    public interface IUserRepository
    {
        List<User> GetAll();
        User? GetById(int id);
        User Create(User user);
        User? Update(int id, User user);
        void SaveProfile(User user);
        bool Delete(int id);
    }
}
