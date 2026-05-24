namespace ClickExpress.BusinessLogic.Helpers
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string htmlBody);
        Task SendLeadConfirmationAsync(string toEmail, string fullName, string origin, string destination, string equipment);
        Task SendLeadAlertAsync(string fullName, string email, string phone, string origin, string destination, string equipment, string message);
        Task SendJobApplicationConfirmationAsync(string toEmail, string applicantName, string position);
        Task SendJobApplicationAlertAsync(string applicantName, string email, string phone, string position, string experience);
    }
}
