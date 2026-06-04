namespace TechMoveGLMS.Shared.Services.Notifications
{
    //(Refactoring Guru, 2026)
    
    // Observer Pattern - Subject that manages observers
    public class NotificationService
    {
        private readonly List<INotificationObserver> _observers = new();
        private readonly ILogger<NotificationService> _logger;
        
        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }
        
        public void Attach(INotificationObserver observer)
        {
            _observers.Add(observer);
            _logger.LogInformation($"Observer attached: {observer.GetType().Name}");
        }
        
        public async Task NotifyAllAsync(string subject, string message, string recipient = "admin@techmove.com")
        {
            foreach (var observer in _observers)
            {
                await observer.NotifyAsync(subject, message, recipient);
            }
        }
        
        public async Task NotifyContractExpiredAsync(string contractInfo)
        {
            await NotifyAllAsync("Contract Expired Alert", 
                $"Contract {contractInfo} has expired and requires attention.", 
                "compliance@techmove.com");
        }
        
        public async Task NotifyNewServiceRequestAsync(string requestDetails)
        {
            await NotifyAllAsync("New Service Request Created",
                $"Service Request: {requestDetails}",
                "operations@techmove.com");
        }
    }
}
// Refactoring Guru, 2026. [Observer] Design Pattern.[Online] Available at:
//https://refactoring.guru/design-patterns/observer