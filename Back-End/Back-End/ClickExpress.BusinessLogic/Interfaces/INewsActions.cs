using ClickExpress.Domain.Models.News;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Interfaces
{
    public interface INewsActions
    {
        List<NewsArticleDTO> GetAllNewsAction(bool onlyPublished);
        NewsArticleDTO? GetNewsByIdAction(int id);
        ResponseAction ResponseCreateNewsAction(int authorId, CreateNewsDTO dto);
        ResponseMsg ResponseUpdateNewsAction(int id, CreateNewsDTO dto);
        ResponseMsg ResponseDeleteNewsAction(int id);
    }
}
