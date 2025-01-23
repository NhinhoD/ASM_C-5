namespace ASM_C_4.Areas.Admin.Repository
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message); // Ham gui email
    }
}
