using ClassifiedAds.Common.Data;
using ClassifiedAds.Common.Dtos;
using ClassifiedAds.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClassifiedAds.Common.Services
{
    public class AdService : IAdService
    {
        private readonly ClassifiedAdsDbContext _context;

        public AdService(ClassifiedAdsDbContext context)
        {
            _context = context;
        }

        public async Task<AdDTO> GetAdByIdAsync(int adId, string userId)
        {
            var ad = await _context.Ads
                .Include(a => a.Category)
                .Include(a => a.SubCategory)
                .Include(a => a.Member) // Include Advertiser details
                .FirstOrDefaultAsync(a => a.Id == adId);

            if (ad == null)
            {
                throw new Exception("Ad not found");
            }

            // Fetch the user from the context using the userId
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            // If the user is an advertiser and not the owner of the ad, deny access
            //if (user.Role == "Advertiser" && ad.Advertiser.Id != user.Id)
            //{
            //    throw new UnauthorizedAccessException("You do not have permission to manage this ad");
            //}

            // If the user is in Audience, they can view but not manage.
            return new AdDTO
            {
                Id = ad.Id,
                Title = ad.Title,
                Description = ad.Description,
                Price = ad.Price,
                MainImageUrl = ad.ImageUrl,
                Category = ad.Category.Name,
                SubCategory = ad.SubCategory.Name,
                MemberId = ad.MemberId // This is an int, still fine for DTOs
            };
        }
    }
}
