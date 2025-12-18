namespace ClassifiedAds.Common.Dtos
{
    public class AdCreateDTO
    {
        // REMOVED MemberId. The client should never send this. 
        // The server determines the MemberId from the auth token.

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public int CityId { get; set; }
        public int CountryId { get; set; }
        public string PostalCode { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new();
    }
}