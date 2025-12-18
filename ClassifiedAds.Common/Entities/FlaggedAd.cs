namespace ClassifiedAds.Common.Entities
{
    public class FlaggedAd
    {
        public int Id { get; set; }
        public int AdId { get; set; }
        public Ad Ad { get; set; }
        public string FlaggedByUserId { get; set; } = string.Empty;
        public Member FlaggedBy { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }

}
