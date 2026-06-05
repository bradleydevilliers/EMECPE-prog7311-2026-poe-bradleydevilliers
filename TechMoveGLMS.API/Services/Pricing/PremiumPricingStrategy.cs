namespace TechMoveGLMS.Shared.Services.Pricing
{
//(Refactoring Guru, 2026)

    public class PremiumPricingStrategy : IPricingStrategy
    {
        private const decimal PREMIUM_MARKUP = 1.25m;
        
        public decimal CalculatePrice(decimal baseCost, string serviceLevel, decimal distance = 0, bool isPriority = false)
        {
            var price = baseCost * PREMIUM_MARKUP;
            if (isPriority) price += 50.00m;
            return price;
        }
    }
}
// Refactoring Guru, 2026. [Strategy] Design Pattern.[Online] Available at:
//https://refactoring.guru/design-patterns/strategy
