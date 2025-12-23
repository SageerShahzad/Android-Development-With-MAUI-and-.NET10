
using ClassifiedAds.Mobile.Models;

namespace ClassifiedAds.Mobile.Services;

public interface ICategoryService
{
    Task<List<CategoryModel>> GetCategories();
}
