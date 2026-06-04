namespace TechMoveGLMS.Shared.Services.Notifications
{
//(Refactoring Guru, 2026)

    // Concrete Observer - Compliance logging for audit trail
    public class ComplianceLogger : INotificationObserver
    {
        private readonly ILogger<ComplianceLogger> _logger;
        private readonly string _logPath;
        
        public ComplianceLogger(ILogger<ComplianceLogger> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _logPath = Path.Combine(env.ContentRootPath, "Logs", "compliance.log");
            
            var logDir = Path.GetDirectoryName(_logPath);
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
        }
        
        public async Task NotifyAsync(string subject, string message, string recipient)
        {
            var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Subject: {subject}, Recipient: {recipient}, Message: {message}";
            _logger.LogInformation($"[COMPLIANCE] {logEntry}");
            await File.AppendAllTextAsync(_logPath, logEntry + Environment.NewLine);
        }
    }
}
// Refactoring Guru, 2026. [Observer] Design Pattern.[Online] Available at:
//https://refactoring.guru/design-patterns/observer