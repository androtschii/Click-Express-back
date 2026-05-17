namespace ClickExpress.Domain.Models.Base
{
    public class ResponseMsg
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
