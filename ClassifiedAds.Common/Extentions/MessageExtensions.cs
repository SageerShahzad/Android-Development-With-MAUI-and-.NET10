using ClassifiedAds.Common.DTOs;
using ClassifiedAds.Common.Entities;
using System.Linq.Expressions;


namespace ClassifiedAds.Common.Extensions;

public static class MessageExtensions
{
    public static MessageDto ToDto(this Message message)
    {
        return new MessageDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            SenderDisplayName = message.Sender.DisplayName,
            SenderImageUrl = message.Sender.ImageUrl,
            RecipientId = message.RecipientId,
            RecipientDisplayName = message.Recipient.DisplayName,
            RecipientImageUrl = message.Recipient.ImageUrl,
            Content = message.Content,
            DateRead = message.DateRead,
            MessageSent = message.MessageSent
        };
    }

    public static Expression<Func<Message, MessageDto>> ToDtoProjection()
    {
        return message => new MessageDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            SenderDisplayName = message.Sender.DisplayName, // Assuming DisplayName exists on Member
            SenderImageUrl = message.Sender.Photos.FirstOrDefault(x => x.IsApproved).Url, // Example logic
            RecipientId = message.RecipientId,
            RecipientDisplayName = message.Recipient.DisplayName,
            RecipientImageUrl = message.Recipient.Photos.FirstOrDefault(x => x.IsApproved).Url,

            Content = message.Content,
            AttachmentUrl = message.AttachmentUrl, // <--- Add this line

            DateRead = message.DateRead,
            MessageSent = message.MessageSent
        };
    }
}
