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
    public class UserServiceTests
    {
        private readonly IMapper _mapper;

        public UserServiceTests()
        {
            var cfg = new MapperConfiguration(c => c.AddProfile<MappingProfile>());
            _mapper = cfg.CreateMapper();
        }

        [Fact]
        public void GetById_ReturnsUser_WhenExists()
        {
            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.GetById(1)).Returns(new User { Id = 1, Username = "test", Email = "t@t.com", Role = "User" });
            var service = new UserService(repo.Object, _mapper, NullLogger<UserService>.Instance);

            var result = service.GetById(1);

            Assert.NotNull(result);
            Assert.Equal("test", result!.Username);
        }

        [Fact]
        public void GetById_ReturnsNull_WhenNotFound()
        {
            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.GetById(It.IsAny<int>())).Returns((User?)null);
            var service = new UserService(repo.Object, _mapper, NullLogger<UserService>.Instance);

            var result = service.GetById(999);

            Assert.Null(result);
        }

        [Fact]
        public void Delete_ReturnsTrue_WhenDeleted()
        {
            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.Delete(1)).Returns(true);
            var service = new UserService(repo.Object, _mapper, NullLogger<UserService>.Instance);

            var ok = service.Delete(1);

            Assert.True(ok);
        }
    }
}
