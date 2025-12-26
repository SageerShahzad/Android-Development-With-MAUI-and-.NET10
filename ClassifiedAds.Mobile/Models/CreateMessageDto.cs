namespace ClassifiedAds.Mobile.Models
{
    public class CreateMessageDto
    {
        public required string RecipientId { get; set; }
        public string? Content { get; set; }
        // We will stick to text-only inquiries for now to keep the UI simple
    }
}