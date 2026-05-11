using Microsoft.EntityFrameworkCore;
using back_end.Domain;

namespace back_end.DAL.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly AppDbContext _db;
        public NewsRepository(AppDbContext db) => _db = db;

        public List<NewsArticle> GetAll(bool onlyPublished)
        {
            var query = _db.NewsArticles.Include(n => n.Author).AsQueryable();
            if (onlyPublished) query = query.Where(n => n.IsPublished);
            return query.OrderByDescending(n => n.PublishedAt).ToList();
        }

        public NewsArticle? GetById(int id)
            => _db.NewsArticles.Include(n => n.Author).FirstOrDefault(n => n.Id == id);

        public NewsArticle Create(NewsArticle article)
        {
            _db.NewsArticles.Add(article);
            _db.SaveChanges();
            return _db.NewsArticles.Include(n => n.Author).First(n => n.Id == article.Id);
        }

        public NewsArticle? Update(int id, Action<NewsArticle> apply)
        {
            var article = _db.NewsArticles.Find(id);
            if (article == null) return null;
            apply(article);
            _db.SaveChanges();
            return _db.NewsArticles.Include(n => n.Author).First(n => n.Id == id);
        }

        public bool Delete(int id)
        {
            var article = _db.NewsArticles.Find(id);
            if (article == null) return false;
            _db.NewsArticles.Remove(article);
            _db.SaveChanges();
            return true;
        }
    }
}
