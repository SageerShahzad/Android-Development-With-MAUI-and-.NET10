namespace ClassifiedAds.Common.Entities;

public class Message
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    // Change to nullable (string?) because a message might be just an image
    public string? Content { get; set; } 
    
    // New properties for the attachment
    public string? AttachmentUrl { get; set; }
    public string? AttachmentPublicId { get; set; } // Useful if you need to delete from Cloudinary later

    public DateTime? DateRead { get; set; }
    public DateTime MessageSent { get; set; } = DateTime.UtcNow;
    public bool SenderDeleted { get; set; }
    public bool RecipientDeleted { get; set; }

    public required string SenderId { get; set; }
    public Member Sender { get; set; } = null!;
    public required string RecipientId { get; set; }
    public Member Recipient { get; set; } = null!;
}