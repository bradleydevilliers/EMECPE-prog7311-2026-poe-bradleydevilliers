using TechMoveGLMS.Models.Entities;

namespace TechMoveGLMS.Shared.Services.Contracts
{
   // (Refactoring Guru, 2026)
   
    // Concrete Factory implementation
    public class ContractFactory : IContractFactory
    {
        public Contract CreateContract(string serviceLevel, int clientId, DateTime startDate, DateTime endDate)
        {
            // Factory Method Pattern - Creates different contract configurations based on service level
            var contract = new Contract
            {
                ClientId = clientId,
                StartDate = startDate,
                EndDate = endDate,
                ServiceLevel = serviceLevel,
                Status = "Active"
            };
            
            // Apply service level specific defaults
            switch (serviceLevel.ToLower())
            {
                case "premium":
                    contract.EndDate = endDate.AddMonths(1);
                    break;
                case "enterprise":
                    contract.EndDate = endDate.AddMonths(3);
                    break;
            }
            
            return contract;
        }
    }
}
// Refactoring Guru, 2026. [Factory] Design Pattern.[Online] Available at:
// https://refactoring.guru/design-patterns/factory-method