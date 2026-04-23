using AutoMapper;
using back_end.BLL.DTOs;
using back_end.DAL.Repositories;
using back_end.Domain;
using Microsoft.Extensions.Logging;
namespace back_end.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;
        public ProductService(IProductRepository repository, IMapper mapper, ILogger<ProductService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }
        public List<ProductDto> GetAll(string? search, string? category, int page = 1, int pageSize = 10)
        {
            var products = _repository.GetAll(search, category);
            return _mapper.Map<List<ProductDto>>(
                products.Skip((page - 1) * pageSize).Take(pageSize).ToList()
            );
        }
        public ProductDto? GetById(int id)
        {
            var p = _repository.GetById(id);
            return p == null ? null : _mapper.Map<ProductDto>(p);
        }
        public ProductDto Create(CreateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            var created = _repository.Create(product);
            _logger.LogInformation("Product {Id} created: {Name}", created.Id, created.Name);
            return _mapper.Map<ProductDto>(created);
        }
        public ProductDto? Update(int id, UpdateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            var updated = _repository.Update(id, product);
            return updated == null ? null : _mapper.Map<ProductDto>(updated);
        }
        public bool Delete(int id)
        {
            var result = _repository.Delete(id);
            if (result) _logger.LogInformation("Product {Id} deleted", id);
            return result;
        }
        public ProductDto? UpdatePrice(int id, decimal price)
        {
            var updated = _repository.UpdatePrice(id, price);
            if (updated != null) _logger.LogInformation("Product {Id} price updated to {Price}", id, price);
            return updated == null ? null : _mapper.Map<ProductDto>(updated);
        }
        public ProductDto? UpdateImage(int id, string imageUrl)
        {
            var updated = _repository.UpdateImage(id, imageUrl);
            return updated == null ? null : _mapper.Map<ProductDto>(updated);
        }
        public ProductDto? ToggleActive(int id)
        {
            var updated = _repository.ToggleActive(id);
            return updated == null ? null : _mapper.Map<ProductDto>(updated);
        }
        public ProductDto? UpdateStock(int id, int quantity)
        {
            var updated = _repository.UpdateStock(id, quantity);
            return updated == null ? null : _mapper.Map<ProductDto>(updated);
        }
        public object GetStats() => _repository.GetStats();
    }
}