using ClassifiedAds.Common.Dtos;
using ClassifiedAds.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClassifiedAds.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController(ILocationRepository loc) : ControllerBase
    {
        // 1. GET: /api/cities/bycountry/1
        [HttpGet("bycountry/{countryId:int}")]
        public async Task<IActionResult> GetByCountry(int countryId)
        {
            var cities = await loc.GetCitiesByCountryIdAsync(countryId);

            // Manual Mapping: Entity -> Dto
            var dtos = cities.Select(c => new CityDto
            {
                Id = c.Id,
                Name = c.Name,
                CountryId = c.CountryId
            });

            return Ok(dtos);
        }

        // 2. GET: /api/cities/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var city = await loc.GetCityByIdAsync(id);
            if (city == null) return NotFound();

            // Manual Mapping: Entity -> Dto
            var dto = new CityDto
            {
                Id = city.Id,
                Name = city.Name,
                CountryId = city.CountryId
            };

            return Ok(dto);
        }

        // 3. GET: /api/cities
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // REWRITTEN: Previously this returned entities directly.
            var cities = await loc.GetAllCitiesAsync();

            return Ok(cities);
        }
    }
}