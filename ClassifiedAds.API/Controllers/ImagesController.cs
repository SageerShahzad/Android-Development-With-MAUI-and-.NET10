using Microsoft.AspNetCore.Mvc;

namespace ClassifiedAds.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    namespace ClassifiedAds.API.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class ImagesController : ControllerBase
        {
            private readonly IWebHostEnvironment _env;
            public ImagesController(IWebHostEnvironment env) => _env = env;

            [HttpGet]
            public IActionResult GetAll()
            {
                var imgDir = Path.Combine(_env.WebRootPath!, "images/users/5");
                if (!Directory.Exists(imgDir))
                    return NotFound();

                // Only pick common image extensions:
                var files = Directory
                    .EnumerateFiles(imgDir)
                    .Where(f => f.EndsWith(".png") || f.EndsWith(".jpg") || f.EndsWith(".jpeg") || f.EndsWith(".gif"))
                    .Select(Path.GetFileName)   // returns e.g. "image1.png"
                    .ToList();

                return Ok(files);
            }
        }
    }

}
