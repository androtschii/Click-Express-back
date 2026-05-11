using AutoMapper;
using back_end.BLL.DTOs;
using back_end.DAL.Repositories;
using back_end.Domain;
using Microsoft.Extensions.Logging;

namespace back_end.BLL.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(IReviewRepository repo, IMapper mapper, ILogger<ReviewService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public List<ReviewDto> GetAll(bool onlyApproved)
            => _mapper.Map<List<ReviewDto>>(_repo.GetAll(onlyApproved));

        public ReviewDto? GetById(int id)
        {
            var review = _repo.GetById(id);
            return review == null ? null : _mapper.Map<ReviewDto>(review);
        }

        public ReviewDto Create(int userId, CreateReviewDto dto)
        {
            var review = new Review
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Rating = dto.Rating,
                Text = dto.Text,
                IsApproved = false
            };
            var created = _repo.Create(review);
            _logger.LogInformation("Review {Id} created by user {UserId}", created.Id, userId);
            return _mapper.Map<ReviewDto>(created);
        }

        public ReviewDto? Approve(int id)
        {
            var approved = _repo.Approve(id);
            if (approved == null) return null;
            _logger.LogInformation("Review {Id} approved", id);
            return _mapper.Map<ReviewDto>(approved);
        }

        public bool Delete(int id)
        {
            var result = _repo.Delete(id);
            if (result) _logger.LogInformation("Review {Id} deleted", id);
            return result;
        }
    }
}
