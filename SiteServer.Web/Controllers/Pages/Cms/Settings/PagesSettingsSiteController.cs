using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Datory;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    [RoutePrefix("pages/cms/settings/settingsSite")]
    public partial class PagesSettingsSiteController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<GetResult> GetConfig([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ConfigSite))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            var styles = new List<Style>();
            foreach (var style in await DataProvider.TableStyleRepository.GetSiteStyleListAsync(request.SiteId))
            {
                styles.Add(new Style
                {
                    Id = style.Id,
                    AttributeName = style.AttributeName,
                    DisplayName = style.DisplayName,
                    InputType = style.InputType.GetValue(),
                    Rules = TranslateUtils.JsonDeserialize<IEnumerable<TableStyleRule>>(style.RuleValues),
                    Items = style.Items
                });

                if (style.InputType == InputType.Image || 
                    style.InputType == InputType.Video ||
                    style.InputType == InputType.File)
                {
                    site.Set(EditorManager.GetCountName(style), site.Get(EditorManager.GetCountName(style), 0));
                }
                else if (style.InputType == InputType.CheckBox || 
                         style.InputType == InputType.SelectMultiple)
                {
                    var list = Utilities.GetStringList(site.Get(style.AttributeName,
                        string.Empty));
                    site.Set(style.AttributeName, list);
                }
            }

            return new GetResult
            {
                Site = site,
                Styles = styles
            };
        }

        [HttpPost, Route(Route)]
        public async Task<BoolResult> Submit([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ConfigSite))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            var styles = await DataProvider.TableStyleRepository.GetSiteStyleListAsync(request.SiteId);

            foreach (var style in styles)
            {
                var value = request.Get(style.AttributeName, string.Empty);
                var inputType = style.InputType;
                if (inputType == InputType.TextEditor)
                {
                    value = await ContentUtility.TextEditorContentEncodeAsync(site, value);
                    value = UEditorUtils.TranslateToStlElement(value);
                }
                else if (inputType == InputType.Image || 
                         inputType == InputType.Video || 
                         inputType == InputType.File)
                {
                    var count = request.Get(EditorManager.GetCountName(style), 0);
                    site.Set(EditorManager.GetCountName(style), count);
                    for (var n = 1; n <= count; n++)
                    {
                        site.Set(EditorManager.GetExtendName(style, n), request.Get(EditorManager.GetExtendName(style, n), string.Empty));
                    }
                }

                if (inputType == InputType.CheckBox || 
                    style.InputType == InputType.SelectMultiple)
                {
                    var list = request.Get<IEnumerable<object>>(style.AttributeName);
                    site.Set(style.AttributeName, Utilities.ToString(list));
                }
                else
                {
                    site.Set(style.AttributeName, value);
                }

                if (style.IsFormatString)
                {
                    var formatStrong = request.Get($"{style.AttributeName}_formatStrong", false);
                    var formatEm = request.Get($"{style.AttributeName}_formatEM", false);
                    var formatU = request.Get($"{style.AttributeName}_formatU", false);
                    var formatColor = request.Get($"{style.AttributeName}_formatColor", string.Empty);
                    var formatString = ContentUtility.GetTitleFormatString(formatStrong, formatEm, formatU, formatColor);

                    site.Set(ContentAttribute.GetFormatStringAttributeName(style.AttributeName), formatString);
                }

                if (inputType == InputType.Image || inputType == InputType.File || inputType == InputType.Video)
                {
                    var attributeName = ContentAttribute.GetExtendAttributeName(style.AttributeName);
                    site.Set(attributeName, request.Get(attributeName, string.Empty));
                }
            }

            site.SiteName = request.SiteName;
            site.PageSize = request.PageSize;
            site.IsCreateDoubleClick = request.IsCreateDoubleClick;
            await DataProvider.SiteRepository.UpdateAsync(site);

            await auth.AddSiteLogAsync(request.SiteId, "修改站点设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
