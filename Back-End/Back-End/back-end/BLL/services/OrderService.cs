using AutoMapper;
using back_end.BLL.DTOs;
using back_end.DAL.Repositories;
using back_end.Domain;
using Microsoft.Extensions.Logging;
namespace back_end.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;
        public OrderService(IOrderRepository repository, IMapper mapper, ILogger<OrderService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
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
            var created = _repository.Create(order);
            _logger.LogInformation("Order {Id} created for user {UserId}", created.Id, userId);
            return _mapper.Map<OrderDto>(created);
        }
        public OrderDto? UpdateStatus(int id, string status)
        {
            var updated = _repository.UpdateStatus(id, status);
            if (updated != null) _logger.LogInformation("Order {Id} status changed to {Status}", id, status);
            return updated == null ? null : _mapper.Map<OrderDto>(updated);
        }
        public bool Delete(int id)
        {
            var result = _repository.Delete(id);
            if (result) _logger.LogInformation("Order {Id} deleted", id);
            return result;
        }
        public object GetStats()
        {
            var orders = _repository.GetAll();
            return new
            {
                Total = orders.Count,
                Pending = orders.Count(o => o.Status == "Pending"),
                Approved = orders.Count(o => o.Status == "Approved"),
                Cancelled = orders.Count(o => o.Status == "Cancelled"),
                TotalRevenue = orders
                    .Where(o => o.Status == "Approved")
                    .Sum(o => o.Product?.Price ?? 0)
            };
        }
    }
}