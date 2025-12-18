namespace ClassifiedAds.Common.Entities
{
    public static class PriceFactory
    {
        public static Price Create(Category category, decimal amount, string currency)
        {
            return category.Name switch
            {
                "Services" => new Price(amount, currency, PricingUnit.PerHour),
                "Rentals" => new Price(amount, currency, PricingUnit.Monthly),
                "Cars" => new Price(amount, currency, PricingUnit.Fixed),
                _ => new Price(amount, currency, PricingUnit.Fixed)
            };
        }
    }
}
