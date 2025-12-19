using ClassifiedAds.Mobile.Dtos;
using System.Text.Json;

namespace ClassifiedAds.Mobile.Services
{
    public class AdsService
    {
        private HttpClient _httpClient;

        // REPLACE with your actual API Port from launchSettings.json
        private const string Port = "5001";

        public AdsService()
        {
            _httpClient = new HttpClient();

            // Determine the base URL based on the device platform
            string baseUrl = DeviceInfo.Platform == DevicePlatform.Android
                ? $"http://10.0.2.2:{Port}"
                : $"http://localhost:{Port}";

            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public async Task<List<AdDTO>> GetAdsAsync()
        {
            try
            {
                // Calling the endpoint we created in Phase 1
                var response = await _httpClient.GetAsync("api/ads/openads");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    return JsonSerializer.Deserialize<List<AdDTO>>(content, options);
                }
            }
            catch (Exception ex)
            {
                // Log error (e.g., Debug.WriteLine(ex.Message))
                Console.WriteLine($"Error fetching ads: {ex.Message}");
            }

            return new List<AdDTO>(); // Return empty list on failure
        }
    }
}