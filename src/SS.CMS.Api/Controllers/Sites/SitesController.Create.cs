using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Api.Common;
using SS.CMS.Core.Common.Enums;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Api.Controllers.Sites
{
    public partial class SitesController
    {
        /// <summary>
        /// Create New Site
        /// </summary>
        /// <returns>User info</returns>
        /// <response code="200">Returns the created site id</response>
        /// <response code="401">Unauthorized</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [Authorize]
        [ClaimRequirement(AuthTypes.ClaimTypes.Role, AuthTypes.Roles.SuperAdministrator)]
        [HttpPost("")]
        public async Task<ActionResult<CreateResponse>> Create(CreateRequest request)
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
                    await _databaseRepository.CreateContentTableAsync(tableName, _databaseRepository.ContentTableDefaultColumns);
                }
                else
                {
                    await _databaseRepository.AlterSystemTableAsync(tableName, _databaseRepository.ContentTableDefaultColumns);
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
                await _databaseRepository.CreateContentTableAsync(tableName, _databaseRepository.ContentTableDefaultColumns);
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

            return new CreateResponse
            {
                Id = siteId
            };
        }

        public class CreateRequest
        {
            /// <summary>
            /// create type: "empty": create site from scratch, "local": create site from local site templates, "cloud": create site from cloud site templates
            /// </summary>
            public string CreateType { get; set; }

            /// <summary>
            /// site template id
            /// </summary>
            public string CreateTemplateId { get; set; }
            public string SiteName { get; set; }
            public bool IsRoot { get; set; }
            public int ParentId { get; set; }
            public string SiteDir { get; set; }
            public string TableRule { get; set; }
            public string TableChoose { get; set; }
            public string TableHandWrite { get; set; }
            public bool IsImportContents { get; set; }
            public bool IsImportTableStyles { get; set; }
        }

        public class CreateResponse
        {
            public int Id { get; set; }
        }
    }
}
