using ClassifiedAds.Common.Dtos;
using ClassifiedAds.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClassifiedAds.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly ILocationRepository _loc;

        public CountriesController(ILocationRepository loc)
        {
            _loc = loc;
        }

        // GET: api/countries
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // 1. Get Entities from DB
            var countries = await _loc.GetAllCountriesAsync();

            // 2. Map Entity -> Dto
            // This prevents the "Cities" list from being serialized
            var dtos = countries.Select(c => new CountryDto
            {
                Id = c.Id,
                IsoCode = c.IsoCode,
                Name = c.Name
            });

            return Ok(dtos);
        }

        // GET: api/countries/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var country = await _loc.GetCountryByIdAsync(id);

            if (country == null) return NotFound();

            // 3. Map single Entity -> Dto
            var dto = new CountryDto
            {
                Id = country.Id,
                IsoCode = country.IsoCode,
                Name = country.Name
            };

            return Ok(dto);
        }
    }
}