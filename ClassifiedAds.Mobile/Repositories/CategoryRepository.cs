using ClassifiedAds.Mobile.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace ClassifiedAds.Mobile.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CategoryRepository(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<CategoryModel>> GetCategories()
    {
        using HttpClient client = _httpClientFactory.CreateClient("GloboTicketAdminApiClient");

        try
        {
            List<CategoryModel>? events = await client.GetFromJsonAsync<List<CategoryModel>>(
                $"categories",
                new JsonSerializerOptions(JsonSerializerDefaults.Web));

            return events ?? new List<CategoryModel>();
        }
        catch (Exception)
        {
            return new List<CategoryModel>();
        }
    }
}
