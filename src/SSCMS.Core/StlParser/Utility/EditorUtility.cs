using System.Threading.Tasks;
using SSCMS.Parse;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.Utility
{
    public static class EditorUtility
    {
        public static async Task<string> ParseAsync(ParsePage pageInfo, string parsedContent)
        {
            if (parsedContent.Contains(" data-vue="))
            {
                await pageInfo.AddPageHeadCodeIfNotExistsAsync(ParsePage.Const.Jquery);
                await pageInfo.AddPageHeadCodeIfNotExistsAsync(ParsePage.Const.Vue);
                await pageInfo.AddPageHeadCodeIfNotExistsAsync(ParsePage.Const.VueElement);

                var elementId = StringUtils.GetElementId();
                parsedContent = parsedContent.Replace("<p>", "<div>");
                parsedContent = parsedContent.Replace("<p ", "<div ");
                parsedContent = parsedContent.Replace("</p>", "</div>");
                parsedContent = $@"
<div id=""vue_{elementId}""></div>
<script type=""text/javascript"">
var templates_{elementId} = '{parsedContent}';
var container_{elementId} = $(templates_{elementId});
var elements_{elementId} = container_{elementId}.find('[data-vue]');
for(var i = 0; i < elements_{elementId}.length; i++) {{
  var element = $(elements_{elementId}[i]);
  var vueHtml = decodeURIComponent(element.data('vue'));
  templates_{elementId} = templates_{elementId}.replace(elements_{elementId}[i].outerHTML, vueHtml);
}}
$('#vue_{elementId}').html(templates_{elementId});

var $vue_{elementId} = new Vue({{
  el: ""#vue_{elementId}"",
  data: {{
    show: false
  }},
  mounted: function () {{
    this.show = true;
  }}
}});
</script>
";
            }

            return parsedContent;
        }
    }
}