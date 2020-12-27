using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Stl
{
    [OpenApiIgnore]
    [Route(Constants.ApiPrefix + Constants.ApiStlPrefix)]
    public partial class ActionsRelatedFieldController : ControllerBase
    {
        private const string Route = "sys/stl/actions/related_field/{siteId}";

        private readonly IRelatedFieldItemRepository _relatedFieldItemRepository;

        public ActionsRelatedFieldController(IRelatedFieldItemRepository relatedFieldItemRepository)
        {
            _relatedFieldItemRepository = relatedFieldItemRepository;
        }

        public class SubmitRequest : SiteRequest
        {
            public string Callback { get; set; }
            public int RelatedFieldId { get; set; }
            public int ParentId { get; set; }
        }

        private async Task<string> GetRelatedFieldAsync(int siteId, int relatedFieldId, int parentId)
        {
            var jsonString = new StringBuilder();

            jsonString.Append("[");

            var list = await _relatedFieldItemRepository.GetRelatedFieldItemsAsync(siteId, relatedFieldId, parentId);
            if (list.Any())
            {
                foreach (var itemInfo in list)
                {
                    jsonString.AppendFormat(@"{{""id"":""{0}"",""name"":""{1}"",""value"":""{2}""}},", itemInfo.Id, StringUtils.ToJsString(itemInfo.Label), StringUtils.ToJsString(itemInfo.Value));
                }
                jsonString.Length -= 1;
            }

            jsonString.Append("]");
            return jsonString.ToString();
        }
    }
}
