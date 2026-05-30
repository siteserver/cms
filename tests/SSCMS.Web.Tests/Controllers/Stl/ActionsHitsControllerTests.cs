using System.Collections.Generic;
using System.Threading.Tasks;
using CacheManager.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Web.Controllers.Stl;
using Xunit;

namespace SSCMS.Web.Tests.Controllers.Stl
{
    public class ActionsHitsControllerTests
    {
        [Fact]
        public async Task SubmitRateLimitsRepeatedAnonymousRequests()
        {
            var contentRepository = new Mock<IContentRepository>();
            contentRepository
                .Setup(x => x.GetHitsAsync(1, 2, 3))
                .ReturnsAsync(10);

            var controller = new ActionsHitsController(contentRepository.Object, new TestCacheManager())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            ActionResult lastResult = null;
            for (var i = 0; i < 61; i++)
            {
                var result = await controller.Submit(new ActionsHitsController.SubmitRequest
                {
                    SiteId = 1,
                    ChannelId = 2,
                    ContentId = 3,
                    AutoIncrease = true
                });
                lastResult = result.Result;
            }

            Assert.IsType<BadRequestObjectResult>(lastResult);
            contentRepository.Verify(x => x.UpdateHitsAsync(1, 2, 3, 11), Times.Exactly(60));
        }

        private class TestCacheManager : ICacheManager
        {
            private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

            public IReadOnlyCacheManagerConfiguration Configuration => null;

            public T Get<T>(string key)
            {
                return _cache.TryGetValue(key, out var value) ? (T)value : default;
            }

            public string GetByFilePath(string filePath)
            {
                return string.Empty;
            }

            public bool Exists(string key)
            {
                return _cache.ContainsKey(key);
            }

            public void AddOrUpdateSliding<T>(string key, T value, int minutes)
            {
                _cache[key] = value;
            }

            public void AddOrUpdateAbsolute<T>(string key, T value, int minutes)
            {
                _cache[key] = value;
            }

            public void AddOrUpdate<T>(string key, T value)
            {
                _cache[key] = value;
            }

            public void Remove(string key)
            {
                _cache.Remove(key);
            }

            public void Clear()
            {
                _cache.Clear();
            }
        }
    }
}
