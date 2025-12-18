using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ClassifiedAds.Common.Entities
{
    public class Ad
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Location relationships
        public int CountryId { get; set; }
        public Country Country { get; set; } = null!;
        public int CityId { get; set; }
        public City City { get; set; } = null!;
        public string PostalCode { get; set; } = string.Empty;

        public decimal? Price { get; set; }
        public bool IsAdultContent { get; set; }

        // Category relationships
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public int SubCategoryId { get; set; }
        public SubCategory SubCategory { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
        public bool IsFlagged { get; set; }

        // --- FIX STARTS HERE ---
        // 1. Change type to string to match Member.Id
        // 2. Rename to MemberId for clarity
        public string MemberId { get; set; } = null!;

        [JsonIgnore]
        // 3. Point the ForeignKey to the string property created above
        [ForeignKey(nameof(MemberId))]
        public Member Member { get; set; } = null!;
        // --- FIX ENDS HERE ---

        [JsonIgnore]
        public List<Photo> Photos { get; set; } = [];
    }
}