using ClassifiedAds.Common.Entities;

namespace ClassifiedAds.Common.Interfaces
{
    public interface IAdRepository
    {
        Task<Ad> GetAdByIdAsync(int id);
        Task<IEnumerable<Ad>> GetAdsByCategoryAsync(int categoryId);
        Task<IEnumerable<Ad>> GetAdsBySubCategoryAsync(int subCategoryId);
        Task<IEnumerable<Ad>> GetAdsByAdvertiserIdAsync(int advertiserId);
        Task<IEnumerable<Ad>> GetAllAdsAsync();
        Task AddAdAsync(Ad ad);
        Task UpdateAdAsync(Ad ad);
        Task DeleteAdAsync(int id);
       
    }
}
