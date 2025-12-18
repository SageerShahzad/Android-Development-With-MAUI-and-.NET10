namespace ClassifiedAds.Common.DTOs;

public class MessageDto
{
    public required string Id { get; set; }
    public required string SenderId { get; set; }
    public required string SenderDisplayName { get; set; }
    public string? SenderImageUrl { get; set; }
    public required string RecipientId { get; set; }
    public required string RecipientDisplayName { get; set; }
    public string? RecipientImageUrl { get; set; }

    public string? Content { get; set; } // Now nullable
    public string? AttachmentUrl { get; set; } // The new image URL

    public DateTime? DateRead { get; set; }
    public DateTime MessageSent { get; set; }
}

