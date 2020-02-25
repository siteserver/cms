using System.Threading.Tasks;
using SS.CMS.StlParser.Model;

namespace SS.CMS.StlParser.Utility
{
    public static class EditorUtility
    {
        public static async Task<string> ParseAsync(PageInfo pageInfo, string parsedContent)
        {
            if (parsedContent.Contains(" data-vue="))
            {
                await pageInfo.AddPageHeadCodeIfNotExistsAsync(PageInfo.Const.Jquery);
                await pageInfo.AddPageHeadCodeIfNotExistsAsync(PageInfo.Const.Vue);
                await pageInfo.AddPageHeadCodeIfNotExistsAsync(PageInfo.Const.VueElement);

                var uniqueId = pageInfo.UniqueId;
                parsedContent = parsedContent.Replace("<p>", "<div>");
                parsedContent = parsedContent.Replace("</p>", "</div>");
                parsedContent = $@"
<div id=""vue_{uniqueId}""></div>
<script type=""text/javascript"">
var templates_{uniqueId} = '{parsedContent}';
var container_{uniqueId} = $(templates_{uniqueId});
var elements_{uniqueId} = container_{uniqueId}.find('[data-vue]');
for(var i = 0; i < elements_{uniqueId}.length; i++) {{
  var element = $(elements_{uniqueId}[i]);
  var vueHtml = decodeURIComponent(element.data('vue'));
  templates_{uniqueId} = templates_{uniqueId}.replace(elements_{uniqueId}[i].outerHTML, vueHtml);
}}
$('#vue_{uniqueId}').html(templates_{uniqueId});

var $vue_{uniqueId} = new Vue({{
  el: ""#vue_{uniqueId}"",
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