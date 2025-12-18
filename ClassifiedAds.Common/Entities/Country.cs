namespace ClassifiedAds.Common.Entities
{
    public class Country
    {
        public int Id { get; set; }
        public string IsoCode { get; set; } = string.Empty;    // e.g. "GB"
        public string Name { get; set; } = string.Empty;       // e.g. "United Kingdom"
        public ICollection<City> Cities { get; set; } = new List<City>();
    }
}
