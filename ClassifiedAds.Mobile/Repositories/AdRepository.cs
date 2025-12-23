using ClassifiedAds.Mobile.Models;
using GloboTicket.Admin.Mobile.Repositories;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ClassifiedAds.Mobile.Repositories;

public class AdRepository : IAdRepository
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AdRepository(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<EventModel?> GetEvent(Guid id)
    {
        using HttpClient client = _httpClientFactory.CreateClient("GloboTicketAdminApiClient");

        try
        {
            EventModel? @event = await client.GetFromJsonAsync<EventModel>(
                $"events/{id}",
                new JsonSerializerOptions(JsonSerializerDefaults.Web));

            return @event;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<List<EventModel>> GetEvents()
    {
        using HttpClient client = _httpClientFactory.CreateClient("GloboTicketAdminApiClient");

        try
        {
            List<EventModel>? events = await client.GetFromJsonAsync<List<EventModel>>(
                $"events",
                new JsonSerializerOptions(JsonSerializerDefaults.Web));

            return events ?? new List<EventModel>();
        }
        catch (Exception)
        {
            return new List<EventModel>();
        }
    }

    public async Task<bool> UpdateStatus(Guid id, EventStatusModel status)
    {
        using HttpClient client = _httpClientFactory.CreateClient("GloboTicketAdminApiClient");

        try
        {
            var content = new StringContent(JsonSerializer.Serialize(status), Encoding.UTF8, "application/json");
            var response = await client.PatchAsync($"events/{id}/status", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }

        return false;
    }

    public async Task<bool> CreateEvent(EventModel model)
    {
        using HttpClient client = _httpClientFactory.CreateClient("GloboTicketAdminApiClient");

        try
        {
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"events", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }

        return false;
    }

    public async Task<bool> EditEvent(EventModel model)
    {
        using HttpClient client = _httpClientFactory.CreateClient("GloboTicketAdminApiClient");

        try
        {
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"events/{model.Id}", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }

        return false;
    }

    public async Task<bool> DeleteEvent(Guid id)
    {
        using HttpClient client = _httpClientFactory.CreateClient("GloboTicketAdminApiClient");

        try
        {
            var response = await client.DeleteAsync($"events/{id}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }

        return false;
    }
}
