using AutoMapper;
using back_end.BLL.DTOs;
using back_end.BLL.Mapping;
using back_end.BLL.Services;
using back_end.DAL.Repositories;
using back_end.Domain;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace back_end.Tests
{
    public class OrderServiceTests
    {
        private readonly IMapper _mapper;

        public OrderServiceTests()
        {
            var cfg = new MapperConfiguration(c => c.AddProfile<MappingProfile>());
            _mapper = cfg.CreateMapper();
        }

        [Fact]
        public void UpdateStatus_ReturnsNull_WhenOrderNotFound()
        {
            var repo = new Mock<IOrderRepository>();
            repo.Setup(r => r.UpdateStatus(It.IsAny<int>(), It.IsAny<string>())).Returns((Order?)null);
            var service = new OrderService(repo.Object, _mapper, NullLogger<OrderService>.Instance);

            var res = service.UpdateStatus(99, "Approved");

            Assert.Null(res);
        }

        [Fact]
        public void Create_SetsStatusToPending()
        {
            var repo = new Mock<IOrderRepository>();
            repo.Setup(r => r.Create(It.IsAny<Order>())).Returns<Order>(o => { o.Id = 1; return o; });
            var service = new OrderService(repo.Object, _mapper, NullLogger<OrderService>.Instance);

            var res = service.Create(1, new CreateOrderDto { ProductId = 5 });

            Assert.Equal("Pending", res.Status);
        }
    }
}
