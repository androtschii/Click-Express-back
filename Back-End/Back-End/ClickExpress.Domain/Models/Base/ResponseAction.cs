namespace ClickExpress.Domain.Models.Base
{
    public class ResponseAction
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Id { get; set; }
    }
}
