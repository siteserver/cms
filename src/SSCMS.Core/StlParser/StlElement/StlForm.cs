using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SSCMS.Configuration;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Models;
using SSCMS.Parse;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "表单", Description = "通过 stl:form 标签在模板中显示表单")]
    public static class StlForm
    {
        public const string ElementName = "stl:form";

        [StlAttribute(Title = "表单名称")]
        private const string Name = nameof(Name);

        [StlAttribute(Title = "表单模板")]
        private const string Type = nameof(Type);

        [StlAttribute(Title = "高度")]
        private const string Height = nameof(Height);

        [StlAttribute(Title = "提交表单的JS函数名称")]
        private const string SubmitName = nameof(SubmitName);

        [StlAttribute(Title = "提交表单发送前执行的JS代码")]
        private const string OnBeforeSend = nameof(OnBeforeSend);

        [StlAttribute(Title = "提交表单成功后执行的JS代码")]
        private const string OnSuccess = nameof(OnSuccess);

        [StlAttribute(Title = "提交表单结束后执行的JS代码")]
        private const string OnComplete = nameof(OnComplete);

        [StlAttribute(Title = "提交表单失败后执行的JS代码")]
        private const string OnError = nameof(OnError);

        [StlAttribute(Title = "提交表单成功后显示的提示文字")]
        private const string SuccessMessage = nameof(SuccessMessage);

        internal static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var context = parseManager.ContextInfo;
            var pageInfo = parseManager.PageInfo;

            var formName = string.Empty;
            var type = "submit";
            var height = string.Empty;
            var submitName = "formSubmit";
            var onBeforeSend = string.Empty;
            var onSuccess = string.Empty;
            var onComplete = string.Empty;
            var onError = string.Empty;
            var attributes = new NameValueCollection();

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Name) || StringUtils.EqualsIgnoreCase(name, "Title"))
                {
                    formName = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    height = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, SubmitName))
                {
                    submitName = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OnBeforeSend))
                {
                    onBeforeSend = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OnSuccess))
                {
                    onSuccess = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OnComplete))
                {
                    onComplete = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OnError))
                {
                    onError = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else
                {
                    attributes[name] = value;
                }
            }

            var formRepository = parseManager.DatabaseManager.FormRepository;
            var siteRepository = parseManager.DatabaseManager.SiteRepository;
            var pathManager = parseManager.PathManager;
            var site = context.Site;

            var form = !string.IsNullOrEmpty(formName) ? await formRepository.GetByTitleAsync(site.Id, formName) : null;

            if (form == null)
            {
                var forms = await formRepository.GetFormsAsync(site.Id);
                if (forms != null && forms.Count > 0)
                {
                    form = forms[0];
                }
            }

            if (form == null) return string.Empty;

            var apiUrl = pathManager.GetApiHostUrl(site, Constants.ApiPrefix);

            if (string.IsNullOrWhiteSpace(context.InnerHtml))
            {
                return await ParseAsync(parseManager, context, site, form, apiUrl, type, height, attributes);
            }
            else
            {
                return await ParseInnerHtmlAsync(parseManager, context, pageInfo, site, form, apiUrl, submitName, onBeforeSend, onSuccess, onComplete, onError, attributes);
            }
        }

        private static async Task<object> ParseAsync(IParseManager parseManager, ParseContext context, Site site, Form form, string apiUrl, string type, string height, NameValueCollection attributes)
        {
            var elementId = StringUtils.GetElementId();
            var libUrl = parseManager.PathManager.GetApiHostUrl(site, "sitefiles/assets/lib/iframe-resizer-4.3.6/iframeResizer.min.js");

            var formTemplate = await parseManager.FormManager.GetFormTemplateAsync(site, type);
            if (formTemplate == null)
            {
                formTemplate = await parseManager.FormManager.GetFormTemplateAsync(site, "submit");
            }
            var pageUrl = string.Empty;
            if (formTemplate.IsSystem)
            {
                pageUrl = parseManager.PathManager.GetApiHostUrl(site, $"sitefiles/assets/forms/{type}/index.html?siteId={site.Id}&channelId={context.ChannelId}&contentId={context.ContentId}&formId={form.Id}&apiUrl={HttpUtility.UrlEncode(apiUrl)}");
            }
            else
            {
                pageUrl = await parseManager.PathManager.GetSiteUrlAsync(site, $"forms/{type}/index.html?siteId={site.Id}&channelId={context.ChannelId}&contentId={context.ContentId}&formId={form.Id}&apiUrl={HttpUtility.UrlEncode(apiUrl)}", false);
            }

            var heightStyle = !string.IsNullOrEmpty(height) ? $"height: {height}" : string.Empty;
            var frameResize = string.Empty;
            if (!string.IsNullOrEmpty(height))
            {
                heightStyle = $"height: {StringUtils.AddUnitIfNotExists(height)}";
            }
            else
            {
                frameResize = $@"
<script type=""text/javascript"" src=""{libUrl}""></script>
<script type=""text/javascript"">iFrameResize({{log: false}}, '#{elementId}')</script>
";
            }

            return $@"
<iframe id=""{elementId}"" frameborder=""0"" scrolling=""no"" src=""{pageUrl}"" style=""width: 1px;min-width: 100%;min-height: 200px;{heightStyle}""></iframe>
{frameResize}
";
        }

        private static async Task<object> ParseInnerHtmlAsync(IParseManager parseManager, ParseContext context, ParsePage pageInfo, Site site, Form form, string apiUrl, string submitName, string onBeforeSend, string onSuccess, string onComplete, string onError, NameValueCollection attributes)
        {
            await pageInfo.AddPageHeadCodeIfNotExistsAsync(ParsePage.Const.Axios);

            var htmlId = attributes["id"];
            if (string.IsNullOrEmpty(htmlId))
            {
                htmlId = StringUtils.GetElementId();
            }
            attributes["id"] = htmlId;
            var onSubmit = attributes["onSubmit"];
            if (string.IsNullOrEmpty(onSubmit))
            {
                onSubmit = "return false;";
            }
            attributes["onSubmit"] = onSubmit;

            var innerHtml = context.InnerHtml;
            var successTemplateString = string.Empty;
            var failureTemplateString = string.Empty;
            var stlElementList = ParseUtils.GetStlElements(innerHtml);
            if (stlElementList.Count > 0)
            {
                foreach (var theStlElement in stlElementList)
                {
                    if (ParseUtils.IsSpecifiedStlElement(theStlElement, StlYes.ElementName) || ParseUtils.IsSpecifiedStlElement(theStlElement, StlYes.ElementName2))
                    {
                        successTemplateString = ParseUtils.GetInnerHtml(theStlElement);
                        innerHtml = innerHtml.Replace(successTemplateString, string.Empty);
                    }
                    else if (ParseUtils.IsSpecifiedStlElement(theStlElement, StlNo.ElementName) || ParseUtils.IsSpecifiedStlElement(theStlElement, StlNo.ElementName2))
                    {
                        failureTemplateString = ParseUtils.GetInnerHtml(theStlElement);
                        innerHtml = innerHtml.Replace(successTemplateString, string.Empty);
                    }
                }
            }

            if (string.IsNullOrEmpty(successTemplateString))
            {
                successTemplateString = """<div class="form-alert form-alert-success">表单提交成功！</div>""";
            }
            if (string.IsNullOrEmpty(failureTemplateString))
            {
                failureTemplateString = """<div class="form-alert form-alert-failure">表单提交失败！</div>""";
            }

            var innerBuilder = new StringBuilder(innerHtml);
            await parseManager.ParseInnerContentAsync(innerBuilder);
            innerHtml = innerBuilder.ToString();

            if (StringUtils.IsStrictName(onBeforeSend))
            {
                onBeforeSend = $"""if (!{onBeforeSend}(data)) return;""";
            }

            if (string.IsNullOrEmpty(onSuccess))
            {
                onSuccess = $"""
var elem = document.querySelector('#{htmlId}');
elem.innerHTML = '{StringUtils.ToJsString(successTemplateString)}';
""";
            }
            else if (StringUtils.IsStrictName(onSuccess))
            {
                onSuccess += "(res);";
            }

            if (string.IsNullOrEmpty(onError))
            {
                onError = $"""
var elem = document.querySelector('#{htmlId}');
elem.innerHTML = '{StringUtils.ToJsString(failureTemplateString)}';
""";
            }
            else if (StringUtils.IsStrictName(onError))
            {
                onError += "(err);";
            }

            if (StringUtils.IsStrictName(onComplete))
            {
                onComplete += "();";
            }

            return $$"""
<style>
.form-alert { width: 100%; text-align: center; padding: 60px 0; font-size: 18px; border-radius: 10px; }
.form-alert-success { background-color: #f0f9eb; color: #67c23a; }
.form-alert-failure { background-color: #fef0f0; color: #f56c6c; }
</style>
<script>
var $formConfigApiUrl = '{{apiUrl}}';
var $formConfigSiteId = {{site.Id}};
var $formConfigChannelId = {{context.ChannelId}};
var $formConfigContentId = {{context.ContentId}};
var $formConfigFormId = {{form.Id}};
function {{submitName}}(data) {
  if (!data) {
    var formData = new FormData(document.getElementById("{{htmlId}}"));
    data = {};
    formData.forEach(function(value, key){
        data[key] = value;
    });
  }
  {{onBeforeSend}}
  axios.post('{{apiUrl}}/v1/forms?siteId={{site.Id}}&channelId={{context.ChannelId}}&contentId={{context.ContentId}}&formId={{form.Id}}', data).then(function (response) {
    var res = response.data;
    {{onSuccess}}
  }).catch(function (err) {
    {{onError}}
  }).then(function () {
    {{onComplete}}
  });
  return false;
}
</script>
<form {{TranslateUtils.ToAttributesString(attributes)}}>
{{innerHtml}}
</form>
""";
        }
    }
}
