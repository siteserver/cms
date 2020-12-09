using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<SitesResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSites))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (!StringUtils.EqualsIgnoreCase(site.SiteDir, request.SiteDir))
            {
                return this.Error("删除失败，请输入正确的文件夹名称");
            }
            if (site.Children != null && site.Children.Count > 0)
            {
                return this.Error("删除失败，不允许删除父站点，在删除父站点前请先删除子站点");
            }

            if (request.DeleteFiles)
            {
                await _pathManager.DeleteSiteFilesAsync(site);
            }
            await _authManager.AddAdminLogAsync("删除站点", $"站点:{site.SiteName}");

            var list = await _channelRepository.GetChannelIdsAsync(request.SiteId);
            await _tableStyleRepository.DeleteAllAsync(site.TableName, list);
            await _contentGroupRepository.DeleteAsync(request.SiteId);
            await _contentTagRepository.DeleteAsync(request.SiteId);
            await _channelRepository.DeleteAllAsync(request.SiteId);

            await _siteRepository.DeleteAsync(request.SiteId);

            var siteIdList = await _siteRepository.GetSiteIdsAsync(0);
            var sites = new List<Site>();
            foreach (var id in siteIdList)
            {
                var info = await _siteRepository.GetAsync(id);
                if (info != null)
                {
                    sites.Add(info);
                }
            }

            return new SitesResult
            {
                Sites = sites
            };
        }
    }
}