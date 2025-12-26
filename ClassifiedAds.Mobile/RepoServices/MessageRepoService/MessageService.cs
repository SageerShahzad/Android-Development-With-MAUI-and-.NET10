using ClassifiedAds.Mobile.Models;

namespace ClassifiedAds.Mobile.RepoServices.MessageRepoService
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<bool> SendInquiryAsync(string recipientId, string messageContent)
        {
            var dto = new CreateMessageDto
            {
                RecipientId = recipientId,
                Content = messageContent
            };

            return await _messageRepository.SendMessageAsync(dto);
        }

        public async Task<List<MessageDto>> GetMessageThreadAsync(string recipientId)
        {
            return await _messageRepository.GetMessageThreadAsync(recipientId);
        }
    }
}