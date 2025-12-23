using ClassifiedAds.Mobile.Models;

namespace ClassifiedAds.Mobile.Repositories;

public interface ICategoryRepository
{
    Task<List<CategoryModel>> GetCategories();
}
