namespace ClassifiedAds.Common.Entities
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;       // e.g. "London"
        public int CountryId { get; set; }
        public Country Country { get; set; }                   // link back to Country

    }
}
