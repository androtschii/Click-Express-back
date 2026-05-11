using AutoMapper;
using back_end.BLL.DTOs;
using back_end.DAL.Repositories;
using back_end.Domain;
using Microsoft.Extensions.Logging;

namespace back_end.BLL.Services
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<NewsService> _logger;

        public NewsService(INewsRepository repo, IMapper mapper, ILogger<NewsService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public List<NewsArticleDto> GetAll(bool onlyPublished)
            => _mapper.Map<List<NewsArticleDto>>(_repo.GetAll(onlyPublished));

        public NewsArticleDto? GetById(int id)
        {
            var article = _repo.GetById(id);
            return article == null ? null : _mapper.Map<NewsArticleDto>(article);
        }

        public NewsArticleDto Create(int authorId, CreateNewsDto dto)
        {
            var article = new NewsArticle
            {
                Title = dto.Title,
                Content = dto.Content,
                ImageUrl = dto.ImageUrl,
                AuthorId = authorId,
                IsPublished = dto.IsPublished
            };
            var created = _repo.Create(article);
            _logger.LogInformation("News article {Id} '{Title}' created", created.Id, created.Title);
            return _mapper.Map<NewsArticleDto>(created);
        }

        public NewsArticleDto? Update(int id, CreateNewsDto dto)
        {
            var updated = _repo.Update(id, a =>
            {
                a.Title = dto.Title;
                a.Content = dto.Content;
                a.ImageUrl = dto.ImageUrl;
                a.IsPublished = dto.IsPublished;
            });
            if (updated == null) return null;
            _logger.LogInformation("News article {Id} updated", id);
            return _mapper.Map<NewsArticleDto>(updated);
        }

        public bool Delete(int id)
        {
            var result = _repo.Delete(id);
            if (result) _logger.LogInformation("News article {Id} deleted", id);
            return result;
        }
    }
}
