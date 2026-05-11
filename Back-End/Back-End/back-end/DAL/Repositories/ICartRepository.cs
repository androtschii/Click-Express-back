using back_end.Domain;

namespace back_end.DAL.Repositories
{
    public interface ICartRepository
    {
        Cart GetOrCreate(int userId);
        CartItem? GetItem(int itemId);
        void Save();
    }
}
