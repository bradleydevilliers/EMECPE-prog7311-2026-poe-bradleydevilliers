using TechMoveGLMS.Shared.Models.Entities;
// (Refactoring Guru, 2026)
namespace TechMoveGLMS.Shared.Services.Contracts
{
    // Factory Method Pattern - Creator interface
    public interface IContractFactory
    {
        Contract CreateContract(string serviceLevel, int clientId, DateTime startDate, DateTime endDate);
    }
}
// Refactoring Guru, 2026. [Factory] Design Pattern.[Online] Available at:
// https://refactoring.guru/design-patterns/factory-method