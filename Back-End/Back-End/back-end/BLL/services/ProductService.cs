using AutoMapper;
using back_end.BLL.DTOs;
using back_end.DAL.Repositories;
using back_end.Domain;

namespace back_end.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public List<ProductDto> GetAll()
          => _mapper.Map<List<ProductDto>>(_repo.GetAll(null, null));

        public ProductDto? GetById(int id)
        {
            var product = _repo.GetById(id);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public ProductDto Create(ProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            var created = _repo.Create(product);
            return _mapper.Map<ProductDto>(created);
        }

        public ProductDto? Update(int id, ProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            var updated = _repo.Update(id, product);
            return updated == null ? null : _mapper.Map<ProductDto>(updated);
        }

        public bool Delete(int id)
            => _repo.Delete(id);
    }
}