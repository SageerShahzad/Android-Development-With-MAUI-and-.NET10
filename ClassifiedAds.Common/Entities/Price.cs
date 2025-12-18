namespace ClassifiedAds.Common.Entities
{
    public class Price
    {
        public decimal Amount { get; }
        public string Currency { get; } // ISO 4217 (e.g., "GBP")
        public PricingUnit Unit { get; } // Time/quantity unit

        public Price(decimal amount, string currency, PricingUnit unit)
        {
            Amount = amount;
            Currency = currency;
            Unit = unit;
        }

        public override string ToString() => Unit switch
        {
            PricingUnit.PerHour => $"{Currency}{Amount}/hour",
            PricingUnit.PerDay => $"{Currency}{Amount}/day",
            PricingUnit.Fixed => $"{Currency}{Amount}",
            _ => $"{Currency}{Amount} ({Unit})"
        };
    }

    public enum PricingUnit
    {
        Fixed,
        PerHour,
        PerDay,
        Weekly,
        Monthly,
        PerSession,
        PerSquareMeter
    }

}
