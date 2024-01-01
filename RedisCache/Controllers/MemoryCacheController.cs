using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RedisCache.Model;
using System.Net.Http;
using System.Text.Json;

namespace RedisCache.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemoryCacheController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpClientFactory _httpClientFactory;
        public MemoryCacheController(IMemoryCache memoryCache, IHttpClientFactory httpClientFactory)
        {
            _memoryCache = memoryCache;
            _httpClientFactory = httpClientFactory;

        }

        [HttpGet("random-api")]
        public async Task<IActionResult> GetDataAsync()
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://random-data-api.com/api/bank/random_bank?size=100");
            var httpClient = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                var res = await JsonSerializer.DeserializeAsync<IEnumerable<BankAccount>>(contentStream);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(300));

                _memoryCache.Set("random-api", res, cacheEntryOptions);
            }
            return Ok();
        }

        [HttpGet]
        public IActionResult GetCache()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var res = _memoryCache.Get("random-api");
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
            return Ok(res);
        }

        [HttpPost]
        public IActionResult SetCache()
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
           .SetSlidingExpiration(TimeSpan.FromSeconds(30));

            _memoryCache.Set("random-api", "B-1071", cacheEntryOptions);
            return Ok();
        }
    }
}
