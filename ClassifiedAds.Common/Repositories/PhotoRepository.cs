using ClassifiedAds.Common.Data;
using ClassifiedAds.Common.DTOs;
using ClassifiedAds.Common.Entities;
using ClassifiedAds.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClassifiedAds.Common.Repositories;

public class PhotoRepository(ClassifiedAdsDbContext context) : IPhotoRepository
{
    public async Task<Photo?> GetPhotoById(int id)
    {
        return await context.Photos
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IReadOnlyList<PhotoForApprovalDto>> GetUnapprovedPhotos()
    {
        return await context.Photos
            .IgnoreQueryFilters()
            .Where(p => p.IsApproved == false)
            .Select(u => new PhotoForApprovalDto
            {
                Id = u.Id,
                UserId = u.MemberId,
                Url = u.Url,
                IsApproved = u.IsApproved
            }).ToListAsync();
    }

    public void RemovePhoto(Photo photo)
    {
        context.Photos.Remove(photo);
    }
}
