using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Web.Controllers.Admin;
using Xunit;

namespace SSCMS.Web.Tests.Controllers.Admin
{
    public class LoginControllerTests
    {
        [Fact]
        public async Task SendSmsReturnsSuccessWithoutSendingSmsForUnknownMobile()
        {
            var smsManager = new Mock<ISmsManager>();
            var cacheManager = new Mock<ICacheManager>();
            var administratorRepository = new Mock<IAdministratorRepository>();
            administratorRepository
                .Setup(x => x.GetByMobileAsync("13800000000"))
                .ReturnsAsync(() => null);

            var controller = new LoginController(
                Mock.Of<ISettingsManager>(),
                Mock.Of<IAuthManager>(),
                Mock.Of<IPathManager>(),
                cacheManager.Object,
                smsManager.Object,
                Mock.Of<IConfigRepository>(),
                administratorRepository.Object,
                Mock.Of<IDbCacheRepository>(),
                Mock.Of<ILogRepository>(),
                Mock.Of<IStatRepository>())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var result = await controller.SendSms(new LoginController.SendSmsRequest
            {
                Mobile = "13800000000"
            });

            var value = Assert.IsType<BoolResult>(result.Value);
            Assert.True(value.Value);
            smsManager.Verify(
                x => x.SendSmsAsync(It.IsAny<string>(), It.IsAny<SSCMS.Enums.SmsCodeType>(), It.IsAny<int>()),
                Times.Never);
            cacheManager.Verify(
                x => x.AddOrUpdateAbsolute(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public async Task SubmitRequiresCaptchaWhenForceLogoutRequested()
        {
            var configRepository = new Mock<IConfigRepository>();
            configRepository.Setup(x => x.GetAsync()).ReturnsAsync(new Config
            {
                IsAdminCaptchaDisabled = false
            });

            var administratorRepository = new Mock<IAdministratorRepository>();

            var controller = new LoginController(
                Mock.Of<ISettingsManager>(),
                Mock.Of<IAuthManager>(),
                Mock.Of<IPathManager>(),
                Mock.Of<ICacheManager>(),
                Mock.Of<ISmsManager>(),
                configRepository.Object,
                administratorRepository.Object,
                Mock.Of<IDbCacheRepository>(),
                Mock.Of<ILogRepository>(),
                Mock.Of<IStatRepository>())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var result = await controller.Submit(new LoginController.SubmitRequest
            {
                Account = "admin",
                Password = "bad-password",
                IsForceLogoutAndLogin = true
            });

            Assert.IsType<BadRequestObjectResult>(result.Result);
            administratorRepository.Verify(x => x.ValidateAsync(It.IsAny<string>(), It.IsAny<string>(), true), Times.Never);
        }

        [Fact]
        public void SubmitResultUsesSanitizedAdministratorDto()
        {
            var property = typeof(LoginController.SubmitResult).GetProperty(nameof(LoginController.SubmitResult.Administrator));

            Assert.NotNull(property);
            Assert.NotEqual(typeof(Administrator), property.PropertyType);
            Assert.Null(property.PropertyType.GetProperty(nameof(Administrator.Password)));
            Assert.Null(property.PropertyType.GetProperty(nameof(Administrator.PasswordSalt)));
            Assert.Null(property.PropertyType.GetProperty(nameof(Administrator.Email)));
            Assert.Null(property.PropertyType.GetProperty(nameof(Administrator.Mobile)));
        }
    }
}
