using ClassifiedAds.Mobile.Models;

namespace ClassifiedAds.Mobile.RepoServices.MessageRepoService
{
    public interface IMessageRepository
    {
        Task<bool> SendMessageAsync(CreateMessageDto messageDto);
        Task<List<MessageDto>> GetMessageThreadAsync(string recipientId);
    }
}