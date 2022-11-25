using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsSiteController : ControllerBase
    {
        private const string Route = "cms/settings/settingsSite";

        private readonly IAuthManager _authManager;
        private readonly ICloudManager _cloudManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly IRelatedFieldItemRepository _relatedFieldItemRepository;

        public SettingsSiteController(IAuthManager authManager, ICloudManager cloudManager, IPathManager pathManager, ISiteRepository siteRepository, ITableStyleRepository tableStyleRepository, IRelatedFieldItemRepository relatedFieldItemRepository)
        {
            _authManager = authManager;
            _cloudManager = cloudManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _tableStyleRepository = tableStyleRepository;
            _relatedFieldItemRepository = relatedFieldItemRepository;
        }

        public class Settings
        {
            public bool IsCloudImages { get; set; }
        }

        public class GetResult
        {
            public string SiteUrl { get; set; }
            public Entity Entity { get; set; }
            public IEnumerable<InputStyle> Styles { get; set; }
            public Dictionary<int, List<Dto.Cascade<int>>> RelatedFields { get; set; }
            public Settings Settings { get; set; }
        }

        public class SubmitRequest : Entity
        {
            public int SiteId { get; set; }
        }

        private async Task<List<InputStyle>> GetInputStylesAsync(int siteId)
        {
            var styles = new List<InputStyle>
            {
                new InputStyle
                {
                    AttributeName = nameof(Site.SiteName),
                    DisplayName = "站点名称",
                    InputType = InputType.Text,
                    Rules = new List<InputStyleRule>
                    {
                        new InputStyleRule
                        {
                            Type = ValidateType.Required,
                            Message = "请输入站点名称"
                        }
                    }
                },
                new InputStyle
                {
                    AttributeName = nameof(Site.ImageUrl),
                    DisplayName = "站点图片/LOGO",
                    InputType = InputType.Image
                },
                new InputStyle
                {
                    AttributeName = nameof(Site.Keywords),
                    DisplayName = "站点关键字",
                    InputType = InputType.Text
                },
                new InputStyle
                {
                    AttributeName = nameof(Site.Description),
                    DisplayName = "站点描述",
                    InputType = InputType.TextArea
                }
            };
            var tableStyles = await _tableStyleRepository.GetSiteStylesAsync(siteId);
            styles.AddRange(tableStyles.Select(x => new InputStyle(x)));

            return styles;
        }
    }
}
