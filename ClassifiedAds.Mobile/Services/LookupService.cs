using System.Net.Http.Json;
using ClassifiedAds.Mobile.Models; // Ensure you have DTOs here

namespace ClassifiedAds.Mobile.Services;

public class LookupService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LookupService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("AdsApi"); // Ensure your HttpClient is named/configured
            // Assuming your Category entity has a 'Name' property
            // We fetch the list and project it to strings for the Picker
            var result = await client.GetFromJsonAsync<List<CategoryDto>>("api/categories");
            return result?.Select(x => x.Name).ToList() ?? new List<string>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Category Fetch Error: {ex.Message}");
            return new List<string>();
        }
    }

    public async Task<List<string>> GetCountriesAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("AdsApi");
            var result = await client.GetFromJsonAsync<List<CountryDto>>("api/countries");
            return result?.Select(x => x.Name).ToList() ?? new List<string>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Country Fetch Error: {ex.Message}");
            return new List<string>();
        }
    }
}

// Simple DTO classes if you don't share them with API
public class CategoryDto { public int Id { get; set; } public string Name { get; set; } }
public class CountryDto { public int Id { get; set; } public string Name { get; set; } }