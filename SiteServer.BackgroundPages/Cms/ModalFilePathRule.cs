using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalFilePathRule : BasePageCms
    {
        public Literal LtlRules;
        public TextBox TbRule;

        private int _channelId;
        private bool _isChannel;
        private string _textBoxClientId = string.Empty;

        public static string GetOpenWindowString(int siteId, int channelId, bool isChannel, string textBoxclientId)
        {
            return LayerUtils.GetOpenScriptWithTextBoxValue(isChannel ? "栏目页文件名规则" : "内容页文件名规则", PageUtils.GetCmsUrl(siteId, nameof(ModalFilePathRule), new NameValueCollection
            {
                {"channelId", channelId.ToString()},
                {"IsChannel", isChannel.ToString()},
                {"TextBoxClientID", textBoxclientId}
            }), textBoxclientId);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _channelId = AuthRequest.GetQueryInt("channelId");
            _isChannel = AuthRequest.GetQueryBool("IsChannel");
            _textBoxClientId = AuthRequest.GetQueryString("TextBoxClientID");

            if (IsPostBack) return;

            LtlRules.Text = GetRulesString();
            if (!string.IsNullOrEmpty(_textBoxClientId))
            {
                TbRule.Text = AuthRequest.GetQueryString(_textBoxClientId);
            }

            InfoMessage(_isChannel
                ? "系统生成栏目页时采取的文件名规则，建议保留{@ChannelId}栏目Id项，否则可能出现重复的文件名称"
                : "系统生成内容页时采取的文件名规则，建议保留{@ContentId}内容Id项，否则可能出现重复的文件名称");
        }

        private string GetRulesString()
        {
            var builder = new StringBuilder();
            var mod = 0;
            var count = 0;
            var entitiesDictionary = _isChannel
                ? PathUtility.ChannelFilePathRules.GetDictionary(SiteInfo, _channelId)
                : PathUtility.ContentFilePathRules.GetDictionary(SiteInfo, _channelId);
            
            foreach (string label in entitiesDictionary.Keys)
            {
                count++;
                var labelName = (string)entitiesDictionary[label];
                string td =
                    $@"<td><a href=""javascript:;"" onclick=""AddOnPos('{label}');return false;"">{label}</a></td><td>{labelName}</td>";
                if (count == entitiesDictionary.Count)
                {
                    td =
                        $@"<td><a href=""javascript:;"" onclick=""AddOnPos('{label}');return false;"">{label}</a></td><td colspan=""5"">{labelName}</td></tr>";
                }
                if (mod++ % 3 == 0)
                {
                    builder.Append("<tr>" + td);
                }
                else
                {
                    builder.Append(td);
                }
            }

            return builder.ToString();
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            string scripts = $"window.parent.document.all.{_textBoxClientId}.value = '{TbRule.Text}';";
            LayerUtils.CloseWithoutRefresh(Page, scripts);
		}
	}
}
