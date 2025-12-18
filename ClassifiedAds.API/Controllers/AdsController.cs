using ClassifiedAds.Common.Data;
using ClassifiedAds.Common.Dtos;
using ClassifiedAds.Common.Entities;
// The namespace for the extension method is necessary here
using ClassifiedAds.Common.Extensions;
using System.Security.Claims; // Needed for the extension method fix
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassifiedAds.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdsController(ClassifiedAdsDbContext context) : ControllerBase
    {
        private readonly ClassifiedAdsDbContext _context = context;

        // Note: The AdDTO definition seems to have two image properties (ImageUrl and MainImageUrl).
        // I will use MainImageUrl for the primary ad image in the DTO mapping.

        [HttpGet("openads")]
        public async Task<ActionResult<List<AdDTO>>> GetOpenAds()
        {
            var ads = await _context.Ads
                .Include(a => a.Country)
                .Include(a => a.City)
                .Include(a => a.Category)
                .Include(a => a.SubCategory)
                .Include(a => a.Member)
                .Include(ad => ad.Photos)
                .ToListAsync();

            var adDtos = ads.Select(MapToDto).ToList();
            return Ok(adDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AdDTO>> GetSingleAdById(int id)
        {
            var ad = await _context.Ads
                .Include(a => a.Country)
                .Include(a => a.City)
                .Include(a => a.Category)
                .Include(a => a.SubCategory)
                .Include(a => a.Member)
                .Include(a => a.Photos)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (ad == null) return NotFound();

            return Ok(MapToDto(ad));
        }

        [HttpGet("myads")]
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<List<AdDTO>>> GetMyAds()
        {
            // FIX: User.GetMemberId() now works with the extension method
            var userId = User.GetMemberId();

            var myAds = await _context.Ads
                .Where(ad => ad.MemberId == userId) // Security: Only fetch own ads
                .Include(a => a.Country)
                .Include(a => a.City)
                .Include(ad => ad.Category)
                .Include(ad => ad.SubCategory)
                .Include(ad => ad.Photos)
                .ToListAsync();

            return Ok(myAds.Select(MapToDto));
        }

        [HttpPost]
        [Authorize(Roles = "Member")] //api/ads
        public async Task<ActionResult<AdDTO>> CreateAd([FromBody] AdCreateDTO model)
        {
            if (model == null) return BadRequest("Invalid data");

            // FIX: User.GetMemberId() now works
            var userId = User.GetMemberId();

            // Validate Foreign Keys exist
            // (You should also validate CountryId and CategoryId)
            if (!await _context.SubCategories.AnyAsync(sc => sc.Id == model.SubCategoryId))
                return BadRequest("Invalid SubCategoryId.");
            if (!await _context.Cities.AnyAsync(c => c.Id == model.CityId))
                return BadRequest("Invalid CityId.");

            // Create Entity
            var ad = new Ad
            {
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                // Assuming AdCreateDTO includes CategoryId and CountryId
                CategoryId = model.CategoryId,
                SubCategoryId = model.SubCategoryId,
                CityId = model.CityId,
                CountryId = model.CountryId,
                PostalCode = model.PostalCode,
                MemberId = userId, // Force assignment from Token
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                ImageUrl = model.ImageUrls.FirstOrDefault() ?? string.Empty // First image as main image
            };

            _context.Ads.Add(ad);

            // Handle Photos (create Photo entities)
            if (model.ImageUrls.Any())
            {
                var isFirst = true;
                foreach (var url in model.ImageUrls)
                {
                    var photo = new Photo
                    {
                        Url = url,
                        IsApproved = true,
                        MemberId = userId,
                        Ad = ad // Link back to the Ad entity
                    };
                    _context.Photos.Add(photo);
                    isFirst = false;
                }
            }

            await _context.SaveChangesAsync();

            // Return the created DTO
            return CreatedAtAction(nameof(GetSingleAdById), new { id = ad.Id }, MapToDto(ad));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> UpdateAd(int id, [FromBody] AdUpdateDTO model)
        {
            var userId = User.GetMemberId(); // FIX: User.GetMemberId() now works

            var ad = await _context.Ads
                .Include(a => a.Photos)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (ad == null) return NotFound("Ad not found.");

            // Authorization Check
            if (ad.MemberId != userId)
            {
                return Forbid();
            }

            // Update properties
            ad.Title = model.Title;
            ad.Description = model.Description;
            ad.Price = model.Price;
            ad.CategoryId = model.CategoryId;
            ad.SubCategoryId = model.SubCategoryId;
            // Assuming model.MainImageUrl is the canonical URL
            ad.ImageUrl = model.MainImageUrl;
            ad.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // You might need to reload navigation properties if you want full details in the returned DTO
            // For simplicity, we assume the helper MapToDto works with the updated Ad entity.
            return Ok(MapToDto(ad));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> DeleteAd(int id)
        {
            var memberId = User.GetMemberId(); // FIX: User.GetMemberId() now works
            var ad = await _context.Ads.FindAsync(id);

            if (ad == null) return NotFound();

            // Authorization Check
            if (ad.MemberId != memberId) return Forbid();

            // Photos will delete via Cascade if configured in DbContext, otherwise delete manually
            var photos = await _context.Photos.Where(x => x.AdId == id).ToListAsync();
            _context.Photos.RemoveRange(photos);

            _context.Ads.Remove(ad);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // --- Helper Mapping Method ---
        private static AdDTO MapToDto(Ad ad)
        {
            return new AdDTO
            {
                Id = ad.Id,
                Title = ad.Title,
                Description = ad.Description,
                Price = ad.Price,
                // Null-safe mapping for navigation properties
                Country = ad.Country?.Name ?? "Unknown",
                City = ad.City?.Name ?? "Unknown",
                Category = ad.Category?.Name ?? "Unknown",
                SubCategory = ad.SubCategory?.Name ?? "Unknown",
                PostalCode = ad.PostalCode,
                CreatedDate = ad.CreatedDate,
                // Mapped entity's ImageUrl to DTO's MainImageUrl
                MainImageUrl = ad.ImageUrl ?? string.Empty,
                // ImageUrl is left empty or defined as a list of photo URLs if needed
                MemberId = ad.MemberId,
            };
        }
    }
}