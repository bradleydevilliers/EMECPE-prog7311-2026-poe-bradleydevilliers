namespace TechMoveGLMS.Shared.Services.Pricing
{
//(Refactoring Guru, 2026)

    public class StandardPricingStrategy : IPricingStrategy
    {
        public decimal CalculatePrice(decimal baseCost, string serviceLevel, decimal distance = 0, bool isPriority = false)
        {
            return baseCost;
        }
    }
}
// Refactoring Guru, 2026. [Strategy] Design Pattern.[Online] Available at:
//https://refactoring.guru/design-patterns/strategy
