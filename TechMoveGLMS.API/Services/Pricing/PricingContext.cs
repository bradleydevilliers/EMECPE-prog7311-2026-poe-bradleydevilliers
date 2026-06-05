namespace TechMoveGLMS.Shared.Services.Pricing
{
//(Refactoring Guru, 2026)

    public class PricingContext
    {
        private IPricingStrategy _strategy;
        private readonly ILogger<PricingContext> _logger;
        
        public PricingContext(ILogger<PricingContext> logger)
        {
            _logger = logger;
            _strategy = new StandardPricingStrategy();
        }
        
        public void SetStrategyByServiceLevel(string serviceLevel)
        {
            _strategy = serviceLevel.ToLower() switch
            {
                "premium" => new PremiumPricingStrategy(),
                "enterprise" => new EnterprisePricingStrategy(),
                _ => new StandardPricingStrategy()
            };
            _logger.LogInformation($"Pricing strategy set: {_strategy.GetType().Name}");
        }
        
        public decimal ExecuteStrategy(decimal baseCost, string serviceLevel, decimal distance = 0, bool isPriority = false)
        {
            return _strategy.CalculatePrice(baseCost, serviceLevel, distance, isPriority);
        }
    }
}
// Refactoring Guru, 2026. [Strategy] Design Pattern.[Online] Available at:
//https://refactoring.guru/design-patterns/strategy
