using ClassifiedAds.Common.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ClassifiedAds.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
    }
}
