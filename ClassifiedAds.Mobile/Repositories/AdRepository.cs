using ClassifiedAds.Mobile.Models;
using System.Net.Http.Json;

namespace ClassifiedAds.Mobile.Repositories;

public class AdRepository : IAdRepository
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AdRepository(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<AdDTO>> GetAds()
    {
        using HttpClient client = _httpClientFactory.CreateClient("AdsApi");
        try
        {
            // Matches your requirement: http://localhost:5000/api/ads/OpenAds/
            var response = await client.GetFromJsonAsync<List<AdDTO>>("api/ads/OpenAds/");
            return response ?? new List<AdDTO>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error fetching ads: {ex.Message}");
            return new List<AdDTO>();
        }
    }

    public async Task<AdDTO?> GetAd(int id)
    {
        using HttpClient client = _httpClientFactory.CreateClient("AdsApi");
        try
        {
            var response = await client.GetFromJsonAsync<AdDTO>($"api/ads/{id}");
            return response;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error fetching ad detail: {ex.Message}");
            return null;
        }
    }
}