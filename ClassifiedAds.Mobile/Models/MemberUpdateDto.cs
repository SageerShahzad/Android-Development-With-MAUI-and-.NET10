namespace ClassifiedAds.Mobile.Models
{
    public class MemberUpdateDto
    {
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }

    public class MemberDto
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Description { get; set; } // Bio
        public string City { get; set; }
        public string Country { get; set; }
        public string ImageUrl { get; set; }
        
    }
}