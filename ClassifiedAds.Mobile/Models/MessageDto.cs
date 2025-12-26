namespace ClassifiedAds.Mobile.Models
{
    public class MessageDto
    {
        public string Id { get; set; }
        public string SenderId { get; set; }
        public string SenderDisplayName { get; set; }
        public string RecipientId { get; set; }
        public string Content { get; set; }
        public DateTime MessageSent { get; set; }
        public DateTime? DateRead { get; set; } // Ensure this exists!
        public string SenderImageUrl { get; set; }
    }
}