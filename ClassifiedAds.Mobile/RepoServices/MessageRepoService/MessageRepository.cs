using ClassifiedAds.Mobile.Models;
using ClassifiedAds.Mobile.RepoServices.UserAuthRepoService;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ClassifiedAds.Mobile.RepoServices.MessageRepoService
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUserAuthService _authService;

        public MessageRepository(IHttpClientFactory httpClientFactory, IUserAuthService authService)
        {
            _httpClientFactory = httpClientFactory;
            _authService = authService;
        }

        public async Task<bool> SendMessageAsync(CreateMessageDto messageDto)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AdsApi");
                var token = await _authService.GetTokenAsync();

                if (string.IsNullOrEmpty(token)) return false;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // The backend requires [FromForm], so we use MultipartFormDataContent
                using var content = new MultipartFormDataContent();

                content.Add(new StringContent(messageDto.RecipientId), "RecipientId");

                if (!string.IsNullOrEmpty(messageDto.Content))
                {
                    content.Add(new StringContent(messageDto.Content), "Content");
                }

                // API Endpoint: POST api/messages
                var response = await client.PostAsync("api/messages", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Send Message Error: {ex.Message}");
                return false;
            }
        }

        public async Task<List<MessageDto>> GetMessageThreadAsync(string recipientId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AdsApi");
                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token)) return new List<MessageDto>();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync($"api/messages/thread/{recipientId}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<MessageDto>>();
                }
                return new List<MessageDto>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Fetch Thread Error: {ex.Message}");
                return new List<MessageDto>();
            }
        }
    }
}