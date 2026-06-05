namespace TechMoveGLMS.Shared.Services.Notifications
{
    // Concrete Observer - Email notifications
    public class EmailNotifier : INotificationObserver
    {
        private readonly ILogger<EmailNotifier> _logger;
        
        public EmailNotifier(ILogger<EmailNotifier> logger)
        {
            _logger = logger;
        }
        
        public async Task NotifyAsync(string subject, string message, string recipient)
        {
            _logger.LogInformation($"[EMAIL] To: {recipient}, Subject: {subject}, Message: {message}");
            await Task.CompletedTask;
        }
    }
}
// Refactoring Guru, 2026. [Observer] Design Pattern.[Online] Available at:
//https://refactoring.guru/design-patterns/observer
