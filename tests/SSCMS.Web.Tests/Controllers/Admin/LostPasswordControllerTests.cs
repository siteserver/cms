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
    public class LostPasswordControllerTests
    {
        [Fact]
        public async Task SendSmsReturnsSuccessWithoutSendingSmsForUnknownMobile()
        {
            var authManager = new Mock<IAuthManager>();
            var cacheManager = new Mock<ICacheManager>();
            var smsManager = new Mock<ISmsManager>();
            var administratorRepository = new Mock<IAdministratorRepository>();
            administratorRepository
                .Setup(x => x.GetByMobileAsync("13800000000"))
                .ReturnsAsync(() => null);

            var controller = new LostPasswordController(
                authManager.Object,
                cacheManager.Object,
                smsManager.Object,
                administratorRepository.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var result = await controller.SendSms(new LostPasswordController.SendSmsRequest
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
        public async Task SubmitRemovesSmsCodeAfterSuccessfulPasswordReset()
        {
            var authManager = new Mock<IAuthManager>();
            var cacheManager = new Mock<ICacheManager>();
            cacheManager
                .Setup(x => x.Get<int>(It.IsAny<string>()))
                .Returns(123456);

            var smsManager = new Mock<ISmsManager>();
            var administratorRepository = new Mock<IAdministratorRepository>();
            var administrator = new Administrator
            {
                UserName = "admin",
                Mobile = "13800000000"
            };
            administratorRepository
                .Setup(x => x.GetByMobileAsync("13800000000"))
                .ReturnsAsync(administrator);
            administratorRepository
                .Setup(x => x.ChangePasswordAsync(administrator, "new-password"))
                .ReturnsAsync((true, string.Empty));

            var controller = new LostPasswordController(
                authManager.Object,
                cacheManager.Object,
                smsManager.Object,
                administratorRepository.Object);

            var result = await controller.Submit(new LostPasswordController.SubmitRequest
            {
                Mobile = "13800000000",
                Code = "123456",
                Password = "new-password"
            });

            var value = Assert.IsType<BoolResult>(result.Value);
            Assert.True(value.Value);
            cacheManager.Verify(x => x.Remove(It.IsAny<string>()), Times.Once);
        }
    }
}
