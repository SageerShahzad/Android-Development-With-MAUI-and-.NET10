using ClassifiedAds.Common.Entities;
using ClassifiedAds.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClassifiedAds.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _repo;

        public CategoriesController(ICategoryRepository repo)
        {
            _repo = repo;
        }

        // GET: /api/categories
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cats = await _repo.GetAllCategoriesAsync();
            return Ok(cats);
        }

        // GET: /api/categories/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cat = await _repo.GetCategoryByIdAsync(id);
            if (cat == null) return NotFound();
            return Ok(cat);
        }
    }
}
