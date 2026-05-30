using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Web.Controllers.Admin.Settings.Administrators;
using Xunit;

namespace SSCMS.Web.Tests.Controllers.Admin.Settings.Administrators
{
    public class AdministratorsControllerTests
    {
        [Fact]
        public async Task SavePermissionsRequiresSuperAdmin()
        {
            var authManager = new Mock<IAuthManager>();
            authManager
                .Setup(x => x.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministrators))
                .ReturnsAsync(true);
            authManager
                .Setup(x => x.IsSuperAdminAsync())
                .ReturnsAsync(false);

            var administratorRepository = new Mock<IAdministratorRepository>();
            administratorRepository
                .Setup(x => x.GetByUserIdAsync(9))
                .ReturnsAsync(new Administrator { Id = 9, UserName = "target" });
            administratorRepository
                .Setup(x => x.AddUserToRoleAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            administratorRepository
                .Setup(x => x.AddUserToRolesAsync(It.IsAny<string>(), It.IsAny<string[]>()))
                .Returns(Task.CompletedTask);
            administratorRepository
                .Setup(x => x.UpdateSiteIdsAsync(It.IsAny<Administrator>(), It.IsAny<List<int>>()))
                .Returns(Task.CompletedTask);
            administratorRepository
                .Setup(x => x.GetRolesAsync(It.IsAny<string>()))
                .ReturnsAsync("roles");

            var administratorsInRolesRepository = new Mock<IAdministratorsInRolesRepository>();
            administratorsInRolesRepository
                .Setup(x => x.RemoveUserAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var controller = new AdministratorsController(
                Mock.Of<ICacheManager>(),
                authManager.Object,
                Mock.Of<IPathManager>(),
                Mock.Of<IDatabaseManager>(),
                administratorRepository.Object,
                Mock.Of<IRoleRepository>(),
                Mock.Of<ISiteRepository>(),
                administratorsInRolesRepository.Object);

            var result = await controller.SavePermissions(9, new AdministratorsController.SavePermissionsRequest
            {
                AdminLevel = "SuperAdmin",
                CheckedRoles = new List<string>(),
                CheckedSites = new List<int>()
            });

            Assert.IsType<UnauthorizedResult>(result.Result);
            administratorRepository.Verify(x => x.GetByUserIdAsync(It.IsAny<int>()), Times.Never);
            administratorsInRolesRepository.Verify(x => x.RemoveUserAsync(It.IsAny<string>()), Times.Never);
        }
    }
}
