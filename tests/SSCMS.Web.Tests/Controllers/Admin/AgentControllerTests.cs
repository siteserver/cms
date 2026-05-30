using System.Collections.Generic;
using System.Threading.Tasks;
using CacheManager.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Web.Controllers.Admin;
using Xunit;

namespace SSCMS.Web.Tests.Controllers.Admin
{
    public class AgentControllerTests
    {
        [Fact]
        public async Task InstallRateLimitsRepeatedBadSecurityKeys()
        {
            var settingsManager = new Mock<ISettingsManager>();
            settingsManager.Setup(x => x.SecurityKey).Returns("correct-key");

            var administratorRepository = new Mock<IAdministratorRepository>();
            administratorRepository
                .Setup(x => x.InsertValidateAsync("admin", "Password1", string.Empty, string.Empty))
                .ReturnsAsync((true, string.Empty));

            var controller = new AgentController(
                settingsManager.Object,
                Mock.Of<IPathManager>(),
                Mock.Of<IDatabaseManager>(),
                Mock.Of<IPluginManager>(),
                new TestCacheManager(),
                Mock.Of<ICreateManager>(),
                Mock.Of<IConfigRepository>(),
                administratorRepository.Object,
                Mock.Of<ISiteRepository>(),
                Mock.Of<IDbCacheRepository>(),
                Mock.Of<IContentRepository>())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            for (var i = 0; i < 10; i++)
            {
                await controller.Install(new AgentController.InstallRequest
                {
                    SecurityKey = "bad-key",
                    UserName = "admin",
                    Password = "Password1"
                });
            }

            var result = await controller.Install(new AgentController.InstallRequest
            {
                SecurityKey = "correct-key",
                UserName = "admin",
                Password = "Password1"
            });

            Assert.IsType<BadRequestObjectResult>(result.Result);
            administratorRepository.Verify(x => x.InsertValidateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
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
