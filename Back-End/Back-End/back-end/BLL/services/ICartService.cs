using back_end.BLL.DTOs;

namespace back_end.BLL.Services
{
    public interface ICartService
    {
        CartDto GetOrCreate(int userId);
        CartDto AddItem(int userId, int productId, int quantity);
        void RemoveItem(int itemId);
        void Clear(int userId);
    }
}
