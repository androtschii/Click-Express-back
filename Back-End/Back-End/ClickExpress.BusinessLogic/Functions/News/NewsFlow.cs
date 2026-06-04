using ClickExpress.BusinessLogic.Core.News;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.Domain.Models.News;
using ClickExpress.Domain.Models.Base;

namespace ClickExpress.BusinessLogic.Functions.News
{
    public class NewsFlow : NewsActions, INewsActions
    {
        public List<NewsArticleDTO> GetAllNewsAction(bool onlyPublished) => ExecuteGetAllNewsAction(onlyPublished);
        public PagedResult<NewsArticleDTO> GetNewsPagedAction(QueryOptions opts, bool onlyPublished) => ExecuteGetNewsPagedAction(opts, onlyPublished);
        public NewsArticleDTO? GetNewsByIdAction(int id) => ExecuteGetNewsByIdAction(id);
        public ResponseAction ResponseCreateNewsAction(int authorId, CreateNewsDTO dto) => ExecuteCreateNewsAction(authorId, dto);
        public ResponseMsg ResponseUpdateNewsAction(int id, CreateNewsDTO dto) => ExecuteUpdateNewsAction(id, dto);
        public ResponseMsg ResponseDeleteNewsAction(int id) => ExecuteDeleteNewsAction(id);
    }
}
