namespace TechMoveGLMS.Shared.Services.Notifications
{
    //(Refactoring Guru, 2026)
    
    // Observer Pattern - Observer interface
    public interface INotificationObserver
    {
        Task NotifyAsync(string subject, string message, string recipient);
    }
}
// Refactoring Guru, 2026. [Observer] Design Pattern.[Online] Available at:
//https://refactoring.guru/design-patterns/observer
