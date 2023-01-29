using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Forms
{
    public partial class FormDataController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var formPermission = MenuUtils.GetFormPermission(request.FormId);
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, formPermission))
            {
                return Unauthorized();
            }

            var form = await _formRepository.GetAsync(request.SiteId, request.FormId);
            if (form == null) return NotFound();

            var site = await _siteRepository.GetAsync(request.SiteId);
            var styles = await _formRepository.GetTableStylesAsync(form.Id);

            var listAttributeNames = ListUtils.GetStringList(form.ListAttributeNames);
            var allAttributeNames = _formRepository.GetAllAttributeNames(styles);
            var pageSize = _formRepository.GetPageSize(form);

            var (total, contents) = await _formDataRepository.GetListAsync(form, false, request.StartDate, request.EndDate, request.Keyword, request.Page, pageSize);
            var items = new List<FormData>();
            foreach (var data in contents)
            {
                var item = data.Clone<FormData>();
                if (data.ChannelId > 0 && listAttributeNames.Contains(nameof(FormData.ChannelId)))
                {
                    var channelName = await _channelRepository.GetChannelNameNavigationAsync(data.SiteId, data.ChannelId);
                    item.Set("channelPage", channelName);
                }
                if (data.ContentId > 0 && listAttributeNames.Contains(nameof(FormData.ContentId)))
                {
                    var content = await _contentRepository.GetAsync(data.SiteId, data.ChannelId, data.ContentId);
                    var title = content != null ? content.Title : string.Empty;
                    item.Set("contentPage", title);
                }
                foreach (var style in styles)
                {
                    if (listAttributeNames.Contains(style.AttributeName))
                    {
                        if (style.InputType == InputType.Image)
                        {
                            var imageUrl = data.Get<string>(style.AttributeName);
                            imageUrl = await _pathManager.ParseSiteUrlAsync(site, imageUrl, true);
                            item.Set(style.AttributeName, imageUrl);
                        }
                    }
                }
                items.Add(item);
            }

            var columns = _formRepository.GetColumns(listAttributeNames, styles, form.IsReply);

            var isSmsEnabled = await _smsManager.IsSmsEnabledAsync();
            if (isSmsEnabled && form.IsSms)
            {
                allAttributeNames.Add("SmsMobile");
                columns.Add(new ContentColumn
                {
                    AttributeName = "SmsMobile",
                    DisplayName = "短信验证手机号码",
                    IsList = ListUtils.ContainsIgnoreCase(listAttributeNames, "SmsMobile")
                });
            }

            return new GetResult
            {
                Items = items,
                Total = total,
                PageSize = pageSize,
                Styles = styles,
                AllAttributeNames = allAttributeNames,
                ListAttributeNames = listAttributeNames,
                IsReply = form.IsReply,
                Columns = columns
            };
        }
    }
}
