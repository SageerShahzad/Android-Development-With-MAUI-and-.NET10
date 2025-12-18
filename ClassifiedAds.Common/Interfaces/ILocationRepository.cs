using ClassifiedAds.Common.Dtos;
using ClassifiedAds.Common.Entities;

namespace ClassifiedAds.Common.Interfaces
{
    public interface ILocationRepository
    {
        // Countries
        Task<IEnumerable<Country>> GetAllCountriesAsync();
        Task<Country> GetCountryByIdAsync(int countryId);

        // Cities
        Task<IEnumerable<City>> GetCitiesByCountryIdAsync(int countryId);
        Task<City> GetCityByIdAsync(int cityId);

        Task<IEnumerable<City>> GetAllCitiesAsync();
    }
}
