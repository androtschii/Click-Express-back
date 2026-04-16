using AutoMapper;
using back_end.BLL.DTOs;
using back_end.DAL.Repositories;
using back_end.Domain;

namespace back_end.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public List<OrderDto> GetAll()
            => _mapper.Map<List<OrderDto>>(_repository.GetAll());

        public List<OrderDto> GetByUserId(int userId)
            => _mapper.Map<List<OrderDto>>(_repository.GetByUserId(userId));

        public OrderDto? GetById(int id)
        {
            var order = _repository.GetById(id);
            return order == null ? null : _mapper.Map<OrderDto>(order);
        }

        public OrderDto Create(int userId, CreateOrderDto dto)
        {
            var order = new Order
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Notes = dto.Notes,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };
            return _mapper.Map<OrderDto>(_repository.Create(order));
        }

        public OrderDto? UpdateStatus(int id, string status)
        {
            var updated = _repository.UpdateStatus(id, status);
            return updated == null ? null : _mapper.Map<OrderDto>(updated);
        }

        public bool Delete(int id) => _repository.Delete(id);
    }
}
