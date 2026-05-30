using System.Collections.Generic;
using System.Threading.Tasks;
using CacheManager.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using HomeLoginController = SSCMS.Web.Controllers.Home.LoginController;
using HomeLostPasswordController = SSCMS.Web.Controllers.Home.LostPasswordController;
using HomeProfileController = SSCMS.Web.Controllers.Home.ProfileController;
using HomeRegisterController = SSCMS.Web.Controllers.Home.RegisterController;
using HomeVerifyMobileController = SSCMS.Web.Controllers.Home.VerifyMobileController;
using V1FormsController = SSCMS.Web.Controllers.V1.FormsController;
using Xunit;

namespace SSCMS.Web.Tests.Controllers.Home
{
    public class HomeSmsRateLimitTests
    {
        [Fact]
        public async Task LoginSendSmsRateLimitsRepeatedRequests()
        {
            var smsManager = CreateSmsManager();
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.GetByMobileAsync("13800000000")).ReturnsAsync(new User());
            userRepository.Setup(x => x.ValidateStateAsync(It.IsAny<User>())).ReturnsAsync((true, string.Empty));

            var controller = WithHttpContext(new HomeLoginController(
                Mock.Of<ISettingsManager>(),
                Mock.Of<IAuthManager>(),
                new TestCacheManager(),
                smsManager.Object,
                Mock.Of<IConfigRepository>(),
                userRepository.Object,
                Mock.Of<ILogRepository>(),
                Mock.Of<IStatRepository>()));

            var lastResult = await SendRepeatedly(() => controller.SendSms(new HomeLoginController.SendSmsRequest
            {
                Mobile = "13800000000"
            }));

            Assert.IsType<BadRequestObjectResult>(lastResult.Result);
            VerifySmsCount(smsManager, 5);
        }

        [Fact]
        public async Task LostPasswordSendSmsRateLimitsRepeatedRequests()
        {
            var smsManager = CreateSmsManager();
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.GetByMobileAsync("13800000000")).ReturnsAsync(new User());
            userRepository.Setup(x => x.ValidateStateAsync(It.IsAny<User>())).ReturnsAsync((true, string.Empty));

            var controller = WithHttpContext(new HomeLostPasswordController(
                Mock.Of<IAuthManager>(),
                new TestCacheManager(),
                smsManager.Object,
                userRepository.Object));

            var lastResult = await SendRepeatedly(() => controller.SendSms(new HomeLostPasswordController.SendSmsRequest
            {
                Mobile = "13800000000"
            }));

            Assert.IsType<BadRequestObjectResult>(lastResult.Result);
            VerifySmsCount(smsManager, 5);
        }

        [Fact]
        public async Task RegisterSendSmsRateLimitsRepeatedRequests()
        {
            var smsManager = CreateSmsManager();
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.IsMobileExistsAsync("13800000000")).ReturnsAsync(false);

            var controller = WithHttpContext(new HomeRegisterController(
                Mock.Of<ISettingsManager>(),
                Mock.Of<ICloudManager>(),
                smsManager.Object,
                new TestCacheManager(),
                Mock.Of<IConfigRepository>(),
                Mock.Of<ITableStyleRepository>(),
                userRepository.Object,
                Mock.Of<IUserGroupRepository>(),
                Mock.Of<IStatRepository>()));

            var lastResult = await SendRepeatedly(() => controller.SendSms(new HomeRegisterController.SendSmsRequest
            {
                Mobile = "13800000000"
            }));

            Assert.IsType<BadRequestObjectResult>(lastResult.Result);
            VerifySmsCount(smsManager, 5);
        }

        [Fact]
        public async Task ProfileSendSmsRateLimitsRepeatedRequests()
        {
            var smsManager = CreateSmsManager();
            var authManager = new Mock<IAuthManager>();
            authManager.Setup(x => x.GetUserAsync()).ReturnsAsync(new User { Mobile = "13800000000" });

            var controller = WithHttpContext(new HomeProfileController(
                authManager.Object,
                Mock.Of<IPathManager>(),
                Mock.Of<ICloudManager>(),
                smsManager.Object,
                new TestCacheManager(),
                Mock.Of<IConfigRepository>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<ITableStyleRepository>(),
                Mock.Of<IRelatedFieldItemRepository>()));

            var lastResult = await SendRepeatedly(() => controller.SendSms(new HomeProfileController.SendSmsRequest
            {
                Mobile = "13800000000"
            }));

            Assert.IsType<BadRequestObjectResult>(lastResult.Result);
            VerifySmsCount(smsManager, 5);
        }

        [Fact]
        public async Task VerifyMobileSendSmsRateLimitsRepeatedRequests()
        {
            var smsManager = CreateSmsManager();
            var authManager = new Mock<IAuthManager>();
            authManager.Setup(x => x.GetUserAsync()).ReturnsAsync(new User { Mobile = "13800000000" });

            var controller = WithHttpContext(new HomeVerifyMobileController(
                authManager.Object,
                new TestCacheManager(),
                smsManager.Object,
                Mock.Of<IUserRepository>()));

            var lastResult = await SendRepeatedly(() => controller.SendSms(new HomeVerifyMobileController.SendSmsRequest
            {
                Mobile = "13800000000"
            }));

            Assert.IsType<BadRequestObjectResult>(lastResult.Result);
            VerifySmsCount(smsManager, 5);
        }

        [Fact]
        public async Task V1FormsSendSmsRateLimitsRepeatedRequests()
        {
            var smsManager = CreateSmsManager();
            smsManager.Setup(x => x.IsSmsEnabledAsync()).ReturnsAsync(true);
            var formRepository = new Mock<IFormRepository>();
            formRepository.Setup(x => x.GetAsync(1, 7)).ReturnsAsync(new Form
            {
                Id = 7,
                IsSms = true
            });

            var controller = WithHttpContext(new V1FormsController(
                new TestCacheManager(),
                Mock.Of<IPathManager>(),
                smsManager.Object,
                Mock.Of<IFormManager>(),
                Mock.Of<ISiteRepository>(),
                formRepository.Object,
                Mock.Of<IFormDataRepository>()));

            var formRequest = new V1FormsController.FormRequest
            {
                SiteId = 1,
                FormId = 7
            };
            var lastResult = await SendRepeatedly(() => controller.SendSms(formRequest, new V1FormsController.SendSmsRequest
            {
                Mobile = "13800000000"
            }));

            Assert.IsType<BadRequestObjectResult>(lastResult.Result);
            VerifySmsCount(smsManager, 5);
        }

        private static Mock<ISmsManager> CreateSmsManager()
        {
            var smsManager = new Mock<ISmsManager>();
            smsManager
                .Setup(x => x.SendSmsAsync(It.IsAny<string>(), It.IsAny<SmsCodeType>(), It.IsAny<int>()))
                .ReturnsAsync((true, string.Empty));
            return smsManager;
        }

        private static void VerifySmsCount(Mock<ISmsManager> smsManager, int count)
        {
            smsManager.Verify(
                x => x.SendSmsAsync(It.IsAny<string>(), It.IsAny<SmsCodeType>(), It.IsAny<int>()),
                Times.Exactly(count));
        }

        private static T WithHttpContext<T>(T controller) where T : ControllerBase
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            return controller;
        }

        private static async Task<ActionResult<BoolResult>> SendRepeatedly(
            System.Func<Task<ActionResult<BoolResult>>> sendSms)
        {
            ActionResult<BoolResult> lastResult = null;
            for (var i = 0; i < 6; i++)
            {
                lastResult = await sendSms();
            }

            return lastResult;
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
