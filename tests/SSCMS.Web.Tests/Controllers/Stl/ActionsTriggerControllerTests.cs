using System.Collections.Generic;
using System.Threading.Tasks;
using CacheManager.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Web.Controllers.Stl;
using Xunit;

namespace SSCMS.Web.Tests.Controllers.Stl
{
    public class ActionsTriggerControllerTests
    {
        [Fact]
        public async Task GetRejectsUnsignedTriggerRequests()
        {
            var createManager = new Mock<ICreateManager>();
            var siteRepository = new Mock<ISiteRepository>();
            siteRepository.Setup(x => x.GetAsync(1)).ReturnsAsync(new Site
            {
                Id = 1
            });

            var controller = new ActionsTriggerController(
                createManager.Object,
                Mock.Of<IPathManager>(),
                siteRepository.Object,
                Mock.Of<IChannelRepository>(),
                Mock.Of<IContentRepository>(),
                Mock.Of<ISettingsManager>(),
                new TestCacheManager());

            var result = await controller.Get(new ActionsTriggerController.GetRequest
            {
                SiteId = 1
            });

            Assert.IsType<UnauthorizedResult>(result);
            createManager.Verify(x => x.ExecuteAsync(
                    It.IsAny<int>(),
                    It.IsAny<CreateType>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public async Task GetAllowsSignedTriggerRequests()
        {
            var createManager = new Mock<ICreateManager>();
            var pathManager = new Mock<IPathManager>();
            pathManager.Setup(x => x.GetIndexPageUrlAsync(It.IsAny<Site>(), false)).ReturnsAsync("/");

            var siteRepository = new Mock<ISiteRepository>();
            siteRepository.Setup(x => x.GetAsync(1)).ReturnsAsync(new Site
            {
                Id = 1
            });

            var settingsManager = new Mock<ISettingsManager>();
            settingsManager
                .Setup(x => x.Decrypt("signed", null))
                .Returns("1:1:0:0:0:False");

            var controller = new ActionsTriggerController(
                createManager.Object,
                pathManager.Object,
                siteRepository.Object,
                Mock.Of<IChannelRepository>(),
                Mock.Of<IContentRepository>(),
                settingsManager.Object,
                new TestCacheManager());

            var result = await controller.Get(new ActionsTriggerController.GetRequest
            {
                SiteId = 1,
                ChannelId = 1,
                Token = "signed"
            });

            Assert.IsType<RedirectResult>(result);
            createManager.Verify(x => x.ExecuteAsync(1, CreateType.Channel, 1, 0, 0, 0), Times.Once);
        }

        [Fact]
        public async Task GetRateLimitsRepeatedSignedTriggerRequests()
        {
            var createManager = new Mock<ICreateManager>();
            var pathManager = new Mock<IPathManager>();
            pathManager.Setup(x => x.GetIndexPageUrlAsync(It.IsAny<Site>(), false)).ReturnsAsync("/");

            var siteRepository = new Mock<ISiteRepository>();
            siteRepository.Setup(x => x.GetAsync(1)).ReturnsAsync(new Site
            {
                Id = 1
            });

            var settingsManager = new Mock<ISettingsManager>();
            settingsManager
                .Setup(x => x.Decrypt("signed", null))
                .Returns("1:1:0:0:0:False");

            var controller = new ActionsTriggerController(
                createManager.Object,
                pathManager.Object,
                siteRepository.Object,
                Mock.Of<IChannelRepository>(),
                Mock.Of<IContentRepository>(),
                settingsManager.Object,
                new TestCacheManager())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            IActionResult lastResult = null;
            for (var i = 0; i < 61; i++)
            {
                lastResult = await controller.Get(new ActionsTriggerController.GetRequest
                {
                    SiteId = 1,
                    ChannelId = 1,
                    Token = "signed"
                });
            }

            Assert.IsType<BadRequestObjectResult>(lastResult);
            createManager.Verify(x => x.ExecuteAsync(1, CreateType.Channel, 1, 0, 0, 0), Times.Exactly(60));
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
