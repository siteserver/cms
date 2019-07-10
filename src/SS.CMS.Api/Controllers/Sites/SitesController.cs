using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Api.Common;
using SS.CMS.Core.Models.Enumerations;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Api.Controllers.Sites
{
    [ApiController]
    [AllowAnonymous]
    [Route("sites")]
    public partial class SitesController : ControllerBase
    {
        public const string Route = "";
        public const string RouteTableNames = "tableNames";
        private readonly ISettingsManager _settingsManager;
        private readonly IDatabase _database;
        private readonly IPluginManager _pluginManager;
        private readonly IUserManager _userManager;
        private readonly ITableManager _tableManager;
        private readonly IUserRepository _userRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly ITemplateRepository _templateRepository;

        public SitesController(ISettingsManager settingsManager, IDatabase database, IPluginManager pluginManager, IUserManager userManager, ITableManager tableManager, IUserRepository userRepository, ISiteRepository siteRepository, IChannelRepository channelRepository, ITemplateRepository templateRepository)
        {
            _settingsManager = settingsManager;
            _database = database;
            _pluginManager = pluginManager;
            _userManager = userManager;
            _tableManager = tableManager;
            _userRepository = userRepository;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _templateRepository = templateRepository;
        }

        [Authorize]
        [HttpGet(Route)]
        public async Task<ActionResult<IList<Site>>> List()
        {
            var list = await _siteRepository.GetSiteInfoListAsync(0);

            return list.ToList();
        }

        [Authorize]
        [ClaimRequirement(AuthTypes.ClaimTypes.Role, AuthTypes.Roles.SuperAdministrator)]
        [HttpPost(Route)]
        public async Task<ActionResult<int>> Create(CreateRequest request)
        {
            if (!_userManager.IsSuperAdministrator())
            {
                return Unauthorized();
            }

            var tableRule = ETableRuleUtils.GetEnumType(request.TableRule);

            if (!request.IsRoot)
            {
                if (!DirectoryUtils.IsDirectoryNameCompliant(request.SiteDir))
                {
                    return BadRequest("文件夹名称不符合系统要求，请更改文件夹名称！");
                }
                var list = await _siteRepository.GetSiteDirListAsync(request.ParentId);
                if (StringUtils.ContainsIgnoreCase(list, request.SiteDir))
                {
                    return BadRequest("已存在相同的发布路径，请更改文件夹名称！");
                }
            }

            var tableName = string.Empty;
            if (tableRule == ETableRule.Choose)
            {
                tableName = request.TableChoose;
            }
            else if (tableRule == ETableRule.HandWrite)
            {
                tableName = request.TableHandWrite;

                if (!await _database.IsTableExistsAsync(tableName))
                {
                    await _tableManager.CreateContentTableAsync(tableName, _tableManager.ContentTableDefaultColumns);
                }
                else
                {
                    await _tableManager.AlterSystemTableAsync(tableName, _tableManager.ContentTableDefaultColumns);
                }
            }

            var siteInfo = new Site
            {
                SiteName = request.SiteName,
                SiteDir = request.SiteDir,
                TableName = tableName,
                ParentId = request.ParentId,
                IsRoot = request.IsRoot,
                IsCheckContentLevel = false
            };

            var siteId = await _siteRepository.InsertAsync(siteInfo);

            var channelInfo = new Channel
            {
                SiteId = siteId,
                ChannelName = "首页",
                IndexName = "首页",
                ParentId = 0,
                ContentModelPluginId = string.Empty
            };

            var channelId = await _channelRepository.InsertAsync(channelInfo);

            var userInfo = await _userManager.GetUserAsync();
            await _userRepository.UpdateSiteIdAsync(userInfo, siteId);

            await _templateRepository.CreateDefaultTemplateInfoAsync(siteId, userInfo.Id);


            // var siteId = _channelRepository.Insert.InsertSiteInfo(channelInfo, siteInfo, request.AdminName);

            if (string.IsNullOrEmpty(tableName))
            {
                tableName = StringUtils.GetContentTableName(siteId);
                await _tableManager.CreateContentTableAsync(tableName, _tableManager.ContentTableDefaultColumns);
                await _siteRepository.UpdateTableNameAsync(siteId, tableName);
            }

            var siteTemplateDir = string.Empty;
            var onlineTemplateName = string.Empty;
            if (StringUtils.EqualsIgnoreCase(request.CreateType, "local"))
            {
                siteTemplateDir = request.CreateTemplateId;
            }
            else if (StringUtils.EqualsIgnoreCase(request.CreateType, "cloud"))
            {
                onlineTemplateName = request.CreateTemplateId;
            }

            // var redirectUrl = PageProgressBar.GetCreateSiteUrl(siteId,
            //     isImportContents, isImportTableStyles, siteTemplateDir, onlineTemplateName, StringUtils.Guid());

            // return Ok(new
            // {
            //     Value = redirectUrl
            // });

            return siteId;
        }

        [Authorize]
        [HttpGet(RouteTableNames)]
        public async Task<ActionResult<IList<string>>> GetTableNames()
        {
            var tableNames = await _siteRepository.GetSiteTableNamesAsync(_pluginManager);

            return tableNames;
        }
    }
}