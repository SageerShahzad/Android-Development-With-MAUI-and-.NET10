
using ClassifiedAds.Common.DTOs;
using ClassifiedAds.Common.Entities;

namespace ClassifiedAds.Common.Interfaces;

public interface IPhotoRepository
{
    Task<IReadOnlyList<PhotoForApprovalDto>> GetUnapprovedPhotos();
    Task<Photo?> GetPhotoById(int id);
    void RemovePhoto(Photo photo);
}
