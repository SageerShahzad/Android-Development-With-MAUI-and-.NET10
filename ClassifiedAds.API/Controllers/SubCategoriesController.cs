using ClassifiedAds.Common.Entities;
using ClassifiedAds.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClassifiedAds.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubCategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _repo;

        public SubCategoriesController(ICategoryRepository repo)
        {
            _repo = repo;
        }

        // GET: /api/subcategories
        // returns *all* subcategories across every category
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // if you want all subcategories, you’ll need to add a method
            // to your repo like GetAllSubCategoriesAsync(). For now:
            var allCats = await _repo.GetAllCategoriesAsync();
            var allSubs = new List<SubCategory>();
            foreach (var c in allCats)
                allSubs.AddRange(await _repo.GetSubCategoriesByCategoryIdAsync(c.Id));
            return Ok(allSubs);
        }

        // GET: /api/subcategories/bycategory/1
        [HttpGet("bycategory/{categoryId:int}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var subs = await _repo.GetSubCategoriesByCategoryIdAsync(categoryId);
            return Ok(subs);
        }

        // GET: /api/subcategories/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sub = await _repo.GetSubCategoryByIdAsync(id);
            if (sub == null) return NotFound();
            return Ok(sub);
        }
    }
}
