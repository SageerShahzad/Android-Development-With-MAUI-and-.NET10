namespace ClassifiedAds.Mobile.Dtos
{
    public class AdDTO
    {
        public int Id { get; set; }
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string SubCategory { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string? ImageUrl { get; set; }
        public string MainImageUrl { get; set; } = string.Empty;
        public string MemberId { get; set; } = null!;
        

    }
}
