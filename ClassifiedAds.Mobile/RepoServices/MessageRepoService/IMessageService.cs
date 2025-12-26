using ClassifiedAds.Mobile.Models;

namespace ClassifiedAds.Mobile.RepoServices.MessageRepoService
{
    public interface IMessageService
    {
        Task<bool> SendInquiryAsync(string recipientId, string messageContent);
        Task<List<MessageDto>> GetMessageThreadAsync(string recipientId);
    }
}