using back_end.Domain;

namespace back_end.DAL.Repositories
{
    public interface INewsRepository
    {
        List<NewsArticle> GetAll(bool onlyPublished);
        NewsArticle? GetById(int id);
        NewsArticle Create(NewsArticle article);
        NewsArticle? Update(int id, Action<NewsArticle> apply);
        bool Delete(int id);
    }
}
