using System.Collections.Generic;
using System.Threading.Tasks;
using CacheManager.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SSCMS.Core.StlParser.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Web.Controllers.Stl;
using Xunit;

namespace SSCMS.Web.Tests.Controllers.Stl
{
    public class ActionsPublicRateLimitTests
    {
        [Fact]
        public void SearchHighlightEscapesRegexMetaCharacters()
        {
            var pattern = ActionsSearchController.GetHighlightRegexPattern("(a+)+ test");

            Assert.DoesNotContain("(a+)+", pattern);
            Assert.Matches(pattern, "(a+)+ test");
            Assert.DoesNotMatch(pattern, "aaaa test");
        }

        [Fact]
        public async Task SearchRateLimitsRepeatedAnonymousRequests()
        {
            var controller = new ActionsSearchController(
                Mock.Of<ISettingsManager>(),
                Mock.Of<IAuthManager>(),
                Mock.Of<IParseManager>(),
                Mock.Of<IDatabaseManager>(),
                Mock.Of<ISiteRepository>(),
                Mock.Of<IContentRepository>(),
                new TestCacheManager())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            ActionResult lastResult = null;
            for (var i = 0; i < 61; i++)
            {
                var result = await controller.Submit(new StlSearchRequest
                {
                    SiteId = 1
                });
                lastResult = result.Result;
            }

            Assert.IsType<BadRequestObjectResult>(lastResult);
        }

        [Fact]
        public async Task DynamicRateLimitsRepeatedAnonymousRequests()
        {
            var settingsManager = new Mock<ISettingsManager>();
            settingsManager.Setup(x => x.Decrypt("{}", null)).Returns("{}");

            var controller = new ActionsDynamicController(
                settingsManager.Object,
                Mock.Of<IAuthManager>(),
                Mock.Of<IParseManager>(),
                new TestCacheManager())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            ActionResult lastResult = null;
            for (var i = 0; i < 61; i++)
            {
                var result = await controller.Submit(new ActionsDynamicController.SubmitRequest
                {
                    Value = "{}"
                });
                lastResult = result.Result;
            }

            Assert.IsType<BadRequestObjectResult>(lastResult);
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
