using ClassifiedAds.Mobile.Models;
using ClassifiedAds.Mobile.RepoServices.UserAuthRepoService;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ClassifiedAds.Mobile.RepoServices.MemberRepoService
{
    public class MemberRepository : IMemberRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUserAuthService _authService;

        public MemberRepository(IHttpClientFactory httpClientFactory, IUserAuthService authService)
        {
            _httpClientFactory = httpClientFactory;
            _authService = authService;
        }

        private async Task<HttpClient> GetAuthenticatedClientAsync()
        {
            var client = _httpClientFactory.CreateClient("AdsApi");
            var token = await _authService.GetTokenAsync(); // You need to expose a method to get the raw token string
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }

        public async Task<MemberDto?> GetMemberProfileAsync(string memberId)
        {
            try
            {
                var client = await GetAuthenticatedClientAsync();
                // Calls GET api/members/{id}
                var response = await client.GetAsync($"api/members/{memberId}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<MemberDto>();
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching profile: {ex.Message}");
                return null;
            }
        }

        public async Task<UserDto?> GetCurrentMemberAsync()
        {
            try
            {
                var client = await GetAuthenticatedClientAsync();
                // Assumes you have an endpoint to get the current user details via token
                // If not, we might need to rely on the UserAuthService cached user or fetch via ID
                // For this example, let's assume getting the specific member by ID from the token is handled by the API
                var response = await client.GetAsync($"api/members/current");
                // Note: You might need to adjust this endpoint based on your specific API routes

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserDto>();
                }
                return null;
            }
            catch { return null; }
        }

        public async Task<bool> UpdateMemberAsync(MemberUpdateDto memberUpdate)
        {
            try
            {
                var client = await GetAuthenticatedClientAsync();
                var response = await client.PutAsJsonAsync("api/members", memberUpdate);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<PhotoDto?> UploadPhotoAsync(FileResult file)
        {
            try
            {
                var client = await GetAuthenticatedClientAsync();

                using var content = new MultipartFormDataContent();
                using var stream = await file.OpenReadAsync();
                using var fileContent = new StreamContent(stream);

                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
                content.Add(fileContent, "file", file.FileName);

                var response = await client.PostAsync("api/members/add-photo", content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<PhotoDto>();
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<bool> SetMainPhotoAsync(int photoId)
        {
            try
            {
                var client = await GetAuthenticatedClientAsync();
                var response = await client.PutAsync($"api/members/set-main-photo/{photoId}", null);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
    }
}