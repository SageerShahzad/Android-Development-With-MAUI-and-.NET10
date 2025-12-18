using ClassifiedAds.Common.Entities;

namespace ClassifiedAds.Common.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);                // ← add
        
        Task<IEnumerable<SubCategory>> GetSubCategoriesByCategoryIdAsync(int categoryId);
        Task<SubCategory> GetSubCategoryByIdAsync(int id);
    }
}
