namespace TechMoveGLMS.Shared.Services.Pricing
{
//(Refactoring Guru, 2026)

    // Strategy Pattern - Strategy interface
    public interface IPricingStrategy
    {
        decimal CalculatePrice(decimal baseCost, string serviceLevel, decimal distance = 0, bool isPriority = false);
    }
}
// Refactoring Guru, 2026. [Strategy] Design Pattern.[Online] Available at:
//https://refactoring.guru/design-patterns/strategy