using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace ClassifiedAds.Common.Interfaces;

public interface IPhotoService
{
    Task<ImageUploadResult> UploadPhotoAsync(IFormFile file);
    Task<DeletionResult> DeletePhotoAsync(string publicId);
}
