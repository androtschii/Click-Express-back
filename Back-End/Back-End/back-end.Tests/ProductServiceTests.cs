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
    public class ProductServiceTests
    {
        private readonly IMapper _mapper;

        public ProductServiceTests()
        {
            var cfg = new MapperConfiguration(c => c.AddProfile<MappingProfile>());
            _mapper = cfg.CreateMapper();
        }

        [Fact]
        public void GetAll_ReturnsMappedDtos()
        {
            var repo = new Mock<IProductRepository>();
            repo.Setup(r => r.GetAll(null, null)).Returns(new List<Product>
            {
                new Product { Id = 1, Name = "Test1", Price = 100, Category = "A" },
                new Product { Id = 2, Name = "Test2", Price = 200, Category = "B" }
            });
            var service = new ProductService(repo.Object, _mapper, NullLogger<ProductService>.Instance);

            var result = service.GetAll(null, null);

            Assert.Equal(2, result.Count);
            Assert.Equal("Test1", result[0].Name);
        }

        [Fact]
        public void UpdatePrice_ReturnsNull_WhenProductMissing()
        {
            var repo = new Mock<IProductRepository>();
            repo.Setup(r => r.UpdatePrice(It.IsAny<int>(), It.IsAny<decimal>())).Returns((Product?)null);
            var service = new ProductService(repo.Object, _mapper, NullLogger<ProductService>.Instance);

            var res = service.UpdatePrice(5, 999);

            Assert.Null(res);
        }
    }
}
