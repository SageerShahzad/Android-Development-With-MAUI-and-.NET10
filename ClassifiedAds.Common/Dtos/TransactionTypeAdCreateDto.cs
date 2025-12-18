namespace ClassifiedAds.Common.Dtos
{
    public class TransactionTypeAdCreateDto
    {
        public int UserId { get; set; } 
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int TransactionTypeId { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public string PostalCode { get; set; }
    }
}

