using System.Net.Http.Json;
using ClassifiedAds.Mobile.Models; // Ensure DTOs are here

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
            var client = _httpClientFactory.CreateClient("AdsApi");
            var result = await client.GetFromJsonAsync<List<CategoryDto>>("api/categories");
            return result?.Select(x => x.Name).ToList() ?? new List<string>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Category Fetch Error: {ex.Message}");
            return new List<string>();
        }
    }

    // UPDATED: Return full objects to get IDs
    public async Task<List<CountryDto>> GetCountriesAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("AdsApi");
            return await client.GetFromJsonAsync<List<CountryDto>>("api/countries") ?? new List<CountryDto>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Country Fetch Error: {ex.Message}");
            return new List<CountryDto>();
        }
    }

    // NEW: Fetch Cities by Country ID
    public async Task<List<string>> GetCitiesByCountryAsync(int countryId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("AdsApi");
            // Calls: api/cities/bycountry/{id}
            var result = await client.GetFromJsonAsync<List<CityDto>>($"api/cities/bycountry/{countryId}");
            return result?.Select(x => x.Name).ToList() ?? new List<string>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"City Fetch Error: {ex.Message}");
            return new List<string>();
        }
    }
}

// Simple DTOs (Add to Models folder if not present)
public class CategoryDto { public int Id { get; set; } public string Name { get; set; } }
public class CountryDto { public int Id { get; set; } public string Name { get; set; } }
public class CityDto { public int Id { get; set; } public string Name { get; set; } public int CountryId { get; set; } }