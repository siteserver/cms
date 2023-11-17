using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class FormsController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            Form form = null;
            if (request.FormId > 0)
            {
                form = await _formRepository.GetAsync(request.SiteId, request.FormId);
            }
            else if (!string.IsNullOrEmpty(request.FormName))
            {
                form = await _formRepository.GetByTitleAsync(request.SiteId, request.FormName);
            }

            if (form == null) 
            {
                return this.Error(Constants.ErrorNotFound);
            }

            var styles = await _formRepository.GetTableStylesAsync(form.Id);

            var listAttributeNames = ListUtils.GetStringList(form.ListAttributeNames);
            var allAttributeNames = _formRepository.GetAllAttributeNames(styles);
            var pageSize = request.PerPage > 0 ? request.PerPage : _formRepository.GetPageSize(form);

            var isRepliedOnly = form.IsReply && !form.IsReplyListAll;
            var (total, items) = await _formDataRepository.GetListAsync(form, isRepliedOnly, null, null, request.Word, request.Page, pageSize);
            var columns = _formRepository.GetColumns(listAttributeNames, styles, form.IsReply);

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
