using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Forms
{
    public partial class FormDataLayerReplyController
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

            var styles = await _formRepository.GetTableStylesAsync(form.Id);
            var data = await _formDataRepository.GetAsync(request.DataId);
            if (data.ChannelId > 0)
            {
                var channelName = await _channelRepository.GetChannelNameNavigationAsync(data.SiteId, data.ChannelId);
                data.Set("channelPage", channelName);
            }
            if (data.ContentId > 0)
            {
                var content = await _contentRepository.GetAsync(data.SiteId, data.ChannelId, data.ContentId);
                var title = content != null ? content.Title : string.Empty;
                data.Set("contentPage", title);
            }
            
            var attributeNames = _formRepository.GetAllAttributeNames(styles);
            attributeNames.Remove(nameof(FormData.IsReplied));
            attributeNames.Remove(nameof(FormData.ReplyDate));
            attributeNames.Remove(nameof(FormData.ReplyContent));

            var allAttributeNames = _formRepository.GetAllAttributeNames(styles);
            var columns = _formRepository.GetColumns(allAttributeNames, styles, true);

            return new GetResult
            {
                Styles = styles,
                Columns = columns,
                FormData = data,
                AttributeNames = attributeNames
            };
        }
    }
}
