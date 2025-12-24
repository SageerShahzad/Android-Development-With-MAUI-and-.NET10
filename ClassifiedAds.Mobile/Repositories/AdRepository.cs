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



    public async Task<AdDTO?> GetAd(int id)

    {

        // We will configure "AdsApi" in MauiProgram.cs later

        using HttpClient client = _httpClientFactory.CreateClient("AdsApi");



        try

        {

            // The route is "api/ads/{id}" based on your URL

            var response = await client.GetFromJsonAsync<AdDTO>($"api/ads/{id}");

            return response;

        }

        catch (Exception ex)

        {

            // Log error

            System.Diagnostics.Debug.WriteLine($"API ERROR: {ex.Message}");

            return null;

        }

    }

}