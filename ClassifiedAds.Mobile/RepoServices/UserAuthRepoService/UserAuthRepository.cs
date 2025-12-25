using ClassifiedAds.Mobile.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ClassifiedAds.Mobile.RepoServices.UserAuthRepoService
{
    public class UserAuthRepository : IUserAuthRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UserAuthRepository(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<UserDto?> LoginAsync(string email, string password)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AdsApi");

                var loginData = new LoginDto
                {
                    Email = email,
                    Password = password
                };

                // PostAsJsonAsync automatically serializes and handles headers
                var response = await client.PostAsJsonAsync("api/account/login", loginData);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserDto>();
                }

                // Optional: Log status code if failed
                System.Diagnostics.Debug.WriteLine($"Login failed: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login Exception: {ex.Message}");
                return null;
            }
        }

        // NEW IMPLEMENTATION
        public async Task LogoutAsync(string token)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AdsApi");

                // If the token exists, attach it to the header
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                // Call the endpoint
                await client.PostAsync("api/account/logout", null);
            }
            catch (Exception ex)
            {
                // We typically suppress errors here because we are logging out anyway.
                // But you can log it for debugging:
                System.Diagnostics.Debug.WriteLine($"Logout API failed: {ex.Message}");
            }
        }
    }
}