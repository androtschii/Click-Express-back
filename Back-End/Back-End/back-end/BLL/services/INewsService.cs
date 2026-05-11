using back_end.BLL.DTOs;

namespace back_end.BLL.Services
{
    public interface INewsService
    {
        List<NewsArticleDto> GetAll(bool onlyPublished);
        NewsArticleDto? GetById(int id);
        NewsArticleDto Create(int authorId, CreateNewsDto dto);
        NewsArticleDto? Update(int id, CreateNewsDto dto);
        bool Delete(int id);
    }
}
