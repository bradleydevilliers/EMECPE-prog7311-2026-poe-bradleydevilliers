namespace TechMoveGLMS.Shared.Services.Pricing
{
//(Refactoring Guru, 2026)

    public class EnterprisePricingStrategy : IPricingStrategy
    {
        private const decimal ENTERPRISE_DISCOUNT = 0.85m;
        private const decimal DISTANCE_RATE = 2.50m;
        
        public decimal CalculatePrice(decimal baseCost, string serviceLevel, decimal distance = 0, bool isPriority = false)
        {
            var price = baseCost * ENTERPRISE_DISCOUNT;
            if (distance > 0) price += distance * DISTANCE_RATE;
            return price;
        }
    }
}
// Refactoring Guru, 2026. [Strategy] Design Pattern.[Online] Available at:
//https://refactoring.guru/design-patterns/strategy
