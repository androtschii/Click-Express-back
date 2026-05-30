using Microsoft.EntityFrameworkCore;
using ClickExpress.DataAccess.Context;
using ClickExpress.Domain.Entities.News;
using ClickExpress.Domain.Models.News;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Core.News
{
    public class NewsActions
    {
        protected List<NewsArticleDTO> ExecuteGetAllNewsAction(bool onlyPublished)
        {
            using var db = new OrderContext();

            return db.NewsArticles
                .AsNoTracking()
                .Where(n => !onlyPublished || n.IsPublished)
                .OrderByDescending(n => n.PublishedAt)
                .Select(n => new NewsArticleDTO
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    ImageUrl = n.ImageUrl,
                    AuthorId = n.AuthorId,
                    AuthorName = n.Author.Username,
                    PublishedAt = n.PublishedAt,
                    IsPublished = n.IsPublished
                })
                .ToList();
        }

        protected NewsArticleDTO? ExecuteGetNewsByIdAction(int id)
        {
            using var db = new OrderContext();

            return db.NewsArticles
                .AsNoTracking()
                .Where(n => n.Id == id)
                .Select(n => new NewsArticleDTO
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    ImageUrl = n.ImageUrl,
                    AuthorId = n.AuthorId,
                    AuthorName = n.Author.Username,
                    PublishedAt = n.PublishedAt,
                    IsPublished = n.IsPublished
                })
                .FirstOrDefault();
        }

        protected ResponseAction ExecuteCreateNewsAction(int authorId, CreateNewsDTO dto)
        {
            using var db = new OrderContext();

            var article = new NewsArticleData
            {
                Title = dto.Title,
                Content = dto.Content,
                ImageUrl = dto.ImageUrl,
                AuthorId = authorId,
                IsPublished = dto.IsPublished,
                PublishedAt = DateTime.UtcNow
            };

            db.NewsArticles.Add(article);
            db.SaveChanges();

            return new ResponseAction { IsSuccess = true, Message = "Article created!", Id = article.Id };
        }

        protected ResponseMsg ExecuteUpdateNewsAction(int id, CreateNewsDTO dto)
        {
            using var db = new OrderContext();

            var article = db.NewsArticles.FirstOrDefault(n => n.Id == id);
            if (article == null)
                return new ResponseMsg { IsSuccess = false, Message = "Article not found!" };

            article.Title = dto.Title;
            article.Content = dto.Content;
            article.ImageUrl = dto.ImageUrl;
            article.IsPublished = dto.IsPublished;
            db.SaveChanges();

            return new ResponseMsg { IsSuccess = true, Message = "Article updated!" };
        }

        protected ResponseMsg ExecuteDeleteNewsAction(int id)
        {
            using var db = new OrderContext();

            var article = db.NewsArticles.FirstOrDefault(n => n.Id == id);
            if (article == null)
                return new ResponseMsg { IsSuccess = false, Message = "Article not found!" };

            db.NewsArticles.Remove(article);
            db.SaveChanges();

            return new ResponseMsg { IsSuccess = true, Message = "Article deleted!" };
        }
    }
}
