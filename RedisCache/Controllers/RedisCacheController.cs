using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RedisCache.DistributedCache;
using RedisCache.Model;
using System.Net.Http;
using System.Text.Json;

namespace RedisCache.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisCacheController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly IHttpClientFactory _httpClientFactory;
        public RedisCacheController(ICacheService cacheService, IHttpClientFactory httpClientFactory)
        {

            _cacheService = cacheService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetCacheAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var res = await _cacheService.GetItemAsync<IEnumerable<BankAccount>>("random-api");
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> SetCacheAsync()
        {
            //var res = await _cacheService.SetItemAsync("nvdung1", "B-1071", DateTime.Now.AddSeconds(100));
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://random-data-api.com/api/bank/random_bank?size=100");
            var httpClient = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                var res = await JsonSerializer.DeserializeAsync<IEnumerable<BankAccount>>(contentStream);
                if(res != null)
                {
                    await _cacheService.SetItemAsync("random-api", res, DateTime.Now.AddSeconds(100));
                }
            }
            return Ok();
        }
    }
}
