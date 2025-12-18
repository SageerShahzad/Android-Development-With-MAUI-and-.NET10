using ClassifiedAds.Common.Data;
using ClassifiedAds.Common.Dtos;
using ClassifiedAds.Common.Entities;
using ClassifiedAds.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClassifiedAds.Common.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly ClassifiedAdsDbContext _ctx;
        public LocationRepository(ClassifiedAdsDbContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<Country>> GetAllCountriesAsync() =>
            await _ctx.Countries.ToListAsync();


        public async Task<IEnumerable<City>> GetAllCitiesAsync() =>
           await _ctx.Cities.ToListAsync();

        public async Task<Country> GetCountryByIdAsync(int countryId) =>
            await _ctx.Countries.FindAsync(countryId);

        public async Task<IEnumerable<City>> GetCitiesByCountryIdAsync(int countryId) =>
            await _ctx.Cities
                      .Where(c => c.CountryId == countryId)
                      .ToListAsync();

        public async Task<City> GetCityByIdAsync(int cityId) =>
            await _ctx.Cities.FindAsync(cityId);
    }
}