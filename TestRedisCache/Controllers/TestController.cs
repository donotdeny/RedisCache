using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestRedisCache.Model;

namespace TestRedisCache.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        public TestController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCacheItemAsync()
        {
            var res = await _cacheService.GetItemAsync<IEnumerable<BankAccount>>("random-api");
            return Ok(res);
        }
    }
}
