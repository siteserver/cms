using System.Collections.Generic;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Cache.Content;
using SS.CMS.Core.Common;
using SS.CMS.Core.Packaging;
using SS.CMS.Plugin;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Core.Services.Admin
{
    public class IndexService : ServiceBase
    {
        public const string Route = "index";
        public const string RouteUnCheckedList = "index/unCheckedList";

        public ResponseResult<object> Get(IRequest request, IResponse response)
        {
            if (!request.IsAdminLoggin)
            {
                return Unauthorized();
            }

            var adminInfo = AdminManager.GetAdminInfoByUserId(request.AdminId);

            return Ok(new
            {
                Value = new
                {
                    Version = SystemManager.ProductVersion == PackageUtils.VersionDev ? "dev" : SystemManager.ProductVersion,
                    LastActivityDate = DateUtils.GetDateString(adminInfo.LastActivityDate, EDateFormatType.Chinese),
                    UpdateDate = DateUtils.GetDateString(ConfigManager.Instance.UpdateDate, EDateFormatType.Chinese)
                }
            });
        }

        public ResponseResult<object> GetUnCheckedList(IRequest request, IResponse response)
        {
            if (!request.IsAdminLoggin)
            {
                return Unauthorized();
            }

            var unCheckedList = new List<object>();

            foreach (var siteInfo in SiteManager.GetSiteInfoList())
            {
                if (!request.AdminPermissions.IsSiteAdmin(siteInfo.Id)) continue;

                var count = ContentManager.GetCount(siteInfo, false);
                if (count > 0)
                {
                    unCheckedList.Add(new
                    {
                        //Url = PageContentSearch.GetRedirectUrlCheck(siteInfo.Id),
                        Url = PageUtils.UnclickedUrl,
                        siteInfo.SiteName,
                        Count = count
                    });
                }
            }

            return Ok(new
            {
                Value = unCheckedList
            });
        }
    }
}