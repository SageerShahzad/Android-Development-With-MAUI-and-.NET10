namespace ClassifiedAds.Common.Entities
{
    public class TransactionType
    {
        public int Id { get; set; }
        public string Name { get; set; }  // e.g. "Buy", "Sell", etc.
        public ICollection<Ad> Ads { get; set; }
    }
}
