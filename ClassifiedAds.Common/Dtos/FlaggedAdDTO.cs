namespace ClassifiedAds.Common.Dtos
{
    public class FlaggedAdDTO
    {
        public int Id { get; set; }
        public int AdId { get; set; }
        public string FlaggedByUserId { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
